'*
'* Copyright (c) 2021 Manufacturing Automation, LLC
'*
'*  This library Is free software; you can redistribute it And/Or
'*  modify it under the terms Of the GNU Lesser General Public
'*  License as published by the Free Software Foundation; either
'*  version 2.1 Of the License, Or (at your option) any later version.
'*  See the file COPYING included With this distribution For more
'*  information.

Namespace Common
    <CLSCompliant(True)>
    Public Class SubscriptionEventArgs
        Inherits Common.PlcComEventArgs

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal b As Common.PlcComEventArgs)
            MyBase.New()

            m_Values = b.Values
            m_RawData = b.RawData
            PlcAddress = b.PlcAddress
            m_TransactionNumber = b.TransactionNumber
            m_ErrorMessage = b.ErrorMessage
            m_ErrorId = b.ErrorId
            m_SubscriptionID = b.SubscriptionID
            m_OwnerObjectID = b.OwnerObjectID
        End Sub


        Public Sub New(ByVal rawData() As Byte, ByVal plcAddress As String, ByVal sequenceNumber As Int32)
            MyBase.New(rawData, plcAddress, sequenceNumber)
        End Sub

        Public Sub New(ByVal errorId As Integer, ByVal errorMessage As String, ByVal transactionNumber As Int32, ByVal ownerObjectID As Int64)
            MyBase.New(errorId, errorMessage, transactionNumber, ownerObjectID)
        End Sub


        Public dlgCallBack As EventHandler(Of Common.PlcComEventArgs)
        Public PollRate As Integer
        Public ID As Integer
        'Public DataType As Int64
    End Class
End Namespace
