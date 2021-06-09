Option Strict On
'***********************************************************************************
'* Atlas Copco Open Protocol Commands
'*
'* 28-DEC-20
'*
'* Copyright (c) 2021 Manufacturing Automation, LLC
'*
'*  This library Is free software; you can redistribute it And/Or
'*  modify it under the terms Of the GNU Lesser General Public
'*  License as published by the Free Software Foundation; either
'*  version 2.1 Of the License, Or (at your option) any later version.
'*  See the file COPYING included With this distribution For more
'*  information.
'***********************************************************************************
Public Class AtlasCopco
#Region "Fields"
    Private Shared DLL As System.Collections.Concurrent.ConcurrentDictionary(Of Integer, AtlasCopcoDataLinkLayer)

    Public Event SubscriptionDataReceived As EventHandler(Of Common.SubscriptionEventArgs)
    Public Event DataReceived As EventHandler(Of Common.PlcComEventArgs)
    Public Event ComError As EventHandler(Of Common.PlcComEventArgs)
    Public Event ConnectionEstablished As EventHandler
    Public Event ConnectionClosed As EventHandler

    Public Event PsetChangeReceived As EventHandler(Of MID0015)
    Public Event TighteningResultReceived As EventHandler(Of MID0061)
    Public Event AlarmReceived As EventHandler(Of MID0071)

    Private Shared ObjectIDs As Int64
    Protected MyObjectID As Int64
    Protected MyDLLInstance As Integer
    Private DLLListLock As New Object
    Protected Shared NextDLLInstance As Integer
    Protected Friend EventHandlerDLLInstance As Integer

    Private Requests(255) As AtlasCopcoAddress
    Private Responses(255) As Common.PlcComEventArgs
    Protected MIDWaitHandle(255) As System.Threading.EventWaitHandle
    Private SubscriptionHoldRelease As New System.Threading.EventWaitHandle(False, Threading.EventResetMode.AutoReset)

    'Private MIDResponseWaitHandles(99) As System.Threading.EventWaitHandle

    Private KeepAliveTimer As Timers.Timer
    Private StartResponse As MID0002
#End Region

#Region "Constructor"
    Public Sub New()
        ObjectIDs += 1
        MyObjectID = ObjectIDs

        For index = 0 To 255
            MIDWaitHandle(index) = New System.Threading.EventWaitHandle(False, System.Threading.EventResetMode.AutoReset)
        Next

        'For index = 0 To MIDResponseWaitHandles.Length - 1
        '    MIDResponseWaitHandles(index) = New System.Threading.EventWaitHandle(False, System.Threading.EventResetMode.AutoReset)
        'Next


        DLL = New System.Collections.Concurrent.ConcurrentDictionary(Of Integer, AtlasCopcoDataLinkLayer)

        KeepAliveTimer = New Timers.Timer(8000)
        AddHandler KeepAliveTimer.Elapsed, AddressOf KeepAliveElapsed
    End Sub
#End Region

#Region "Properties"
    Private m_IPAddress As String = "192.168.1.33"   '* this is a default value
    <System.ComponentModel.Category("Communication Settings")>
    Public Overridable Property IPAddress() As String
        Get
            Return m_IPAddress.ToString
        End Get
        Set(ByVal value As String)
            If m_IPAddress <> value Then
                '* If this been attached to a DLL, then remove first
                If EventHandlerDLLInstance = (MyDLLInstance + 1) Then
                    RemoveDLLConnection(MyDLLInstance)
                End If

                m_IPAddress = value


                'If Not Me.DesignMode Then
                '    '* If a new instance needs to be created
                '    CreateDLLInstance()
                'End If
            End If
        End Set
    End Property

    Private m_TcpipPort As UInt16 = 502
    <System.ComponentModel.Category("Communication Settings")>
    Public Property TcpipPort() As UInt16
        Get
            Return m_TcpipPort
        End Get
        Set(ByVal value As UInt16)
            If m_TcpipPort <> value Then
                '* If this been attached to a DLL, then remove first
                If EventHandlerDLLInstance = (MyDLLInstance + 1) Then
                    RemoveDLLConnection(MyDLLInstance)
                End If

                m_TcpipPort = value
                'If Not Me.DesignMode Then
                '    '* If a new instance needs to be created, such as a different AMS Address
                '    CreateDLLInstance()
                'End If
            End If
        End Set
    End Property
