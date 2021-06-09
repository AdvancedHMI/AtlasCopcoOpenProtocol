'************************************************************
'* MID 0015 - 5.5.6 Parameter set selected
'* Sent By: Torque Controller
'*
'* Pset Number - 3 bytes  (range 000 - 999)
'* Date of Last Change - 25 bytes (ASCII Characters)
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
Public Class MID0015
#Region "Poperties"
    Public Property PsetNumber As Integer
    Public Property DateofLastPsetChange As Date

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
            '* ITEM 01
            Dim ItemSize As Integer = 3
            If data.Length >= (ItemSize + 2) Then
                Dim index As Integer = 0
                Dim PsetAsString = System.Text.Encoding.ASCII.GetString(data, index, ItemSize)
                Integer.TryParse(PsetAsString, PsetNumber)

                index += ItemSize

                ItemSize = 19
                If data.Length > (index + ItemSize) Then
                    Dim TimeStampAsString As String = System.Text.ASCIIEncoding.ASCII.GetString(data, index, ItemSize).Trim
                    TimeStampAsString = TimeStampAsString.Substring(0, 10) & " " & TimeStampAsString.Substring(11, 8)
                    Date.TryParse(TimeStampAsString, DateofLastPsetChange)
                    index += ItemSize
                End If
            End If
        End If
    End Sub
#End Region
End Class
