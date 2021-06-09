Option Strict On
Imports System.Net.Sockets
'**********************************************************************************************
'* Atlas Copco Data Link Layer
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* support@advancedhmi.com
'* 28-DEC-20
'*
'* This class implements
'* 
'*
'* Reference : 
'*
'*
'* Copyright (c) 2021 Manufacturing Automation, LLC
'*
'*  This library Is free software; you can redistribute it And/Or
'*  modify it under the terms Of the GNU Lesser General Public
'*  License as published by the Free Software Foundation; either
'*  version 2.1 Of the License, Or (at your option) any later version.
'*  See the file COPYING included With this distribution For more
'*  information.

'**********************************************************************************************
Public Class AtlasCopcoDataLinkLayer
    Implements IDisposable
    Private disposed As Boolean = False


    Private TransportLayer As AtlasCopcoTransport

    Public Event DataReceived As EventHandler(Of Common.PlcComEventArgs)
    Public Event ConnectionClosed As EventHandler
    Public Event ConnectionEstablished As EventHandler
    Public Event ComError As EventHandler(Of Common.PlcComEventArgs)

    'Private Responded(255) As Boolean
    'Protected waitHandle(255) As System.Threading.EventWaitHandle
    Protected waitHandle As System.Threading.EventWaitHandle

    'Private SendQueueThread As System.Threading.Thread
    Private SendQueueTask As System.Threading.Tasks.Task
    Protected QueHoldRelease As New System.Threading.EventWaitHandle(False, Threading.EventResetMode.AutoReset)


#Region "Properties"
    '* Create a property for the IPaddress and store it in this private variable
    '* with a property you can validate the values before accepting
    Public Property IPAddress() As String
        Get
            Return TransportLayer.IPAddress
        End Get
        Set(ByVal value As String)
            TransportLayer.IPAddress = value
        End Set
    End Property

    Public Property Port As UShort  ' 4545
        Get
            Return TransportLayer.Port
        End Get
        Set(value As UShort)
            TransportLayer.Port = value
        End Set
    End Property

    Private m_ConnectionCount As Integer
    Public Property ConnectionCount As Integer
        Get
            Return m_ConnectionCount
        End Get
        Set(value As Integer)
            m_ConnectionCount = value
        End Set
    End Property

    Private m_Timeout As Integer = 2000
    Public Property Timeout As Integer
        Get
            Return m_Timeout
        End Get
        Set(value As Integer)
            m_Timeout = value
        End Set
    End Property
#End Region

#Region "ConstructorDestructor"
    Public Sub New()
        MyBase.New()


        'If Utilities.CompCheck Then

        '        For i = 0 To 255
        '       waitHandle(i) = New System.Threading.EventWaitHandle(False, System.Threading.EventResetMode.AutoReset)
        '      Next
        waitHandle = New System.Threading.EventWaitHandle(False, System.Threading.EventResetMode.AutoReset)



        TransportLayer = New AtlasCopcoTransport
        '* Tell the transport about the packet header
        TransportLayer.HeaderSize = 20
        TransportLayer.Port = 4545


        AddHandler TransportLayer.DataReceived, AddressOf TransportDataReceived
        AddHandler TransportLayer.ConnectionClosed, AddressOf TransportConnectionClosed
        AddHandler TransportLayer.ConnectionEstablished, AddressOf TransportConnectionEstablished
        AddHandler TransportLayer.ComError, AddressOf TransportError
        'End If
    End Sub

    Public Sub New(ByVal ip As String)
        Me.New()
        IPAddress = ip
    End Sub

    Public Sub New(ByVal ip As String, ByVal port As Integer)
        Me.New(ip)
        Me.Port = CUShort(port)
    End Sub


    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If disposed Then Return

        If (disposing) Then
            StopThread = True
            QueHoldRelease.Set()
            disposed = True

            ''* Wait for the thread to stop
            If SendQueueTask IsNot Nothing AndAlso SendQueueTask.Status = Threading.Tasks.TaskStatus.Running Then
                SendQueueTask.Wait(3000)
            End If


            If TransportLayer IsNot Nothing Then
                TransportLayer.CloseConnection()
                TransportLayer.Dispose()
            End If
        End If
    End Sub


    Public Sub Dispose() Implements System.IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

#Region "Public Methods"
    '*********************************
    '* Send data out the tcp socket
    '*********************************
    Public Function SendData(ByVal Frame As MessageFrame) As Integer
        If (disposed) Then
            Throw New ObjectDisposedException("AtlasCopcoDataLinkLayer")
        End If

        If Not StopThread Then
            '* Add this packet to the queue
            QueuePacket(Frame)
        End If

        Return 0
    End Function