#End Region

#Region "Public Methods"
    '***************************************************************************************
    '* 5.3.1 Communication start (MID = 0001)
    '***************************************************************************************
    Public Function CommunicationStart() As Boolean
        Dim MID As Integer = 1
        Dim Pckt As New MessageFrame(MID)

        SendPacket(Pckt)

        '* Response to MID001 is MOD002
        If MIDWaitHandle(MID).WaitOne(3000) Then
            If Responses(MID) IsNot Nothing AndAlso Responses(MID).ErrorId = 0 Then
                StartResponse = New MID0002(DirectCast(Responses(MID).ResponseFrame, MessageFrame).Data.ToArray)
                Return True
            End If
        Else
            If Responses(MID) IsNot Nothing Then
                Throw New Exception("No Response to Com Start : " & Responses(MID).ErrorId & " , " & Responses(MID).ErrorMessage)
            End If
        End If

            Return False
    End Function

    '***************************************************************************************
    '* 5.5.1 Parameter set number upload request (MID = 0010)
    '***************************************************************************************
    Public Function ParameterSetRequest() As MID0011
        Dim MID As Integer = 10
        Dim Pckt As New MessageFrame(MID)

        SendPacket(Pckt)

        If MIDWaitHandle(MID).WaitOne(3000) Then
            If Responses(MID) IsNot Nothing AndAlso Responses(MID).ErrorId = 0 Then
                Dim PSets = New MID0011(DirectCast(Responses(MID).ResponseFrame, MessageFrame).Data.ToArray)
                Return PSets
            Else
                Throw New Exception("Error : " & Responses(MID).ErrorId & " , " & Responses(MID).ErrorMessage)
            End If
        Else
            Throw New Exception("Timeout Waiting for Parameter Set")
        End If
    End Function

    '***************************************************************************************
    '* 5.5.5 Parameter set “selected” subscribe (MID = 0014)
    '***************************************************************************************
    Public Function ParameterSelectedSubscribe() As Boolean
        Dim MID As Integer = 14
        Dim Pckt As New MessageFrame(MID)

        SendPacket(Pckt)

        If MIDWaitHandle(MID).WaitOne(3000) Then
            If Responses(MID).ErrorId = 0 Then
                Return True
            Else
                Throw New Exception("Error : " & Responses(MID).ErrorId & " , " & Responses(MID).ErrorMessage)
            End If
        Else
            Throw New Exception("Timeout Waiting to Last Result Subscribe")
        End If
    End Function

    '**************************************************************************************
    '* 5.5.9 Select Parameter set (MID = 0018)
    '**************************************************************************************
    Private MID0018Result As Boolean
    Public Function SelectParameterSet(ByVal pSet As Integer) As Boolean
        Dim MID As Integer = 18
        Dim Pckt As New MessageFrame(MID)
        Dim PsetAsASCII As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(pSet.ToString("000"))
        For i = 0 To PsetAsASCII.Length - 1
            Pckt.Data.Add(PsetAsASCII(i))
        Next

        SendPacket(Pckt)

        If MIDWaitHandle(5).WaitOne(3000) Then
            If Responses(MID) IsNot Nothing AndAlso Responses(MID).ErrorId = 0 Then
                Dim OKResult As New MID0005(DirectCast(Responses(MID).ResponseFrame, MessageFrame).Data.ToArray)
                If OKResult.MID = 18 Then
                    Return True
                Else
                    Return False
                End If
            Else
                Throw New Exception("Error : " & Responses(MID).ErrorId & " , " & Responses(MID).ErrorMessage)
            End If
        Else
            Throw New Exception("Timeout Waiting to Select Parameter Set")
        End If
    End Function

    '***************************************************************************************
    '* 5.5.3 Parameter set data upload request  (MID = 0012)
    '***************************************************************************************
    Public Function ParameterSetDataRequest(ByVal Pset As Integer) As MID0013
        Dim MID As Integer = 12
        Dim Pckt As New MessageFrame(MID)
        Dim PsetAsString As String = Pset.ToString("000")
        Dim PsetAsASCII As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(PsetAsString)
        For i = 0 To PsetAsASCII.Length - 1
            Pckt.Data.Add(PsetAsASCII(i))
        Next

        SendPacket(Pckt)

        If MIDWaitHandle(MID).WaitOne(3000) Then
            If Responses(MID).ErrorId = 0 Then
                Dim PsetData As New MID0013(DirectCast(Responses(MID).ResponseFrame, MessageFrame).Data.ToArray)
                Return PsetData
            Else
                Throw New Exception("Error : " & Responses(MID).ErrorId & " , " & Responses(MID).ErrorMessage)
            End If
        Else
            Throw New Exception("Timeout Waiting to Parameter Set Data Request")
        End If
    End Function

    '**************************************************************************************
    '* 5.9.1 Last tightening result data subscribe (MID = 0060)
    '**************************************************************************************
    Public Function LastResultSubscribe() As Boolean
        Dim MID As Integer = 60
        Dim Pckt As New MessageFrame(MID)

        SendPacket(Pckt)

        If MIDWaitHandle(MID).WaitOne(3000) Then
            If Responses(MID).ErrorId = 0 Then
                Return True
            Else
                Throw New Exception("Error : " & Responses(MID).ErrorId & " , " & Responses(MID).ErrorMessage)
            End If
        Else
            Throw New Exception("Timeout Waiting to Last Result Subscribe")
        End If
    End Function

    '**************************************************************************************
    '* 5.9.1 Last tightening result unsubscribe (MID = 0063)
    '**************************************************************************************
    Public Function LastResultUnSubscribe() As Boolean
        Dim MID As Integer = 63
        Dim Pckt As New MessageFrame(MID)

        SendPacket(Pckt)

        If MIDWaitHandle(MID).WaitOne(3000) Then
            If Responses(MID).ErrorId = 0 Then
                Return True
            Else
                Throw New Exception("Error : " & Responses(MID).ErrorId & " , " & Responses(MID).ErrorMessage)
            End If
        Else
            Throw New Exception("Timeout Waiting to Last Result Subscribe")
        End If
    End Function



    '**************************************************************************************
    '* 5.9.5 Old tightening result upload request (MID = 0064) 
    '**************************************************************************************
    Public Function TighteningResultUpload(ByVal TighteningID As Integer) As MID0065
        Dim MID As Integer = 64
        Dim Pckt As New MessageFrame(MID)
        Dim TighteningIDAsString As String = TighteningID.ToString("0000000000")
        Dim TighteningIDAsASCII As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(TighteningIDAsString)
        For i = 0 To TighteningIDAsASCII.Length - 1
            Pckt.Data.Add(TighteningIDAsASCII(i))
        Next

        SendPacket(Pckt)

        If MIDWaitHandle(MID).WaitOne(3000) Then
            If Responses(MID).ErrorId = 0 Then
                Dim TighteningResult As New MID0065(DirectCast(Responses(MID).ResponseFrame, MessageFrame).Data.ToArray)
                Return TighteningResult
            Else
                Throw New Exception("Error : " & Responses(MID).ErrorId & " , " & Responses(MID).ErrorMessage)
            End If
        Else
            Throw New Exception("Timeout Waiting for Tightening Result Upload")
        End If
    End Function

    '**************************************************************************************
    '* 5.10.1 Alarm subscribe (MID = 0070)
    '**************************************************************************************
    Public Function AlarmSubscribe() As Boolean
        Dim MID As Integer = 70
        Dim Pckt As New MessageFrame(MID)

        SendPacket(Pckt)

        If MIDWaitHandle(MID).WaitOne(3000) Then
            If Responses(MID).ErrorId = 0 Then
                Return True
            Else
                Throw New Exception("Error : " & Responses(MID).ErrorId & " , " & Responses(MID).ErrorMessage)
            End If
        Else
            Throw New Exception("Timeout Waiting to Alarm Subscribe")
        End If
    End Function




    '************************************************************************************************
    '************************************************************************************************
    Private Function SendPacket(p As MessageFrame) As Boolean
        If p IsNot Nothing Then
            If EventHandlerDLLInstance <= 0 Then
                CreateDLLInstance()
            End If

            p.OwnerObjectID = MyObjectID

            Dim TransactionID As Integer
            TransactionID = Common.TransactionNumberGenerator.GetNextTNSNumber(32767)
            'p.TransactionNumber = TransactionID

            Dim TransactionByte As Integer = (TransactionID And 255)
            Requests(TransactionByte) = New AtlasCopcoAddress(p.MID)
            Responses(TransactionByte) = Nothing
            MIDWaitHandle(TransactionByte).Reset()

            KeepAliveTimer.Stop()
            DLL(MyDLLInstance).SendData(p)
            KeepAliveTimer.Start()

            Return True
        Else
            Return False
        End If
    End Function
