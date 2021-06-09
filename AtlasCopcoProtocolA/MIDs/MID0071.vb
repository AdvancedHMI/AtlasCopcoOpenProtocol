'************************************************************
'* MID 0071 - 5.10.2 Alarm Upload reply
'*
'* Error Code - 4 bytes
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
'************************************************************
Public Class MID0071
#Region "Poperties"
    Public Property ErrorCode As Integer
    Public Property ControllerReadyStatus As Boolean
    Public Property ToolReadyStatus As Boolean
    Public Property TimeStamp As Date


#End Region

#Region "Constructor"
    Public Sub New()
    End Sub

    Public Sub New(data() As Byte)
        Me.New()
        Parse(data)
    End Sub
#End Region


#Region "Methods"
    Private Sub Parse(ByVal data() As Byte)
        If data IsNot Nothing Then
            If data.Length >= 4 Then
                Dim index As Integer = 0
                Dim ErrorIDAsString = System.Text.Encoding.ASCII.GetString(data, index, 4)
                Integer.TryParse(ErrorIDAsString, ErrorCode)
                index += 4

                If data.Length > index + 1 Then
                    Dim Status As Integer
                    Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index, 1), Status)
                    ControllerReadyStatus = (Status = 1)
                    index += 1

                    If data.Length > index + 1 Then
                        Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index, 1), Status)
                        ToolReadyStatus = (Status = 1)
                        index += 1

                        If data.Length > index + 19 Then
                            Dim TimeStampAsString As String = System.Text.ASCIIEncoding.ASCII.GetString(data, index, 19).Trim
                            Date.TryParse(TimeStampAsString, TimeStamp)
                            index += 19
                        End If
                    End If
                End If
            End If
        End If
    End Sub
#End Region
End Class
