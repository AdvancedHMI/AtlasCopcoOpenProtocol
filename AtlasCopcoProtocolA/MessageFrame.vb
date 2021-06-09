'***********************************************************************************
'* Atlas Copco Open Protocol Packet
'*
'* 28-DEC-20
'*
'* Copyright (c) 2021 Manufacturing Automation, LLC
'*
'*  This library Is free software; you can redistribute it And/Or
'*  modify it under the terms Of the GNU Lesser General Public
'*  License as published by the Free Software Foundation; either
'*  version 2.1 Of the License, Or (at your option) any later version.
'*  See the file COPYING included With this distribution For more
'*  information.
'***********************************************************************************
Public Class MessageFrame
    Implements IStreamable

#Region "Constructor"
    Public Sub New()
        Data = New System.Collections.ObjectModel.Collection(Of Byte)
    End Sub

    Public Sub New(messageID As Integer)
        Me.New()
        Me.MID = messageID
    End Sub

    Public Sub New(data As Byte(), length As Integer)
        Me.New()

        Dim L As Integer
        If data.Length > 4 Then
            Try
                L = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(data, 0, 4))
            Catch ex As Exception
                Throw New Exception("Mesaage Frame: Invald Length in packet")
            End Try
        End If

        If (data.Length - 1) < L Then
            Throw New Exception("Mesaage Frame: data too short")
        End If

        If data.Length > 8 Then
            Try
                MID = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(data, 4, 4))
            Catch ex As Exception
                Throw New Exception("Mesaage Frame: Invald MessageID")
            End Try
        End If

        If data.Length > 8 Then
            Try
                Dim RevAsString As String = System.Text.Encoding.ASCII.GetString(data, 8, 3)
                ' Section 3.3 Header , note in revision says 3 spaces is rev 1
                If RevAsString = "   " Then
                    Revision = 1
                Else
                    Revision = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(data, 8, 3))
                End If
            Catch ex As Exception
                Throw New Exception("Mesaage Frame: Invald Revision")
            End Try
        End If

        If data.Length > 20 Then
            Try
                Dim index As Integer = 20
                While index < data.Length AndAlso index < L
                    Me.Data.Add(data(index))
                    index += 1
                End While
            Catch ex As Exception
                Throw New Exception("Mesaage Frame: Invald MessageID")
            End Try
        End If
    End Sub
#End Region

#Region "Properties"
    '********************************************
    '* Length of Header (20 bytes) + data field
    '* excluding null termination
    '********************************************
    Public ReadOnly Property Length As Integer
        Get
            Return (20 + Data.Count)
        End Get
    End Property

    '********************************************
    '* Message ID (MID)
    '********************************************
    Public Property MID As Integer
    Public Property Revision As Integer = 1
    Public Property NoAckFlag As Integer

    '********************************************
    '* Transaction ID
    '* used only for internal purposes, not part
    '* of protocol
    '********************************************
    'Public Property TransactionNumber As Integer Implements IStreamable.TransactionNumber

    'Private m_OwnerObjectID As Int64
    Public Property OwnerObjectID As Int64 Implements IStreamable.OwnerObjectID


    '********************************************
    '* Collection Specific to Message type
    '********************************************
    Public Property Data As System.Collections.ObjectModel.Collection(Of Byte)

#End Region

#Region "Public Methods"
    Public Function GetBytes() As Byte() Implements IStreamable.GetBytes
        Dim ByteStream As New List(Of Byte)

        ByteStream.AddRange(System.Text.Encoding.ASCII.GetBytes(Length.ToString("0000")))
        ByteStream.AddRange(System.Text.Encoding.ASCII.GetBytes(MID.ToString("0000")))
        ByteStream.AddRange(System.Text.Encoding.ASCII.GetBytes(Revision.ToString("000")))
        For index = 0 To 8
            ByteStream.Add(32)
        Next

        ByteStream.AddRange(Data)

        '* Null Terminator
        ByteStream.Add(0)

        Return ByteStream.ToArray()
    End Function
#End Region
End Class
