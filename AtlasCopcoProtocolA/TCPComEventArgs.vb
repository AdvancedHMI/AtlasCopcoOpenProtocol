Option Strict On
'*************************************************************************
'* TCP Driver Event Arguments
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* 19-APR-13
'*
'*
'* Used to pass response data to events handlers
'*
'* Copyright (c) 2021 Manufacturing Automation, LLC
'*
'*  This library Is free software; you can redistribute it And/Or
'*  modify it under the terms Of the GNU Lesser General Public
'*  License as published by the Free Software Foundation; either
'*  version 2.1 Of the License, Or (at your option) any later version.
'*  See the file COPYING included With this distribution For more
'*  information.
'************************************************************************

Namespace Common
    Public Class TCPComEventArgs
        Inherits EventArgs

#Region "Constructors"
        Public Sub New()
        End Sub

        Public Sub New(so As Common.TcpStateObject)
            m_StateObject = so
        End Sub
#End Region

        '****************************************
        '* 
        '****************************************
        Private m_StateObject As Common.TcpStateObject
        Public ReadOnly Property StateObject As Common.TcpStateObject
            Get
                Return m_StateObject
            End Get
        End Property
    End Class
End Namespace