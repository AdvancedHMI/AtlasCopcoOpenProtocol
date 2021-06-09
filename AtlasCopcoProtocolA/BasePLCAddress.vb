Option Strict On
'*
'* Copyright (c) 2021 Manufacturing Automation, LLC
'*
'*  This library Is free software; you can redistribute it And/Or
'*  modify it under the terms Of the GNU Lesser General Public
'*  License as published by the Free Software Foundation; either
'*  version 2.1 Of the License, Or (at your option) any later version.
'*  See the file COPYING included With this distribution For more
'*  information.
Option Explicit On
Public MustInherit Class BasePLCAddress
    Protected MustOverride Sub ParseAddress()


    Protected m_Address As String
    Public Property Address() As String
        Get
            Return m_Address
        End Get
        Set(ByVal value As String)
            m_Address = value
            ParseAddress()
        End Set
    End Property

    Protected m_BitsPerElement As Integer
    Public Property BitsPerElement() As Integer
        Get
            Return m_BitsPerElement
        End Get
        Set(ByVal value As Integer)
            m_BitsPerElement = value
        End Set
    End Property

    Protected m_ElementNumber As Integer
    Public Property ElementNumber() As Integer
        Get
            Return m_ElementNumber
        End Get
        Set(ByVal value As Integer)
            m_ElementNumber = value
        End Set
    End Property

    Protected m_BitNumber As Integer
    Public Property BitNumber() As Integer
        Get
            Return m_BitNumber
        End Get
        Set(ByVal value As Integer)
            m_BitNumber = value
        End Set
    End Property

    Protected m_NumberOfElements As Integer
    Public Property NumberOfElements() As Integer
        Get
            Return m_NumberOfElements
        End Get
        Set(ByVal value As Integer)
            m_NumberOfElements = value
        End Set
    End Property

    Protected m_Modifier As String
    Public ReadOnly Property Modifier() As String
        Get
            Return m_Modifier
        End Get
    End Property
End Class
