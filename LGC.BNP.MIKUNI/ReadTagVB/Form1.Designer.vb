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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.listBox1 = New System.Windows.Forms.ListBox()
        Me.groupBox2 = New System.Windows.Forms.GroupBox()
        Me.listBox2 = New System.Windows.Forms.ListBox()
        Me.groupBox1 = New System.Windows.Forms.GroupBox()
        Me.btn_clear = New System.Windows.Forms.Button()
        Me.button2 = New System.Windows.Forms.Button()
        Me.btn_start = New System.Windows.Forms.Button()
        Me.notifyIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.btn_con_signalr = New System.Windows.Forms.Button()
        Me.label2 = New System.Windows.Forms.Label()
        Me.label1 = New System.Windows.Forms.Label()
        Me.status_reader = New System.Windows.Forms.Button()
        Me.status_segnalr = New System.Windows.Forms.Button()
        Me.groupBox2.SuspendLayout()
        Me.groupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'listBox1
        '
        Me.listBox1.FormattingEnabled = True
        Me.listBox1.Location = New System.Drawing.Point(87, 22)
        Me.listBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.listBox1.Name = "listBox1"
        Me.listBox1.Size = New System.Drawing.Size(324, 342)
        Me.listBox1.TabIndex = 2
        '
        'groupBox2
        '
        Me.groupBox2.Controls.Add(Me.listBox2)
        Me.groupBox2.Location = New System.Drawing.Point(29, 449)
        Me.groupBox2.Margin = New System.Windows.Forms.Padding(2)
        Me.groupBox2.Name = "groupBox2"
        Me.groupBox2.Padding = New System.Windows.Forms.Padding(2)
        Me.groupBox2.Size = New System.Drawing.Size(456, 110)
        Me.groupBox2.TabIndex = 8
        Me.groupBox2.TabStop = False
        Me.groupBox2.Text = "Log"
        '
        'listBox2
        '
        Me.listBox2.FormattingEnabled = True
        Me.listBox2.Location = New System.Drawing.Point(13, 22)
        Me.listBox2.Margin = New System.Windows.Forms.Padding(2)
        Me.listBox2.Name = "listBox2"
        Me.listBox2.Size = New System.Drawing.Size(398, 69)
        Me.listBox2.TabIndex = 0
        '
        'groupBox1
        '
        Me.groupBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.groupBox1.Controls.Add(Me.btn_clear)
        Me.groupBox1.Controls.Add(Me.listBox1)
        Me.groupBox1.Controls.Add(Me.button2)
        Me.groupBox1.Controls.Add(Me.btn_start)
        Me.groupBox1.Location = New System.Drawing.Point(29, 42)
        Me.groupBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Padding = New System.Windows.Forms.Padding(2)
        Me.groupBox1.Size = New System.Drawing.Size(456, 401)
        Me.groupBox1.TabIndex = 7
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "IMPINJ"
        '
        'btn_clear
        '
        Me.btn_clear.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.btn_clear.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(222, Byte))
        Me.btn_clear.ForeColor = System.Drawing.Color.Snow
        Me.btn_clear.Location = New System.Drawing.Point(14, 116)
        Me.btn_clear.Margin = New System.Windows.Forms.Padding(2)
        Me.btn_clear.Name = "btn_clear"
        Me.btn_clear.Size = New System.Drawing.Size(56, 29)
        Me.btn_clear.TabIndex = 3
        Me.btn_clear.Text = "Clear"
        Me.btn_clear.UseVisualStyleBackColor = False
        '
        'button2
        '
        Me.button2.AutoSize = True
        Me.button2.BackColor = System.Drawing.Color.Red
        Me.button2.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.button2.ForeColor = System.Drawing.Color.White
        Me.button2.Location = New System.Drawing.Point(13, 73)
        Me.button2.Margin = New System.Windows.Forms.Padding(2)
        Me.button2.Name = "button2"
        Me.button2.Size = New System.Drawing.Size(58, 31)
        Me.button2.TabIndex = 1
        Me.button2.Text = "Stop"
        Me.button2.UseVisualStyleBackColor = False
        '
        'btn_start
        '
        Me.btn_start.BackColor = System.Drawing.Color.MediumSeaGreen
        Me.btn_start.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_start.ForeColor = System.Drawing.Color.White
        Me.btn_start.Location = New System.Drawing.Point(13, 31)
        Me.btn_start.Margin = New System.Windows.Forms.Padding(2)
        Me.btn_start.Name = "btn_start"
        Me.btn_start.Size = New System.Drawing.Size(58, 28)
        Me.btn_start.TabIndex = 0
        Me.btn_start.Text = "Start"
        Me.btn_start.UseVisualStyleBackColor = False
        '
        'notifyIcon
        '
        Me.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
        Me.notifyIcon.BalloonTipText = "RFID READER"
        Me.notifyIcon.BalloonTipTitle = "program runing on background"
        Me.notifyIcon.Icon = CType(resources.GetObject("notifyIcon.Icon"), System.Drawing.Icon)
        Me.notifyIcon.Text = "notifyIcon"
        '
        'btn_con_signalr
        '
        Me.btn_con_signalr.BackColor = System.Drawing.Color.Transparent
        Me.btn_con_signalr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btn_con_signalr.FlatAppearance.BorderColor = System.Drawing.Color.Red
        Me.btn_con_signalr.FlatAppearance.BorderSize = 2
        Me.btn_con_signalr.Location = New System.Drawing.Point(214, 16)
        Me.btn_con_signalr.Margin = New System.Windows.Forms.Padding(2)
        Me.btn_con_signalr.Name = "btn_con_signalr"
        Me.btn_con_signalr.Size = New System.Drawing.Size(19, 20)
        Me.btn_con_signalr.TabIndex = 13
        Me.btn_con_signalr.UseVisualStyleBackColor = False
        Me.btn_con_signalr.Visible = False
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(263, 19)
        Me.label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(58, 13)
        Me.label2.TabIndex = 12
        Me.label2.Text = "READER :"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(63, 19)
        Me.label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(70, 13)
        Me.label1.TabIndex = 11
        Me.label1.Text = "WebSocket :"
        Me.label1.Visible = False
        '
        'status_reader
        '
        Me.status_reader.BackColor = System.Drawing.Color.Red
        Me.status_reader.ForeColor = System.Drawing.Color.White
        Me.status_reader.Location = New System.Drawing.Point(320, 12)
        Me.status_reader.Margin = New System.Windows.Forms.Padding(2)
        Me.status_reader.Name = "status_reader"
        Me.status_reader.Size = New System.Drawing.Size(81, 23)
        Me.status_reader.TabIndex = 10
        Me.status_reader.Text = "Disconnect"
        Me.status_reader.UseVisualStyleBackColor = False
        '
        'status_segnalr
        '
        Me.status_segnalr.BackColor = System.Drawing.Color.Red
        Me.status_segnalr.ForeColor = System.Drawing.Color.White
        Me.status_segnalr.Location = New System.Drawing.Point(129, 15)
        Me.status_segnalr.Margin = New System.Windows.Forms.Padding(2)
        Me.status_segnalr.Name = "status_segnalr"
        Me.status_segnalr.Size = New System.Drawing.Size(81, 23)
        Me.status_segnalr.TabIndex = 9
        Me.status_segnalr.Text = "Disconnect"
        Me.status_segnalr.UseVisualStyleBackColor = False
        Me.status_segnalr.Visible = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(531, 608)
        Me.Controls.Add(Me.groupBox2)
        Me.Controls.Add(Me.groupBox1)
        Me.Controls.Add(Me.btn_con_signalr)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.status_reader)
        Me.Controls.Add(Me.status_segnalr)
        Me.Name = "Form1"
        Me.Text = "ReadTag VB"
        Me.groupBox2.ResumeLayout(False)
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents listBox1 As ListBox
    Private WithEvents groupBox2 As GroupBox
    Private WithEvents listBox2 As ListBox
    Private WithEvents groupBox1 As GroupBox
    Private WithEvents btn_clear As Button
    Private WithEvents button2 As Button
    Private WithEvents btn_start As Button
    Private WithEvents notifyIcon As NotifyIcon
    Private WithEvents btn_con_signalr As Button
    Private WithEvents label2 As Label
    Private WithEvents label1 As Label
    Private WithEvents status_reader As Button
    Private WithEvents status_segnalr As Button
End Class
