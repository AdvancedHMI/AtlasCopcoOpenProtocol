'************************************************************
'* MID 0004 - 5.4.1 Command error
'* Sent By: Torque Controller
'*
'* MID        - 4 bytes  (range 0000 - 9999)
'* Error ID   - 2 bytes  (range 00 - 20)
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
Public Class MID0004
    Public Property MID As Integer
    Public Property ErrorID As Integer

#Region "Constructor"
    Public Sub New()

    End Sub

    Public Sub New(data() As Byte)
        Me.New()
        Parse(data)
    End Sub
#End Region


#Region "Methods"
    Public Sub Parse(ByVal data() As Byte)
        If data IsNot Nothing Then
            If data.Length >= 4 Then
                Dim MIDAsString = System.Text.Encoding.ASCII.GetString(data, 0, 4)
                Integer.TryParse(MIDAsString, MID)

                If data.Length >= 6 Then
                    Dim ErrorIdAsString = System.Text.Encoding.ASCII.GetString(data, 4, 2)
                    Integer.TryParse(ErrorIdAsString, ErrorID)
                End If
            End If
        End If
    End Sub
#End Region
End Class