#End Region

    '***************************************************************
    '* Create the Data Link Layer Instances
    '* if the IP Address is the same, then resuse a common instance
    '***************************************************************
    Private CreateDLLLock As New Object
    Protected Sub CreateDLLInstance()
        '* Still default, so ignore
        If m_IPAddress = "0.0.0.0" Then Exit Sub

        SyncLock (CreateDLLLock)
            '* Check to see if it has the same IP address and Port
            '* if so, reuse the instance, otherwise create a new one
            Dim KeyFound As Boolean
            For Each d In DLL
                If d.Value IsNot Nothing Then
                    If (d.Value.IPAddress = m_IPAddress And d.Value.Port = m_TcpipPort) Then
                        MyDLLInstance = d.Key
                        KeyFound = True
                        Exit For
                    End If
                End If
            Next

            '* A DLL instance for this IP does not exist
            If Not KeyFound Then
                NextDLLInstance += 1
                MyDLLInstance = NextDLLInstance
            End If

            '* Do we need to create a new DLL instance?
            If (Not DLL.ContainsKey(MyDLLInstance) OrElse (DLL(MyDLLInstance) Is Nothing)) Then
                Dim NewDLL As New AtlasCopcoDataLinkLayer(m_IPAddress, m_TcpipPort)
                NewDLL.Port = 4545
                'NewDLL.Timeout = Me.Timeout
                DLL(MyDLLInstance) = NewDLL
            End If


            '* Have we already attached event handler to this data link layer?
            If EventHandlerDLLInstance <> (MyDLLInstance + 1) Then
                '* If event handler to another layer has been created, remove them
                If EventHandlerDLLInstance > 0 Then
                    If DLL.ContainsKey(EventHandlerDLLInstance - 1) Then
                        RemoveDLLConnection(EventHandlerDLLInstance - 1)
                    End If
                End If

                AddHandler DLL(MyDLLInstance).DataReceived, AddressOf DataLinkLayerDataReceived
                AddHandler DLL(MyDLLInstance).ComError, AddressOf DataLinkLayerComError
                AddHandler DLL(MyDLLInstance).ConnectionClosed, AddressOf DLLConnectionClosed
                AddHandler DLL(MyDLLInstance).ConnectionEstablished, AddressOf DataLinkLayerConnectionEstablished
                '* Track how many instanced use this DLL, so we know when to dispose
                DLL(MyDLLInstance).ConnectionCount += 1
                EventHandlerDLLInstance = MyDLLInstance + 1
            End If
            'End If
        End SyncLock
    End Sub

    Protected Sub RemoveDLLConnection(ByVal instance As Integer)
        '* The handle linked to the DataLink Layer has to be removed, otherwise it causes a problem when a form is closed
        If DLL.ContainsKey(instance) AndAlso DLL(instance) IsNot Nothing Then
            RemoveHandler DLL(MyDLLInstance).DataReceived, AddressOf DataLinkLayerDataReceived
            RemoveHandler DLL(instance).ComError, AddressOf DataLinkLayerComError
            RemoveHandler DLL(MyDLLInstance).ConnectionClosed, AddressOf DLLConnectionClosed
            RemoveHandler DLL(MyDLLInstance).ConnectionEstablished, AddressOf DataLinkLayerConnectionEstablished
            EventHandlerDLLInstance = 0

            DLL(MyDLLInstance).ConnectionCount -= 1

            If DLL(instance).ConnectionCount <= 0 Then
                DLL(instance).Dispose()
                DLL(instance) = Nothing
                Dim x As AtlasCopcoDataLinkLayer = Nothing
                DLL.TryRemove(instance, x)
            End If
        End If
    End Sub

