Option Strict On
Imports System.Net.Sockets
'******************************************************************************
'* TCP Communication State Object
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* 13-OCT-11
'*
'*
'* This class hold data returned from TCP socket communications
'* It's purpose is to accumulate data when all data is not recieved
'*  from a single DataReceived event
'*
'* Copyright (c) 2021 Manufacturing Automation, LLC
'*
'*  This library Is free software; you can redistribute it And/Or
'*  modify it under the terms Of the GNU Lesser General Public
'*  License as published by the Free Software Foundation; either
'*  version 2.1 Of the License, Or (at your option) any later version.
'*  See the file COPYING included With this distribution For more
'*  information.
'******************************************************************************
Namespace Common
    Public Class TcpStateObject
        Implements IDisposable

        Private disposed As Boolean

#Region "Properties"
        Private m_workSocket As Socket
        Public Property WorkSocket() As Socket
            Get
                Return m_workSocket
            End Get
            Set(ByVal value As Socket)
                m_workSocket = value
            End Set
        End Property

        '*********************************
        '* The received data byte stream
        '*********************************
        Friend data(1024) As Byte

        '**********************************
        '* Current Index within data array
        '**********************************
        Private m_CurrentIndex As Integer
        Public Property CurrentIndex() As Integer
            Get
                Return m_CurrentIndex
            End Get
            Set(ByVal value As Integer)
                If value >= data.Count Then
                    Throw New PLCDriverException("TCP State object can only hold up to 4096 bytes")
                    Exit Property
                End If
                m_CurrentIndex = value
            End Set
        End Property

        Private m_OwnerObjectID As Int64
        Public Property OwnerObjectID As Int64
            Get
                Return m_OwnerObjectID
            End Get
            Set(value As Int64)
                m_OwnerObjectID = value
            End Set
        End Property

        Private m_TransactionNumber As Integer
        Public Property TransactionNumber As Integer
            Get
                Return m_TransactionNumber
            End Get
            Set(value As Integer)
                m_TransactionNumber = value
            End Set
        End Property

        Private m_WaitHandle As New System.Threading.EventWaitHandle(False, Threading.EventResetMode.ManualReset)
        Public Property WaitHandle As System.Threading.EventWaitHandle
            Get
                Return m_WaitHandle
            End Get
            Set(value As System.Threading.EventWaitHandle)
                m_WaitHandle = value
            End Set
        End Property
#End Region

#Region "Constructors"
        Public Sub New()
        End Sub

        Public Sub New(s As Socket)
            m_workSocket = s
        End Sub

        Protected Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If disposed Then Return

            If disposing Then
                If m_WaitHandle IsNot Nothing Then
                    m_WaitHandle.Dispose()
                End If
            End If

            ' Free any unmanaged objects here.
            '
            disposed = True
        End Sub


#End Region

    End Class
End Namespace

