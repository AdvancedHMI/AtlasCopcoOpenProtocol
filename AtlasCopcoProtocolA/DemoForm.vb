Public Class DemoForm
    Private ac As New AtlasCopco
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Dim y = ac.TighteningResultUpload(0)
        'Dim x = ac.SelectParameterSet(1)
        AddHandler ac.AlarmReceived, AddressOf AlarmReceived
        AddHandler ac.TighteningResultReceived, AddressOf TorqueDataReceived
        AddHandler ac.PsetChangeReceived, AddressOf PsetChangeReceived
    End Sub

    Private Sub ListPsets_Click(sender As Object, e As EventArgs) Handles ListPSetsButton.Click
        Dim sets As MID0011 = ac.ParameterSetRequest()

        ListBox1.Items.Clear()
        For i = 0 To sets.PsetNumbers.Count - 1
            ListBox1.Items.Add(sets.PsetNumbers(i))
        Next
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ac.LastResultSubscribe()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles AlarmSubscribeButton.Click
        ac.AlarmSubscribe()

        ListBox3.Items.Clear()
    End Sub

    Private Sub ConnectButton_Click(sender As Object, e As EventArgs) Handles ConnectButton.Click
        ac.IPAddress = IPAddressTextBox.Text
        Try
            ac.CommunicationStart()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        If ListBox1.SelectedIndex >= 0 Then
            Dim Pset As MID0013 = ac.ParameterSetDataRequest(ListBox1.SelectedItem)

            ListBox2.Items.Clear()
            ListBox2.Items.Add("PsetID=" & Pset.PsetID)
            ListBox2.Items.Add("Pset Name=" & Pset.PSetname)
            ListBox2.Items.Add("Torque Min=" & Pset.TorqueMin)
            ListBox2.Items.Add("Torque Max=" & Pset.TorqueMax)
            ListBox2.Items.Add("Angle Min=" & Pset.AngleMin)
            ListBox2.Items.Add("Angle Max=" & Pset.AngleMax)
            ListBox2.Items.Add("Torque Final Target=" & Pset.TorqueFinalTarget)
        Else
            MsgBox("Select item from Pset List")
        End If
    End Sub


    '*************************************************************************
    '* Events
    '*************************************************************************
    Public Sub AlarmReceived(ByVal sender As Object, ByVal e As MID0071)
        ListBox3.Items.Add("Alarm : " & e.TimeStamp & " - Code: " & e.ErrorCode)
    End Sub

    Public Sub TorqueDataReceived(ByVal sender As Object, ByVal e As MID0061)
        Me.BeginInvoke(dcc, New Object() {e})
    End Sub

    Delegate Sub TorqueDataDelegate(e As MID0061)
    Private dcc As TorqueDataDelegate = AddressOf TorqueDataDisplay
    Private Sub TorqueDataDisplay(ByVal e As MID0061)
        TorqueResultListBox.Items.Add(Now())
        TorqueResultListBox.Items.Add("Cell ID(01)=" & e.CellID)
        TorqueResultListBox.Items.Add("Controller Name(03)=" & e.TorqueControllerName)
        TorqueResultListBox.Items.Add("Job ID(05)=" & e.JobNumber)
        TorqueResultListBox.Items.Add("Pset ID(06)=" & e.PSetNumber)
        TorqueResultListBox.Items.Add("Batch Counter(08)=" & e.BatchCounter)
        TorqueResultListBox.Items.Add("Torque Min(12)=" & e.TorqueMinLimit)
        TorqueResultListBox.Items.Add("Torque Max(13)=" & e.TorqueMaxLimit)
        TorqueResultListBox.Items.Add("Final Target(14)=" & e.TorqueFinalTarget)
        TorqueResultListBox.Items.Add("Torque(15)=" & e.Torque)
        TorqueResultListBox.Items.Add("Angle(19)=" & e.Angle)
        TorqueResultListBox.Items.Add("ID(23)=" & e.TighteningID)
    End Sub

    Private Sub PsetChangeReceived(ByVal sender As Object, ByVal e As MID0015)
        Me.BeginInvoke(pcd, New Object() {e})
    End Sub

    Delegate Sub PsetChangeDelegate(e As MID0015)
    Private pcd As PsetChangeDelegate = AddressOf PsetChangeDisplay
    Private Sub PsetChangeDisplay(ByVal e As MID0015)
        PsetSelected.Items.Add(e.DateofLastPsetChange)
        PsetSelected.Items.Add(e.PsetNumber)
    End Sub

    Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles Button3.Click
        If ListBox1.SelectedIndex >= 0 Then
            ac.SelectParameterSet(ListBox1.SelectedIndex + 1)
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ac.LastResultUnSubscribe()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim dataAsString As String = "010000020003Cylinder West 2          04                         054206005070006080001091101111120800001310000014090000150903501600000170999918000001900051201993-05-17:02:26:20211991-02-15:14:30:22220230000282058"
        Dim d(dataAsString.Length - 1) As Byte
        For index = 0 To d.Length - 1
            d(index - 0) = Asc(dataAsString(index))
        Next

        Dim Result As New MID0061(d)

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        ac.ParameterSelectedSubscribe()
    End Sub
End Class
