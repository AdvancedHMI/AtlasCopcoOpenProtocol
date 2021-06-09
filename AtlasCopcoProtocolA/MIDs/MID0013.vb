'************************************************************
'* MID 0013 - 5.5.4 Parameter set data upload reply 
'* Sent By: Torque Controller
'*
'* Pset ID - 3 bytes  (range 000 - 999)
'* Pset Name - 25 bytes (ASCII Characters)
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
Public Class MID0013
#Region "Poperties"
    Public Property PsetID As Integer
    Public Property PSetname As String
    Public Property RotationDirection As Integer
    Public Property BatchSize As Integer

    Public Property TorqueMin As Single
    Public Property TorqueMax As Single
    Public Property TorqueFinalTarget As Single

    Public Property AngleMin As Single
    Public Property AngleMax As Single
    Public Property AngleFinalTarget As Single


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

#Region "Methods"
    Private Sub Parse(ByVal data() As Byte)
        If data IsNot Nothing Then
            If data.Length >= (3 + 2) Then
                Dim index As Integer = 0
                Dim IDAsString = System.Text.Encoding.ASCII.GetString(data, index + 2, 3)
                Integer.TryParse(IDAsString, PsetID)

                index += 3 + 2
                If data.Length > index + 25 Then
                    PSetname = System.Text.ASCIIEncoding.ASCII.GetString(data, index + 2, 25)
                    PSetname = PSetname.Trim()

                    index += 25 + 2
                    If data.Length > (index + 1 + 2) Then
                        Dim RotationAsString = System.Text.Encoding.ASCII.GetString(data, index + 2, 1)
                        Integer.TryParse(RotationAsString, RotationDirection)

                        index += 1 + 2
                        If data.Length > (index + 2 + 2) Then
                            Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, 2), BatchSize)
                            index += 2 + 2

                            If data.Length > (index + 6 + 2) Then
                                Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, 6), TorqueMin)
                                TorqueMin /= 100

                                index += 6 + 2
                                If data.Length > (index + 6 + 2) Then
                                    Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, 6), TorqueMax)
                                    TorqueMax /= 100
                                    index += 6 + 2

                                    If data.Length > (index + 6 + 2) Then
                                        Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, 6), TorqueFinalTarget)
                                        TorqueFinalTarget /= 100
                                        index += 6 + 2

                                        If data.Length > (index + 5 + 2) Then
                                            Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, 5), AngleMin)
                                            index += 5 + 2
                                            If data.Length > (index + 5 + 2) Then
                                                Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, 5), AngleMax)
                                                index += 5 + 2
                                                If data.Length > (index + 5 + 2) Then
                                                    Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, 5), AngleFinalTarget)
                                                    index += 5 + 2
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
#End Region
End Class