#End Region

#Region "Private Methods"
    '************************************************************
    '* Call back procedure - called when data comes back
    '* This is the procedure pointed to by the BeginWrite method
    '************************************************************
    Private Sub TransportDataReceived(sender As Object, ByVal e As Common.TCPComEventArgs)
        Dim response As New MessageFrame(e.StateObject.data, e.StateObject.CurrentIndex)
        'response.TransactionNumber = e.StateObject.TransactionNumber


        '****************************************
        '* Extract the data
        '****************************************
        Dim EncapsulatedData(e.StateObject.CurrentIndex) As Byte
        For index = 0 To response.Data.Count - 1 + 20
            EncapsulatedData(index) = e.StateObject.data(index)
        Next

        Dim ReturnedData As Common.PlcComEventArgs
        '* Check for error (function code + &H80)
        If response.MID <> 4 Or True Then
            '* Be sure there is data
            If EncapsulatedData.Length > 0 Then
                ReturnedData = New Common.PlcComEventArgs(EncapsulatedData, Nothing, 0, e.StateObject.OwnerObjectID)
                '*************************************************
                '* Raise an event to indicate that data was rcvd
                '*************************************************
                Try
                    OnDataReceived(ReturnedData)
                Catch ex As Exception
                    Dim dbg = 0
                End Try
            End If
        Else
            If EncapsulatedData.Length > 0 Then
                '*************************************************
                '* Raise an event to indicate that data was rcvd
                '*************************************************
                Dim ErrorID As Integer
                Dim MessageID As Integer
                Dim MessageData() As Byte = response.Data.ToArray
                If response.Data.Count >= 4 Then
                    Try
                        MessageID = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(MessageData, 0, 4))
                    Catch ex As Exception
                        Throw New Exception("Invald MessageID")
                    End Try
                End If

                If response.Data.Count >= 6 Then
                    Try
                        ErrorID = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(MessageData, 4, 2))
                    Catch ex As Exception
                        Throw New Exception("Invald MessageID")
                    End Try
                End If


                OnComError(New Common.PlcComEventArgs(ErrorID, ErrorCodes.Errors(ErrorID), response.MID, e.StateObject.OwnerObjectID))
            End If
        End If

        'Dim TransactionByte As Integer = response.TransactionNumber And 255

        waitHandle.Set()
    End Sub

    '* The connection was closed from the Transport layer
    Private Sub TransportConnectionClosed(sender As Object, e As EventArgs)
        OnConnectionClosed(System.EventArgs.Empty)
    End Sub

    Private Sub TransportError(sender As Object, e As Common.PlcComEventArgs)
        OnComError(e)
    End Sub

    Private Sub TransportConnectionEstablished(ByVal sender As Object, ByVal e As System.EventArgs)
        OnConnectionEstablished(e)
    End Sub

    Protected Overridable Sub OnConnectionEstablished(ByVal e As System.EventArgs)
        RaiseEvent ConnectionEstablished(Me, e)
    End Sub

    '****************************************************************************************************************************
    '****************************************************************************************************************************
    'Private SendQue As New Queue(Of ModbusTCP.ModbusTCPFrame)
    Private SendQue As New System.Collections.Concurrent.ConcurrentQueue(Of MessageFrame)
    Private SendLockObject As New Object
    Private QueueLockObject As New Object
    Private StopThread, ThreadStarted As Boolean
    Private SendRetries As Integer
    'Private ResponseWaitTicks As Integer


    Public Sub QueuePacket(ByVal e As MessageFrame)
        If (disposed) Then
            Throw New ObjectDisposedException("AtlasTCPDataLinkLayer")
        End If

        If StopThread Then
            Throw New Common.PLCDriverException("Atlas DLL being stopped")
        End If

        If e IsNot Nothing And Not StopThread Then
            If e.GetBytes IsNot Nothing AndAlso e.GetBytes.Length > 0 Then
                SyncLock (QueueLockObject)
                    Try
                        '* a zero indicates an internal request such as ForwardClose
                        If SendQue.Count < 30 Then
                            SendQue.Enqueue(e)


                            QueHoldRelease.Set()
                        End If
                    Catch ex As Exception
                        Return
                        Throw ex
                    End Try
                End SyncLock

                If Not ThreadStarted And Not StopThread Then
                    Try
                        If SendQueueTask Is Nothing OrElse (Not SendQueueTask.Status = Threading.Tasks.TaskStatus.Created And
                                                            Not SendQueueTask.Status = Threading.Tasks.TaskStatus.Running And
                                                            Not SendQueueTask.Status = Threading.Tasks.TaskStatus.WaitingToRun) Then
                            SendQueueTask = System.Threading.Tasks.Task.Factory.StartNew(AddressOf SendQueProcessor)
                        End If

                    Catch ex As Threading.ThreadStateException
                        Using fs1 As New System.IO.FileStream("DriverErrorLog.log", System.IO.FileMode.Append)
                            Using writer As System.IO.StreamWriter = New System.IO.StreamWriter(fs1, System.Text.Encoding.ASCII)
                                writer.Write("DLLSendData-" & ex.Message)
                            End Using
                        End Using
                    End Try
                End If
            End If
        Else
            Dim dbg = 0
        End If

        If SendQue.Count >= 30 Then
            If SendQueueTask IsNot Nothing AndAlso SendQueueTask.Status = Threading.Tasks.TaskStatus.Running Then
                Throw New Common.PLCDriverException(-22, "Send queue full, data request too fast or may have lost connection.")
            End If
        End If
    End Sub

    '*******************************************************************
    '* This runs on a thread to send commands out one at a time
    '*******************************************************************
    Private Sub SendQueProcessor()
        ThreadStarted = True
        Dim item As MessageFrame = Nothing
        Dim WaitResult As Boolean

        While Not StopThread
            Try
                If SendQue.Count > 0 Then
                    SendQue.TryPeek(item)

                    If item IsNot Nothing Then
                        '*****************************************
                        '* Send the data out
                        '*****************************************
                        waitHandle.Reset()
                        Try
                            TransportLayer.SendData(item, True, m_Timeout)

                            'Dim sw As New Stopwatch
                            'sw.Reset()
                            'sw.Start()
                            '* Did the response time out?
                            WaitResult = waitHandle.WaitOne(m_Timeout)
                            'sw.Stop()


                            'If WaitForResponse(item.PDU.TransactionID And 255, 500) <> 0 Then
                            If Not WaitResult Then
                                ' Debug.Print("RETRYING TO SEND COMMAND--------------------" & NotReady & ChecksumFailed)
                                '* if so, then leave in que

                                SendRetries += 1
                                '* Too many retries
                                If SendRetries > 0 Then
                                    SendRetries = 0
                                    If SendQue.Count > 0 Then SendQue.TryDequeue(item)
                                    OnComError(New Common.PlcComEventArgs(-21, "No Response from PLC(21)", CUShort(item.MID), item.OwnerObjectID))
                                End If
                            Else
                                '* Successful send, remove from queue
                                SendRetries = 0
                                'SyncLock (QueueLockObject)
                                SendQue.TryDequeue(item)
                                'End SyncLock
                            End If
                        Catch ex As Common.PLCDriverException
                            If SendQue.Count > 0 Then SendQue.TryDequeue(item)
                            OnComError(New Common.PlcComEventArgs(ex.ErrorCode, ex.Message, CUShort(item.MID), item.OwnerObjectID))
                        End Try
                    Else
                        '* Data packet was nothing, so dequeue command
                        Try
                            If SendQue.Count > 0 Then SendQue.TryDequeue(item)
                        Catch ex As Exception
                        End Try
                    End If
                Else
                    '* Nothing in queue, so pass this time slice
                    QueHoldRelease.WaitOne(5000)
                    QueHoldRelease.Reset()
                End If
            Catch ex As Common.PLCDriverException
                If SendQue.Count > 0 Then SendQue.TryDequeue(item)
                OnComError(New Common.PlcComEventArgs(ex.ErrorCode, ex.Message, CUShort(item.MID), item.OwnerObjectID))
            Catch ex As Exception
                '* on error, throw out this packet
                If SendQue.Count > 0 Then SendQue.TryDequeue(item)
            End Try
        End While

        ThreadStarted = False
        StopThread = False
    End Sub

#End Region

#Region "Events"
    Protected Overridable Sub OnDataReceived(ByVal e As Common.PlcComEventArgs)
        Try
            RaiseEvent DataReceived(Me, e)
        Catch ex As Exception
            'Dim dbg = 0
        End Try
    End Sub

    Protected Overridable Sub OnConnectionClosed(ByVal e As EventArgs)
        RaiseEvent ConnectionClosed(Me, e)
    End Sub

    Protected Overridable Sub OnComError(ByVal e As Common.PlcComEventArgs)
        RaiseEvent ComError(Me, e)
    End Sub
#End Region
End Class
'End Namespace