'************************************************************
'* MID 0011 - 5.5.2 Parameter set numbers upload reply 
'* Sent By: Torque Controller
'*
'* No. of Valid Pset - 3 bytes  (range 000 - 999)
'* Numbers of valid Pset's   - 3 bytes x No. of ValidPsets
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
Public Class MID0011
#Region "Poperties"
    Public ReadOnly Property PsetNumbers As List(Of Integer)
#End Region
#Region "Constructor"
    Public Sub New()
        PsetNumbers = New List(Of Integer)
    End Sub

    Public Sub New(data() As Byte)
        Me.New()
        Parse(data)
    End Sub
#End Region

#Region "Methods"

#Region "Methods"
    Public Sub Parse(ByVal data() As Byte)
        If data IsNot Nothing Then
            If data.Length >= 3 Then
                Dim CountAsString = System.Text.Encoding.ASCII.GetString(data, 0, 3)
                Dim Count As Integer
                Integer.TryParse(CountAsString, Count)

                Dim index As Integer = 3
                Dim PsetNumberAsString As String
                Dim PsetNumber As Integer
                PsetNumbers.Clear()
                While index < (data.Length - 3 + 3)
                    PsetNumberAsString = System.Text.Encoding.ASCII.GetString(data, index, 3)
                    Integer.TryParse(PsetNumberAsString, PsetNumber)
                    PsetNumbers.Add(PsetNumber)
                    index += 3
                End While
            End If
        End If
    End Sub
#End Region
#End Region
End Class
