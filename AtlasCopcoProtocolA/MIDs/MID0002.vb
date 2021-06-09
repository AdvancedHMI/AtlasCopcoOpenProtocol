'************************************************************
'* MID 0002 - 5.3.2 Communication start acknowledge 
'* Sent By: Torque Controller
'*
'* Cell ID      - 4 bytes  (range 0000 - 9999)
'* Channel ID   - 2 bytes  (range 00 - 20)
'* Controller Name - 25 bytes
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
Public Class MID0002
    Public Property CellId As Integer
    Public Property ChannelID As Integer
    Public Property ControllerName As String

    '* Revision 2 only
    Public Property SupplierCode As String

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
            If data.Length >= 6 Then
                Dim CellIdAsString = System.Text.Encoding.ASCII.GetString(data, 2, 4)
                Integer.TryParse(CellIdAsString, CellId)

                If data.Length >= 10 Then
                    Dim ChannelIdAsString = System.Text.Encoding.ASCII.GetString(data, 8, 2)
                    Integer.TryParse(ChannelIdAsString, ChannelID)

                    Dim TrimChars() As Char = {Chr(0), " "c}
                    If data.Length > 35 Then
                        ControllerName = System.Text.Encoding.ASCII.GetString(data, 12, data.Length - 6).TrimEnd(TrimChars)

                        If data.Length >= 42 Then
                            SupplierCode = System.Text.Encoding.ASCII.GetString(data, 39, 3).TrimEnd(TrimChars)
                        End If
                    End If
                End If
            End If
        End If
    End Sub
#End Region
End Class
