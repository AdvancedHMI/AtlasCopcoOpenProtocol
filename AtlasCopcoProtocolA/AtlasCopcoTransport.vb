Option Strict On
Imports System.Net.Sockets
'**********************************************************************************************
'* Atlas Copco Transport
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* 04-MAR-21
'*
'* These classes implements the Atlas Copco transport layer
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
Friend Class AtlasCopcoTransport
    Implements IDisposable
    Private TCPSocket As System.Net.Sockets.Socket
    Private BroadcastSocket As System.Net.Sockets.Socket

    Public Event DataReceived As EventHandler(Of Common.TCPComEventArgs)
    Public Event ConnectionClosed As EventHandler
    Public Event ConnectionEstablished As EventHandler
    Public Event ComError As EventHandler(Of Common.PlcComEventArgs)

    Private DataReceivedCallBackDelegate As AsyncCallback

#Region "Constructor/Destructors"
    Public Sub New()
        DataReceivedCallBackDelegate = New AsyncCallback(AddressOf DataReceivedCallback)
        m_ProtocolType = Net.Sockets.ProtocolType.Tcp
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            CloseConnection()
        End If
    End Sub
#End Region

#Region "Properties"
    Private m_IPAddress As String = "192.168.0.1"   '* this is a default value
    Public Property IPAddress() As String
        Get
            Return m_IPAddress.ToString
        End Get
        Set(ByVal value As String)
            Dim address As New System.Net.IPAddress(0)
            If System.Net.IPAddress.TryParse(m_IPAddress, address) AndAlso System.Net.IPAddress.TryParse(value, address) Then
                If Not System.Net.IPAddress.Parse(m_IPAddress).Equals(System.Net.IPAddress.Parse(value)) Then
                    '* if the address is changed, be sure to disconnect first
                    If TCPSocket IsNot Nothing AndAlso TCPSocket.Connected Then
                        CloseConnection()
                    End If
                End If
            Else
                If m_IPAddress <> value Then
                    '* if the address is changed, be sure to disconnect first
                    If TCPSocket IsNot Nothing AndAlso TCPSocket.Connected Then
                        CloseConnection()
                    End If
                End If
            End If
            m_IPAddress = value

        End Set
    End Property

    Private m_Port As UShort = &HAF12
    Friend Property Port As UShort
        Get
            Return m_Port
        End Get
        Set(value As UShort)
            If m_Port <> value Then
                If TCPSocket IsNot Nothing AndAlso TCPSocket.Connected Then
                    CloseConnection()
                End If
                m_Port = value
            End If
        End Set
    End Property

    Private Property m_ProtocolType As ProtocolType = ProtocolType.Tcp
    Public Property ProtocolType As ProtocolType
        Get
            Return m_ProtocolType
        End Get
        Set(value As ProtocolType)
            m_ProtocolType = value
        End Set
    End Property


    Private m_HeaderSize As Integer = 24
    Public Property HeaderSize As Integer
        Get
            Return m_HeaderSize
        End Get
        Set(value As Integer)
            m_HeaderSize = value
        End Set
    End Property
#End Region

