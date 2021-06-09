<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ListPSetsButton = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.AlarmSubscribeButton = New System.Windows.Forms.Button()
        Me.IPAddressTextBox = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ConnectButton = New System.Windows.Forms.Button()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.ListBox2 = New System.Windows.Forms.ListBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ListBox3 = New System.Windows.Forms.ListBox()
        Me.TorqueResultListBox = New System.Windows.Forms.ListBox()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.PsetSelected = New System.Windows.Forms.ListBox()
        Me.SuspendLayout()
        '
        'ListPSetsButton
        '
        Me.ListPSetsButton.Location = New System.Drawing.Point(47, 107)
        Me.ListPSetsButton.Name = "ListPSetsButton"
        Me.ListPSetsButton.Size = New System.Drawing.Size(107, 44)
        Me.ListPSetsButton.TabIndex = 0
        Me.ListPSetsButton.Text = "List Available Parameter Sets"
        Me.ListPSetsButton.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(526, 107)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(158, 44)
        Me.Button2.TabIndex = 1
        Me.Button2.Text = "Subscribe To TorqueResult"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'AlarmSubscribeButton
        '
        Me.AlarmSubscribeButton.Location = New System.Drawing.Point(413, 107)
        Me.AlarmSubscribeButton.Name = "AlarmSubscribeButton"
        Me.AlarmSubscribeButton.Size = New System.Drawing.Size(107, 44)
        Me.AlarmSubscribeButton.TabIndex = 2
        Me.AlarmSubscribeButton.Text = "Alarm Subscribe"
        Me.AlarmSubscribeButton.UseVisualStyleBackColor = True
        '
        'IPAddressTextBox
        '
        Me.IPAddressTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.IPAddressTextBox.Location = New System.Drawing.Point(148, 54)
        Me.IPAddressTextBox.Name = "IPAddressTextBox"
        Me.IPAddressTextBox.Size = New System.Drawing.Size(139, 26)
        Me.IPAddressTextBox.TabIndex = 5
        Me.IPAddressTextBox.Text = "10.229.208.35"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(43, 54)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(99, 26)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "IP Address"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ConnectButton
        '
        Me.ConnectButton.Location = New System.Drawing.Point(320, 54)
        Me.ConnectButton.Name = "ConnectButton"
        Me.ConnectButton.Size = New System.Drawing.Size(135, 26)
        Me.ConnectButton.TabIndex = 7
        Me.ConnectButton.Text = "CONNECT"
        Me.ConnectButton.UseVisualStyleBackColor = True
        '
        'ListBox1
        '
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(47, 157)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(107, 251)
        Me.ListBox1.TabIndex = 8
        '
        'ListBox2
        '
        Me.ListBox2.FormattingEnabled = True
        Me.ListBox2.Location = New System.Drawing.Point(160, 157)
        Me.ListBox2.Name = "ListBox2"
        Me.ListBox2.Size = New System.Drawing.Size(153, 251)
        Me.ListBox2.TabIndex = 10
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(160, 107)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(153, 44)
        Me.Button1.TabIndex = 9
        Me.Button1.Text = "List Selected Pset Values"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'ListBox3
        '
        Me.ListBox3.FormattingEnabled = True
        Me.ListBox3.Location = New System.Drawing.Point(413, 157)
        Me.ListBox3.Name = "ListBox3"
        Me.ListBox3.Size = New System.Drawing.Size(107, 251)
        Me.ListBox3.TabIndex = 11
        '
        'TorqueResultListBox
        '
        Me.TorqueResultListBox.FormattingEnabled = True
        Me.TorqueResultListBox.Location = New System.Drawing.Point(526, 157)
        Me.TorqueResultListBox.Name = "TorqueResultListBox"
        Me.TorqueResultListBox.Size = New System.Drawing.Size(261, 251)
        Me.TorqueResultListBox.TabIndex = 12
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(47, 421)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(106, 53)
        Me.Button3.TabIndex = 13
        Me.Button3.Text = "Set Selected Parameter Set Active"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(690, 107)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(97, 44)
        Me.Button4.TabIndex = 14
        Me.Button4.Text = "Unsubscribe"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(691, 12)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(97, 50)
        Me.Button5.TabIndex = 15
        Me.Button5.Text = "Test0061Parser"
        Me.Button5.UseVisualStyleBackColor = True
        Me.Button5.Visible = False
        '
        'Button6
        '
        Me.Button6.Location = New System.Drawing.Point(319, 107)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(88, 44)
        Me.Button6.TabIndex = 16
        Me.Button6.Text = "Pset Selected Subscribe"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'PsetSelected
        '
        Me.PsetSelected.FormattingEnabled = True
        Me.PsetSelected.Location = New System.Drawing.Point(319, 157)
        Me.PsetSelected.Name = "PsetSelected"
        Me.PsetSelected.Size = New System.Drawing.Size(88, 251)
        Me.PsetSelected.TabIndex = 17
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 531)
        Me.Controls.Add(Me.PsetSelected)
        Me.Controls.Add(Me.Button6)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.TorqueResultListBox)
        Me.Controls.Add(Me.ListBox3)
        Me.Controls.Add(Me.ListBox2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.ListBox1)
        Me.Controls.Add(Me.ConnectButton)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.IPAddressTextBox)
        Me.Controls.Add(Me.AlarmSubscribeButton)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.ListPSetsButton)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ListPSetsButton As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents AlarmSubscribeButton As Button
    Friend WithEvents IPAddressTextBox As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents ConnectButton As Button
    Friend WithEvents ListBox1 As ListBox
    Friend WithEvents ListBox2 As ListBox
    Friend WithEvents Button1 As Button
    Friend WithEvents ListBox3 As ListBox
    Friend WithEvents TorqueResultListBox As ListBox
    Friend WithEvents Button3 As Button
    Friend WithEvents Button4 As Button
    Friend WithEvents Button5 As Button
    Friend WithEvents Button6 As Button
    Friend WithEvents PsetSelected As ListBox
End Class
