Option Strict On
Option Explicit On
'*
'* Copyright (c) 2021 Manufacturing Automation, LLC
'*
'*  This library Is free software; you can redistribute it And/Or
'*  modify it under the terms Of the GNU Lesser General Public
'*  License as published by the Free Software Foundation; either
'*  version 2.1 Of the License, Or (at your option) any later version.
'*  See the file COPYING included With this distribution For more
'*  information.

Public Class AtlasCopcoAddress
    Inherits BasePLCAddress

    '**************************************************************
    '* These are used for synchronous communication to indicate when
    '* a response was received
    '**************************************************************
    Public Property Responded() As Boolean
    Public Property IsWrite() As Boolean

    Public Property MID As Integer


#Region "Constructor"
    Public Sub New()
    End Sub

    Public Sub New(ByVal mid As Integer)
        Me.New()
        Me.MID = mid
    End Sub
#End Region
    Protected Overrides Sub ParseAddress()
        Throw New NotImplementedException()
    End Sub
End Class