#Region "Private Methods"
    '*********************************************
    '* Connect to the socket and begin listening
    '* for responses
    '********************************************
    Private Sub Connect(ByVal timeout As Integer)
        Dim EndPoint As System.Net.IPEndPoint = CreateEndPoint(m_IPAddress)

        If TCPSocket Is Nothing OrElse Not TCPSocket.Connected Then
            TCPSocket = CreateSocket(EndPoint, m_ProtocolType)
        End If

        Dim iar As IAsyncResult
        Try
            TCPSocket.Blocking = False
            'TCPSocket.NoDelay = True

            '* Use a begin connect so we can control the wait time for connection
            iar = TCPSocket.BeginConnect(EndPoint, New AsyncCallback(AddressOf ConnectCallback), Nothing)

            Dim WaitResult As Boolean = iar.AsyncWaitHandle.WaitOne(timeout)

            If WaitResult And TCPSocket.Connected Then
                OnConnectionEstablished(System.EventArgs.Empty)
            Else
                CloseConnection()
                Throw New Common.PLCDriverException(-34, "Could not connect to " & m_IPAddress & ", port " & m_Port & ". Timed out")
            End If
        Catch ex As SocketException
            ' 10035 == WSAEWOULDBLOCK
            If ex.NativeErrorCode.Equals(10035) Then
                'Throw
            Else
                Throw New Common.PLCDriverException(m_IPAddress & " " & ex.Message)
            End If
            'Finally
            '    TCPSocket.Blocking = blockingState
        End Try


        TCPSocket.Blocking = True
        '*Version 3.99b
        If m_ProtocolType = Net.Sockets.ProtocolType.Tcp Then
            TCPSocket.LingerState = New System.Net.Sockets.LingerOption(True, 1000)
        End If

        'If TCPSocket.Poll(2000, SelectMode.SelectWrite) = False Then
        '    Throw New Common.PLCDriverException(-35, "Could not connect to " & m_IPAddress & ". ")
        'End If

        '* Don't buffer the data, so it goes out immediately
        '* Otherwise packets send really fast will get grouped
        '* And the PLC will not respond to all of them
        TCPSocket.SendBufferSize = 1

        'Dim so As New Common.TcpStateObject
        'so.WorkSocket = TCPSocket
        'TCPSocket.BeginReceive(ReceiveBuffer, 0, 1024, System.Net.Sockets.SocketFlags.None,
        '                       New AsyncCallback(AddressOf DataReceivedCallback), so)
    End Sub

    Private Function CreateEndPoint(ByVal iPAddress As String) As System.Net.IPEndPoint
        Dim EndPoint As System.Net.IPEndPoint
        Dim IP As System.Net.IPHostEntry

        Dim address As New System.Net.IPAddress(0)
        If System.Net.IPAddress.TryParse(iPAddress, address) Then
            EndPoint = New System.Net.IPEndPoint(address, m_Port)
        Else
            Try
                IP = System.Net.Dns.GetHostEntry(iPAddress)
                '* Ethernet/IP uses port AF12 (44818)
                EndPoint = New System.Net.IPEndPoint(IP.AddressList(0), m_Port)
            Catch ex As Exception
                Throw New Common.PLCDriverException("Can't resolve the address " & iPAddress)
            End Try
        End If

        Return EndPoint
    End Function

    Private Function CreateSocket(ByVal endPoint As System.Net.IPEndPoint, ByVal protocol As Net.Sockets.ProtocolType) As Socket
        Dim s As Socket

        If protocol = Net.Sockets.ProtocolType.Tcp Then
            s = New System.Net.Sockets.Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            '* Comment these out for Compact Framework
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 5000)
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, True)
        Else
            s = New System.Net.Sockets.Socket(endPoint.AddressFamily, SocketType.Dgram, protocol)
            's.DontFragment = True
        End If

        s.SendTimeout = 2000
        s.ReceiveBufferSize = &H5000

        Return s
    End Function

    Private Sub ConnectCallback(ByVal ar As IAsyncResult)
        Try
            If TCPSocket IsNot Nothing AndAlso TCPSocket.Connected Then
                TCPSocket.EndConnect(ar)
                ' OnConnectionEstablished(System.EventArgs.Empty)

                Dim StateObject As New Common.TcpStateObject(TCPSocket)
                ' StateObject.OwnerObjectID = ownerObjectID  ' packet.OwnerObjectID
                'StateObject.TransactionNumber = transactionNumber  ' packet.TransactionNumber
                '* Start listening for data received

                TCPSocket.BeginReceive(StateObject.data, 0, StateObject.data.Length, SocketFlags.None, DataReceivedCallBackDelegate, StateObject)
            End If
        Catch ex As Exception
            'Dim dbg = 0
        End Try
    End Sub


    '************************************************************
    '* Call back procedure - called when data comes back
    '* This is the procedure pointed to by the BeginWrite method
    '************************************************************
    Private Sub DataReceivedCallback(ByVal ar As System.IAsyncResult)
        ' Retrieve the state object and the client socket 
        ' from the asynchronous state object.
        Dim StateObject As Common.TcpStateObject = CType(ar.AsyncState, Common.TcpStateObject)

        'Console.WriteLine("DateReceived TNS=" & StateObject.TransactionNumber)

        '* If the socket was closed, then we cannot do anything
        If StateObject.WorkSocket.ProtocolType = ProtocolType.Tcp AndAlso Not StateObject.WorkSocket.Connected Then
            Exit Sub
        End If


        '* Get the number of bytes read and add it to the state object accumulator
        Try
            '* Add the byte count to the state object
            StateObject.CurrentIndex += StateObject.WorkSocket.EndReceive(ar)
        Catch ex As Exception
            '* Return an error code
            OnComError(New Common.PlcComEventArgs(-1, "Socket Error : " & ex.Message))
            Exit Sub
        End Try


        Try
            ' Full packet length not received yet, so setup to receive more
            Dim DataLength As UInteger
            If (StateObject.CurrentIndex >= m_HeaderSize) Then
                Try
                    DataLength = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(StateObject.data, 0, 4)) - Convert.ToUInt32(m_HeaderSize)
                Catch ex As Exception
                End Try
            End If
            'Console.WriteLine("Socket available=" & StateObject.WorkSocket.Available)


            If (StateObject.CurrentIndex >= (DataLength + m_HeaderSize)) Then
                '* Let the parent know that data was rcvd
                Try
                    OnDataReceived(New Common.TCPComEventArgs(StateObject))
                Catch ex As Exception
                    OnComError(New Common.PlcComEventArgs(-3, ex.Message & "- edr(5)"))
                End Try

                '* Start a new packet
                'If StateObject.OwnerObjectID = 0 Then
                '* Listen for unsolicited data
                Dim so = New Common.TcpStateObject
                so.TransactionNumber = 0
                so.OwnerObjectID = 0
                so.WorkSocket = StateObject.WorkSocket
                StateObject.WorkSocket.BeginReceive(so.data, 0, so.data.Length,
                        System.Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf DataReceivedCallback), so)
                'End If
            Else
                '* Add received data when the full packet not received
                StateObject.WorkSocket.BeginReceive(StateObject.data, StateObject.CurrentIndex, StateObject.data.Length,
                                                    System.Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf DataReceivedCallback), StateObject)
            End If
        Catch ex As Exception
            OnComError(New Common.PlcComEventArgs(-2, ex.Message & "- edr(2)"))
        End Try
    End Sub
