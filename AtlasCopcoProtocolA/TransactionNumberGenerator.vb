'*
'* Copyright (c) 2021 Manufacturing Automation, LLC
'*
'*  This library Is free software; you can redistribute it And/Or
'*  modify it under the terms Of the GNU Lesser General Public
'*  License as published by the Free Software Foundation; either
'*  version 2.1 Of the License, Or (at your option) any later version.
'*  See the file COPYING included With this distribution For more
'*  information.

Option Strict On

Namespace Common
    Public NotInheritable Class TransactionNumberGenerator
        Private Sub New()
        End Sub

        Private Shared NewNumberLockObject As New Object
        Private Shared NextNumber As Integer

        Public Shared Function GetNextTNSNumber(ByVal max As Integer) As Integer
            SyncLock (NewNumberLockObject)
                If NextNumber = 0 Then
                    '* Start with a random number because if one read is done, then the app restarted, the PLC will think it is a duplicated packet
                    Dim rnd As New Random
                    NextNumber = Convert.ToInt32(rnd.Next(127)) + CInt(max / 2)
                Else
                    NextNumber += 1
                End If

                If NextNumber >= max Then NextNumber = 1


                Return NextNumber
            End SyncLock
        End Function
    End Class
End Namespace
