Option Strict On
'*************************************************************************
'* Com Driver Event Arguments
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* 09-JUN-21
'*
'*
'* Used to pass response data to events handlers
'*
'* 21-FEB-16 Changed SeqNumber from Uint16 to Int32 for CLS Compliance
'*
'* Copyright (c) 2021 Manufacturing Automation, LLC
'*
'*  This library Is free software; you can redistribute it And/Or
'*  modify it under the terms Of the GNU Lesser General Public
'*  License as published by the Free Software Foundation; either
'*  version 2.1 Of the License, Or (at your option) any later version.
'*  See the file COPYING included With this distribution For more
'*  information.
'************************************************************************
Namespace Common
    <CLSCompliant(True)>
    Public Class PlcComEventArgs
        Inherits EventArgs

        'Implements ICloneable

#Region "Constructors"
        Public Sub New()
            '* Create a new list of values that will be extracted from the Raw Data
            m_Values = New System.Collections.ObjectModel.Collection(Of String)
        End Sub

        Public Sub New(ByVal rawData() As Byte, ByVal plcAddress As String, ByVal sequenceNumber As Int32) ' UInt16)
            MyClass.New()

            m_RawData = rawData

            m_PlcAddress = plcAddress
            m_TransactionNumber = sequenceNumber
        End Sub

        Public Sub New(ByVal rawData() As Byte, ByVal plcAddress As String, ByVal sequenceNumber As Int32, ownerObjectID As Int64)
            MyClass.New(rawData, plcAddress, sequenceNumber)

            m_OwnerObjectID = ownerObjectID
        End Sub

        Public Sub New(values() As String, plcAddress As String, ByVal sequenceNumber As Int32)
            m_Values = New System.Collections.ObjectModel.Collection(Of String)(values)
            'Dim d(values.Length - 1) As String
            'For i = 0 To d.Length - 1
            'm_ = values(i)
            'Next
            'm_Values.AddRange(Values)

            m_PlcAddress = plcAddress
            m_TransactionNumber = sequenceNumber
        End Sub

        Public Sub New(values() As String, plcAddress As String, ByVal sequenceNumber As Int32, ownerObjectID As Int64)
            m_Values = New System.Collections.ObjectModel.Collection(Of String)
            Dim d(values.Length - 1) As String
            For i = 0 To d.Length - 1
                m_Values.Add(d(i))
            Next
            'm_Values.AddRange(Values)

            m_PlcAddress = plcAddress
            m_TransactionNumber = sequenceNumber

            m_OwnerObjectID = ownerObjectID
        End Sub

        '* When used for error message
        Public Sub New(ByVal errorId As Integer, ByVal errorMessage As String)
            m_Values = New System.Collections.ObjectModel.Collection(Of String)
            m_ErrorId = errorId
            m_ErrorMessage = errorMessage
        End Sub

        Public Sub New(ByVal errorId As Integer, ByVal errorMessage As String, ByVal transactionNumber As Int32)
            m_Values = New System.Collections.ObjectModel.Collection(Of String)
            m_ErrorId = errorId
            m_ErrorMessage = errorMessage
            m_TransactionNumber = transactionNumber
        End Sub

        Public Sub New(ByVal errorId As Integer, ByVal errorMessage As String, ByVal transactionNumber As Int32, ByVal ownerObjectID As Int64)
            m_Values = New System.Collections.ObjectModel.Collection(Of String)
            m_ErrorId = errorId
            m_ErrorMessage = errorMessage
            m_TransactionNumber = transactionNumber
            m_OwnerObjectID = ownerObjectID
        End Sub
#End Region

#Region "Properties"
        '****************************************
        '* Extracted values from Raw Byte Stream
        '****************************************
        Protected m_Values As System.Collections.ObjectModel.Collection(Of String)
        Public Property Values() As System.Collections.ObjectModel.Collection(Of String)
            Get
                Return m_Values
            End Get
            Set(value As System.Collections.ObjectModel.Collection(Of String))
                m_Values = value
            End Set
        End Property

        '*************************************
        '* Raw data byte stream from response
        '*************************************
        Protected m_RawData As Byte()
        Public ReadOnly Property RawData() As Byte()
            Get
                Return m_RawData
            End Get
        End Property

        Private m_ResponseFrame As Object
        Public Property ResponseFrame As Object
            Get
                Return m_ResponseFrame
            End Get
            Set(value As Object)
                m_ResponseFrame = value
            End Set
        End Property

        Private m_PlcAddress As String
        Public Property PlcAddress() As String
            Get
                Return m_PlcAddress
            End Get
            Set(ByVal value As String)
                m_PlcAddress = value
            End Set
        End Property

        Protected m_TransactionNumber As Int32 ' UInt16
        Public ReadOnly Property TransactionNumber() As Int32 ' UInt16
            Get
                Return m_TransactionNumber
            End Get
        End Property

        Protected m_ErrorMessage As String
        Public Property ErrorMessage() As String
            Get
                Return m_ErrorMessage
            End Get
            Set(value As String)
                m_ErrorMessage = value
            End Set
        End Property

        Protected m_ErrorId As Integer
        Public Property ErrorId() As Integer
            Get
                Return m_ErrorId
            End Get
            Set(ByVal value As Integer)
                m_ErrorId = value
            End Set
        End Property

        Protected m_SubscriptionID As Integer
        Public Property SubscriptionID As Integer
            Get
                Return m_SubscriptionID
            End Get
            Set(value As Integer)
                m_SubscriptionID = value
            End Set
        End Property

        Protected m_OwnerObjectID As Int64
        Public Property OwnerObjectID As Int64
            Get
                Return m_OwnerObjectID
            End Get
            Set(value As Int64)
                m_OwnerObjectID = value
            End Set
        End Property

        'Private m_DataType As Integer
        'Public Property DataType As Integer
        '    Get
        '        Return m_DataType
        '    End Get
        '    Set(value As Integer)
        '        m_DataType = value
        '    End Set
        'End Property
#End Region


        Public Function Clone() As Object 'Implements ICloneable.Clone
            Dim pea As New PlcComEventArgs(m_ErrorId, m_ErrorMessage, m_TransactionNumber, m_OwnerObjectID)

            pea.PlcAddress = m_PlcAddress
            pea.m_SubscriptionID = m_SubscriptionID
            pea.m_Values = m_Values
            pea.m_ErrorMessage = m_ErrorMessage
            pea.m_ErrorId = m_ErrorId
            pea.m_OwnerObjectID = m_OwnerObjectID
            pea.m_TransactionNumber = m_TransactionNumber
            pea.m_RawData = m_RawData



            '*TODO : finish cloning

            Return pea
        End Function
    End Class
End Namespace