#End Region

#Region "Public Methods"
    Public Sub CloseConnection()
        Try
            If TCPSocket IsNot Nothing Then
                If TCPSocket.Connected Then
                    Try
                        TCPSocket.Shutdown(System.Net.Sockets.SocketShutdown.Send)
                    Catch ex As Exception
                    End Try
                    TCPSocket.Close()
                    OnConnectionClosed(System.EventArgs.Empty)
                End If
                If TCPSocket IsNot Nothing Then
                    TCPSocket.Dispose()
                    TCPSocket = Nothing
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    '*********************************
    '* Send data out the tcp socket
    '*********************************
    '* Ver 3.99b - changed return result to Bool - for future compatibility with UWP
    Public Function SendData(ByVal packet As IStreamable, ByVal listenForResponse As Boolean, timeOut As Integer) As Boolean ' System.IAsyncResult
        If packet IsNot Nothing Then
            Dim data() As Byte = packet.GetBytes
            Return SendData(data, listenForResponse, timeOut, packet.OwnerObjectID, 0) ' packet.TransactionNumber)
        Else
            Return False
        End If
    End Function

    Public Function SendData(ByVal data() As Byte, ByVal listenForResponse As Boolean, timeOut As Integer,
                             ByVal ownerObjectID As Long, ByVal transactionNumber As Integer) As Boolean ' System.IAsyncResult
        '* connect if it has not been already
        If data IsNot Nothing Then
            Dim ia As System.IAsyncResult = Nothing

            If TCPSocket Is Nothing OrElse Not TCPSocket.Connected Then
                '* V3.99y - added catch to exit sub
                Try
                    Connect(timeOut)
                Catch ex As Exception
                    Throw ex
                    Return False
                End Try
            End If

            'Try
            '    'If listenForResponse Then
            '    '* Create a new state object to contain the data received
            '    'Dim StateObject As New Common.TcpStateObject(TCPSocket)
            '    'StateObject.OwnerObjectID = ownerObjectID  ' packet.OwnerObjectID
            '    'StateObject.TransactionNumber = transactionNumber  ' packet.TransactionNumber
            '    ''* Start listening for data received

            '    'ia = TCPSocket.BeginReceive(StateObject.data, 0, StateObject.data.Length, SocketFlags.None, DataReceivedCallBackDelegate, StateObject)
            '    'End If
            'Catch ex As Exception
            '    'Console.WriteLine("EIPTransport SendData exception=" & ex.Message)
            '    Dim e As New Common.PlcComEventArgs(-80, ex.Message)
            '    OnComError(e)
            '    CloseConnection()
            '    '* V3.99y
            '    Return False
            'End Try

            'Dim data() As Byte = packet.GetBytes
            Try
                TCPSocket.Send(data, data.Length, SocketFlags.None)

                'If listenForResponse Then
                '    ia.AsyncWaitHandle.WaitOne(timeOut, True)

                '    If Not ia.IsCompleted Then
                '        CloseConnection()
                '        Throw New Common.PLCDriverException(-1, "EIPTransport.SendData-No Response")
                '    End If
                'End If
                'ia.AsyncState
            Catch ex As Exception
                Dim e As New Common.PlcComEventArgs(-80, ex.Message)
                OnComError(e)
                CloseConnection()
                Throw ex
            End Try


            'Return ia
            Return True
        End If

        Return False
    End Function

#End Region

#Region "Events"
    Protected Overridable Sub OnDataReceived(ByVal e As Common.TCPComEventArgs)
        RaiseEvent DataReceived(Me, e)
    End Sub

    Protected Overridable Sub OnComError(ByVal e As Common.PlcComEventArgs)
        RaiseEvent ComError(Me, e)
    End Sub

    Protected Overridable Sub OnConnectionClosed(ByVal e As EventArgs)
        RaiseEvent ConnectionClosed(Me, e)
    End Sub

    Protected Overridable Sub OnConnectionEstablished(ByVal e As EventArgs)
        RaiseEvent ConnectionEstablished(Me, e)
    End Sub
#End Region
End Class
