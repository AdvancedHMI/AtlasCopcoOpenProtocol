'************************************************************
'* MID 0065 - 5.9.6 Old tightening result reply 
'* Sent By: Torque Controller
'*
'* Tightening ID - 10 bytes
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
Public Class MID0065
#Region "Poperties"
    Public Property TighteningID As Integer
    Public Property VINNumber As String
    Public Property PSetNumber As Integer
    Public Property BatchCounter As Integer

    Public Property TighteningStatus As Boolean
    Public Property TorqueStatus As Boolean
    Public Property AngleStatus As Boolean

    Public Property Torque As Single
    Public Property Angle As Single
    Public Property TimeStamp As Date
    Public Property BatchStatus As Boolean


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
            If data.Length >= 10 Then
                Dim index As Integer = 0
                Dim IDAsString = System.Text.Encoding.ASCII.GetString(data, 0, 10)
                Integer.TryParse(IDAsString, TighteningID)

                index += 10
                If data.Length > index + 25 Then
                    VINNumber = System.Text.ASCIIEncoding.ASCII.GetString(data, index, 25).Trim

                    index += 25
                    If data.Length > index + 3 Then
                        Dim PSetNumberAsString = System.Text.Encoding.ASCII.GetString(data, index, 3)
                        Integer.TryParse(PSetNumberAsString, PSetNumber)

                        index += 3
                        If data.Length > index + 4 Then
                            Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index, 4), BatchCounter)
                            index += 4

                            If data.Length > index + 1 Then
                                Dim Status As Integer
                                Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index, 1), Status)
                                TighteningStatus = (Status = 1)

                                index += 1
                                If data.Length > index + 1 Then
                                    Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index, 1), Status)
                                    AngleStatus = (Status = 1)
                                    index += 1

                                    If data.Length > index + 6 Then
                                        Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index, 6), Torque)
                                        Torque /= 100
                                        index += 6

                                        If data.Length > index + 5 Then
                                            Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index, 5), Angle)
                                            Angle = AngleStatus / 100
                                            index += 5
                                            If data.Length > index + 19 Then
                                                Dim TimeStampAsString As String = System.Text.ASCIIEncoding.ASCII.GetString(data, index, 19).Trim
                                                Date.TryParse(TimeStampAsString, TimeStamp)
                                                index += 19

                                                If data.Length > index + 1 Then
                                                    Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index, 1), Status)
                                                    BatchStatus = (Status = 1)
                                                    index += 1
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub
#End Region
End Class
