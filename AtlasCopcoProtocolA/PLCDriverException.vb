Option Strict On
'***********************************************************
'* Com Driver Exception Class
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* 13-OCT-11
'*
'*
'* Exception class for communication drivers
'*
'* Copyright (c) 2021 Manufacturing Automation, LLC
'*
'*  This library Is free software; you can redistribute it And/Or
'*  modify it under the terms Of the GNU Lesser General Public
'*  License as published by the Free Software Foundation; either
'*  version 2.1 Of the License, Or (at your option) any later version.
'*  See the file COPYING included With this distribution For more
'*  information.
'************************************************************
Namespace Common
    '<SerializableAttribute()> _
    <Serializable>
    Public Class PLCDriverException
        Inherits Exception

        Private _ErrorCode As Integer
        Public Property ErrorCode() As Integer
            Get
                Return _ErrorCode
            End Get
            Set(ByVal value As Integer)
                _ErrorCode = value
            End Set
        End Property

        Public Sub New()
            '* Use the resource manager to satisfy code analysis CA1303
            'Me.New(New System.Resources.ResourceManager("en-US", System.Reflection.Assembly.GetExecutingAssembly()).GetString("DF1 Exception"))
        End Sub

        Public Sub New(ByVal message As String)
            Me.New(message, Nothing)
        End Sub

        Public Sub New(ByVal errorCode As Integer, ByVal message As String)
            Me.New(message, Nothing)
            _ErrorCode = errorCode
        End Sub

        'Public Sub New(ByVal innerException As Exception)
        '    Me.New(New System.Resources.ResourceManager("en-US", System.Reflection.Assembly.GetExecutingAssembly()).GetString("DF1 Exception"), innerException)
        'End Sub

        Public Sub New(ByVal message As String, ByVal innerException As Exception)
            MyBase.New(message, innerException)
        End Sub


        Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class
End Namespace