#Region "Events"
    '*******************************************************************
    '* 5.18.1 Keep alive message (MID = 9999)
    '*******************************************************************
    Private Sub KeepAliveElapsed(ByVal sender As Object, ByVal e As Timers.ElapsedEventArgs)
        '* MID 9999 - Keep Alive
        Dim Pckt As New MessageFrame(9999)
        SendPacket(Pckt)
    End Sub

    '************************************************
    '* Process data recieved from controller
    '************************************************
    Private Sub DataLinkLayerDataReceived(ByVal sender As Object, ByVal e As Common.PlcComEventArgs)
        Dim msg As New MessageFrame(New List(Of Byte)(e.RawData).ToArray, e.RawData.Length)

        ProcessDataReceived(msg, e)
    End Sub

    Private Sub DLLConnectionClosed(ByVal sender As Object, ByVal e As System.EventArgs)
        KeepAliveTimer.Stop()
        StartResponse = Nothing
        OnConnectionClosed(e)
    End Sub


    '************************************************
    '* Process data recieved from controller
    '************************************************
    Protected Sub ProcessDataReceived(ByVal msg As MessageFrame, ByVal e As Common.PlcComEventArgs)
        '* Not enough data to make up a packet
        If e.RawData Is Nothing OrElse e.RawData.Length < 4 Then
            Exit Sub
        End If

        'Dim TIDByte As Integer = e.TransactionNumber And 255

        'If e.OwnerObjectID = 0 Or (e.OwnerObjectID = MyObjectID AndAlso Requests(msg.MID) IsNot Nothing) Then

        Select Case msg.MID
            Case 2 '* Start Comm ACK (Result from MID001)
                KeepAliveTimer.Start()
                Responses(1) = e
                Responses(1).ResponseFrame = msg

                MIDWaitHandle(1).Set()
            Case 4 ' * Error
                Dim ErrorResponse As New MID0004(msg.Data.ToArray)
                If msg.Data.Count >= 6 Then
                    Try
                        Responses(ErrorResponse.MID) = e
                        Responses(ErrorResponse.MID).ErrorId = ErrorResponse.ErrorID 'System.Text.Encoding.ASCII.GetString(msg.Data.ToArray, 4, 2) '(msg.Data(4) - 48) * 10 + (msg.Data(5) - 48)
                        Responses(ErrorResponse.MID).ErrorMessage = ErrorCodes.Errors(ErrorResponse.ErrorID)
                        OnComError(New Common.PlcComEventArgs(ErrorResponse.ErrorID, Responses(msg.MID).ErrorMessage))
                    Catch ex As Exception
                    End Try
                End If

            Case 5 '* OK
                Dim OKResult As New MID0005(msg.Data.ToArray)
                Responses(OKResult.MID) = e
                Responses(OKResult.MID).ResponseFrame = msg
                '* Release The Async Hold on the errored command
                If MIDWaitHandle(OKResult.MID) IsNot Nothing Then
                    MIDWaitHandle(OKResult.MID).Set()
                End If
            Case 11 ' Parameter Set Numbers returned (Result from MID010)
                Responses(10) = e
                Responses(10).ResponseFrame = msg
                MIDWaitHandle(10).Set()
            Case 13 ' Parameter Set Data Response (Result from MID012)
                Responses(12) = e
                Responses(12).ResponseFrame = msg
                MIDWaitHandle(12).Set()
            Case 15
                Responses(14) = e
                Responses(14).ResponseFrame = msg

                Dim PsetResult As New MID0015(msg.Data.ToArray)
                OnPsetChangeReceived(PsetResult)

                Dim MID As Integer = 16
                Dim Pckt As New MessageFrame(MID)
                SendPacket(Pckt)


                MIDWaitHandle(14).Set()
            Case 61 ' 5.9.2 Last tightening result data upload reply (MID = 0061)
                Dim Result As New MID0061(msg.Data.ToArray)
                OnTighteningResultReceived(Result)
                '* Let the controller know the data was received successfully using MID005
                Dim MID As Integer = 62
                Dim Pckt As New MessageFrame(MID)
                'Dim ByteStream As New List(Of Byte)
                'ByteStream.AddRange(System.Text.Encoding.ASCII.GetBytes(msg.MID.ToString("0000")))
                'For index = 0 To ByteStream.Count - 1
                'Pckt.Data.Add(ByteStream(index))
                'Next
                SendPacket(Pckt)
            Case 71 ' 5.10.2 Alarm Upload reply (MID = 0071)
                Dim Result As New MID0071(msg.Data.ToArray)
                OnAlarmReceived(Result)
            Case 9999
                '* No need for response acknowledgement
                'Responses(9999) = e
                'Responses(9999).ResponseFrame = msg
                'MIDWaitHandle(9999).Set()

            Case Else
        End Select



        'If e.Values.Count >= Requests(TIDByte).NumberOfElements Then
        '********************************************************************
        '* Send the information back to DataReceived events or subscriptions
        '********************************************************************
        OnDataReceived(e)
        '    End If

        'End If

        '* Let everyone know we received a reponse for this request
        'If MIDWaitHandle(msg.MID) IsNot Nothing Then
        'MIDWaitHandle(msg.MID).Set()
        'End If
    End Sub

    Protected Overridable Sub OnTighteningResultReceived(ByVal e As MID0061)
        RaiseEvent TighteningResultReceived(Me, e)
    End Sub

    Protected Overridable Sub OnPsetChangeReceived(ByVal e As MID0015)
        RaiseEvent PsetChangeReceived(Me, e)
    End Sub


    Protected Overridable Sub OnAlarmReceived(ByVal e As MID0071)
        RaiseEvent AlarmReceived(Me, e)
    End Sub


    Protected Overridable Sub OnDataReceived(ByVal e As Common.PlcComEventArgs)
        RaiseEvent DataReceived(Me, e)
    End Sub

    Protected Overridable Sub OnComError(ByVal e As Common.PlcComEventArgs)
        RaiseEvent ComError(Me, e)

        'SendToSubscriptions(e)
    End Sub

    Protected Overridable Sub OnSubscriptionDataReceived(ByVal e As Common.SubscriptionEventArgs)
        RaiseEvent SubscriptionDataReceived(Me, e)

        '* Send to subscriptions V3.99y
        e.dlgCallBack(Me, e)
    End Sub


    Protected Friend Sub DataLinkLayerConnectionEstablished(ByVal sender As Object, ByVal e As System.EventArgs)
        OnConnectionEstablished(e)
    End Sub

    Protected Overridable Sub OnConnectionEstablished(ByVal e As System.EventArgs)
        RaiseEvent ConnectionEstablished(Me, e)
    End Sub

    Protected Overridable Sub OnConnectionClosed(ByVal e As System.EventArgs)
        RaiseEvent ConnectionClosed(Me, e)
    End Sub




    Protected Friend Sub DataLinkLayerComError(ByVal sender As Object, ByVal e As Common.PlcComEventArgs)
        If e.OwnerObjectID = MyObjectID Then
            If e.TransactionNumber >= 0 Then
                Dim TIDByte As Integer = e.TransactionNumber And 255

                '* Save this for other uses
                Responses(TIDByte) = e

                '* This is kind of a patch because the response can occur too fast
                If Requests(TIDByte) Is Nothing Then
                    Using DelayHandle As New System.Threading.EventWaitHandle(False, Threading.EventResetMode.AutoReset)
                        DelayHandle.WaitOne(250)
                    End Using
                End If


                If Requests(TIDByte) IsNot Nothing Then
                    'Requests(e.TransactionNumber).ErrorReturned = True
                    Requests(TIDByte).Responded = True
                    'Requests(e.TransactionNumber And 255)(0).Responded = True
                    If MIDWaitHandle(e.TransactionNumber And 255) IsNot Nothing Then
                        MIDWaitHandle(e.TransactionNumber And 255).Set()
                    End If
                End If

                OnComError(e)

                'SendToSubscriptions(e)
            End If
        End If
    End Sub

#End Region



End Class
