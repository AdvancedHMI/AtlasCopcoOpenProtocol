Option Strict On
'************************************************************
'* MID 0065 - 5.9.2 Last tightening result data upload reply (MID = 0061) 
'* Sent By: Torque Controller
'*
'* Cell ID - 4 bytes
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
Public Class MID0061
#Region "Poperties"
    Public Property CellID As Integer
    Public Property ChannelID As Integer
    Public Property TorqueControllerName As String
    Public Property VINNumber As String

    Public Property JobNumber As Integer
    Public Property PSetNumber As Integer
    Public Property BatchSize As Integer

    Public Property BatchCounter As Integer
    Public Property TighteningStatus As Boolean
    Public Property TorqueStatus As Boolean
    Public Property AngleStatus As Boolean

    Public Property TorqueMinLimit As Single
    Public Property TorqueMaxLimit As Single
    Public Property TorqueFinalTarget As Single
    Public Property Torque As Single

    Public Property AngleMin As Single
    Public Property AngleMax As Single
    Public Property FinalAngleTarget As Single
    Public Property Angle As Single


    Public Property TimeStamp As Date
    Public Property TimeOfLastPsetChange As Date
    Public Property BatchStatus As Boolean

    Public Property TighteningID As Integer
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
            Dim index As Integer = 0
            Dim ItemNumber As Integer
            Dim sItemNumber As String
            '* ITEM 01
            Dim ItemSize As Integer = 4
            If data.Length >= (index + ItemSize + 2) Then
                Dim IDAsString = System.Text.Encoding.ASCII.GetString(data, 2, ItemSize)
                Integer.TryParse(IDAsString, CellID)
                index += ItemSize + 2

                '* ITEM 02
                ItemSize = 2
                If data.Length > (index + ItemSize + 2) Then
                    Dim ChannelIDAsString = System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize)
                    Integer.TryParse(ChannelIDAsString, ChannelID)
                    index += ItemSize + 2

                    '* ITEM 03
                    ItemSize = 25
                    If data.Length > (index + ItemSize + 2) Then
                        TorqueControllerName = System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize)
                        index += ItemSize + 2

                        '* ITEM 04
                        ItemSize = 25
                        If data.Length > (index + ItemSize + 2) Then
                            VINNumber = System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize)
                            index += ItemSize + 2

                            '* ITEM 05
                            ItemSize = 2
                            If data.Length > (index + ItemSize + 2) Then
                                sItemNumber = System.Text.Encoding.ASCII.GetString(data, index, 2)
                                Integer.TryParse(sItemNumber, ItemNumber)

                                Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize), JobNumber)
                                index += ItemSize + 2

                                '* ITEM 06
                                ItemSize = 3
                                If data.Length > (index + ItemSize + 2) Then
                                    sItemNumber = System.Text.Encoding.ASCII.GetString(data, index, 2)
                                    Integer.TryParse(sItemNumber, ItemNumber)

                                    Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize), PSetNumber)
                                    index += ItemSize + 2

                                    '* ITEM 07
                                    ItemSize = 4
                                    If data.Length > (index + ItemSize + 2) Then
                                        sItemNumber = System.Text.Encoding.ASCII.GetString(data, index, 2)
                                        Integer.TryParse(sItemNumber, ItemNumber)

                                        Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize), BatchSize)
                                        index += ItemSize + 2

                                        '* ITEM 08
                                        ItemSize = 4
                                        If data.Length > (index + ItemSize + 2) Then
                                            Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, 4), BatchCounter)
                                            index += ItemSize + 2

                                            '* ITEM 09
                                            ItemSize = 1
                                            If data.Length > (index + ItemSize + 2) Then
                                                Dim Status As Integer
                                                Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, 1), Status)
                                                TighteningStatus = (Status = 1)
                                                index += ItemSize + 2

                                                '* ITEM 10
                                                ItemSize = 1
                                                If data.Length > (index + ItemSize + 2) Then
                                                    Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize), Status)
                                                    TorqueStatus = (Status = 1)
                                                    index += ItemSize + 2

                                                    '* ITEM 11
                                                    ItemSize = 1
                                                    If data.Length > (index + ItemSize + 2) Then
                                                        Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize), Status)
                                                        AngleStatus = (Status = 1)
                                                        index += ItemSize + 2

                                                        '* ITEM 12
                                                        ItemSize = 6
                                                        If data.Length > (index + ItemSize + 2) Then
                                                            Single.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize), TorqueMinLimit)
                                                            TorqueMinLimit /= 100
                                                            index += ItemSize + 2

                                                            '* ITEM 13
                                                            ItemSize = 6
                                                            If data.Length > (index + ItemSize + 2) Then
                                                                Single.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize), TorqueMaxLimit)
                                                                TorqueMaxLimit /= 100
                                                                index += ItemSize + 2

                                                                '* ITEM 14
                                                                ItemSize = 6
                                                                If data.Length > (index + ItemSize + 2) Then
                                                                    Single.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize), TorqueFinalTarget)
                                                                    TorqueFinalTarget /= 100
                                                                    index += ItemSize + 2

                                                                    '* ITEM 15
                                                                    ItemSize = 6
                                                                    If data.Length > (index + ItemSize + 2) Then
                                                                        Single.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize), Torque)
                                                                        Torque /= 100
                                                                        index += ItemSize + 2

                                                                        '* ITEM 16
                                                                        ItemSize = 5
                                                                        If data.Length > (index + ItemSize + 2) Then
                                                                            sItemNumber = System.Text.Encoding.ASCII.GetString(data, index, 2)
                                                                            Integer.TryParse(sItemNumber, ItemNumber)

                                                                            Single.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize), AngleMin)
                                                                            index += ItemSize + 2

                                                                            '* ITEM 17
                                                                            ItemSize = 5
                                                                            If data.Length > (index + ItemSize + 2) Then
                                                                                Single.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, ItemSize), AngleMax)
                                                                                index += ItemSize + 2

                                                                                If data.Length > (index + 5 + 2) Then
                                                                                    Single.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, 5), FinalAngleTarget)
                                                                                    index += 5 + 2

                                                                                    If data.Length > (index + 5 + 2) Then
                                                                                        Single.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, 5), Angle)
                                                                                        index += 5 + 2

                                                                                        If data.Length > (index + 19 + 2) Then
                                                                                            Dim TimeStampAsString As String = System.Text.ASCIIEncoding.ASCII.GetString(data, index + 2, 19).Trim
                                                                                            TimeStampAsString = TimeStampAsString.Substring(0, 10) & " " & TimeStampAsString.Substring(11, 8)
                                                                                            Date.TryParse(TimeStampAsString, TimeStamp)
                                                                                            index += 19 + 2

                                                                                            If data.Length > (index + 19 + 2) Then
                                                                                                sItemNumber = System.Text.Encoding.ASCII.GetString(data, index, 2)
                                                                                                Integer.TryParse(sItemNumber, ItemNumber)

                                                                                                TimeStampAsString = System.Text.ASCIIEncoding.ASCII.GetString(data, index + 2, 19).Trim
                                                                                                TimeStampAsString = TimeStampAsString.Substring(0, 10) & " " & TimeStampAsString.Substring(11, 8)
                                                                                                Date.TryParse(TimeStampAsString, TimeOfLastPsetChange)
                                                                                                index += 19 + 2

                                                                                                If data.Length > (index + 1 + 2) Then
                                                                                                    Integer.TryParse(System.Text.Encoding.ASCII.GetString(data, index + 2, 1), Status)
                                                                                                    BatchStatus = (Status = 1)
                                                                                                    index += 1 + 2

                                                                                                    IDAsString = System.Text.Encoding.ASCII.GetString(data, index + 2, 10)
                                                                                                    Integer.TryParse(IDAsString, TighteningID)

                                                                                                    index += 10 + 2
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
        End If
    End Sub
#End Region
End Class
