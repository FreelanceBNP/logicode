Namespace LGC.BNP.MIKUNI.Read.Tag
    Partial Class Form1
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <paramname="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Windows Form Designer generated code"

        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            components = New ComponentModel.Container()
            Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(Form1))
            groupBox2 = New Windows.Forms.GroupBox()
            listBox2 = New Windows.Forms.ListBox()
            groupBox1 = New Windows.Forms.GroupBox()
            btn_clear = New Windows.Forms.Button()
            listBox1 = New Windows.Forms.ListBox()
            button2 = New Windows.Forms.Button()
            btn_start = New Windows.Forms.Button()
            notifyIcon = New Windows.Forms.NotifyIcon(components)
            status_reader = New Windows.Forms.Button()
            label1 = New Windows.Forms.Label()
            label2 = New Windows.Forms.Label()
            status_segnalr = New Windows.Forms.Button()
            btn_con_signalr = New Windows.Forms.Button()
            groupBox2.SuspendLayout()
            groupBox1.SuspendLayout()
            SuspendLayout()
            ' 
            ' groupBox2
            ' 
            groupBox2.Controls.Add(listBox2)
            groupBox2.Location = New Drawing.Point(46, 444)
            groupBox2.Margin = New Windows.Forms.Padding(2, 2, 2, 2)
            groupBox2.Name = "groupBox2"
            groupBox2.Padding = New Windows.Forms.Padding(2, 2, 2, 2)
            groupBox2.Size = New Drawing.Size(456, 110)
            groupBox2.TabIndex = 1
            groupBox2.TabStop = False
            groupBox2.Text = "Log"
            ' 
            ' listBox2
            ' 
            listBox2.FormattingEnabled = True
            listBox2.Location = New Drawing.Point(13, 22)
            listBox2.Margin = New Windows.Forms.Padding(2, 2, 2, 2)
            listBox2.Name = "listBox2"
            listBox2.Size = New Drawing.Size(398, 69)
            listBox2.TabIndex = 0
            ' 
            ' groupBox1
            ' 
            groupBox1.BackgroundImageLayout = Windows.Forms.ImageLayout.None
            groupBox1.Controls.Add(btn_clear)
            groupBox1.Controls.Add(listBox1)
            groupBox1.Controls.Add(button2)
            groupBox1.Controls.Add(btn_start)
            groupBox1.Location = New Drawing.Point(46, 37)
            groupBox1.Margin = New Windows.Forms.Padding(2, 2, 2, 2)
            groupBox1.Name = "groupBox1"
            groupBox1.Padding = New Windows.Forms.Padding(2, 2, 2, 2)
            groupBox1.Size = New Drawing.Size(456, 401)
            groupBox1.TabIndex = 0
            groupBox1.TabStop = False
            groupBox1.Text = "IMPINJ"
            ' 
            ' btn_clear
            ' 
            btn_clear.BackColor = Drawing.SystemColors.ActiveCaption
            btn_clear.Font = New Drawing.Font("Microsoft Sans Serif", 10.2F, Drawing.FontStyle.Bold Or Drawing.FontStyle.Italic, Drawing.GraphicsUnit.Point, 222)
            btn_clear.ForeColor = Drawing.Color.Snow
            btn_clear.Location = New Drawing.Point(14, 116)
            btn_clear.Margin = New Windows.Forms.Padding(2, 2, 2, 2)
            btn_clear.Name = "btn_clear"
            btn_clear.Size = New Drawing.Size(56, 29)
            btn_clear.TabIndex = 3
            btn_clear.Text = "Clear"
            btn_clear.UseVisualStyleBackColor = False
            AddHandler btn_clear.Click, New EventHandler(AddressOf btn_clear_DoubleClick)
            ' 
            ' listBox1
            ' 
            listBox1.FormattingEnabled = True
            listBox1.Location = New Drawing.Point(87, 22)
            listBox1.Margin = New Windows.Forms.Padding(2, 2, 2, 2)
            listBox1.Name = "listBox1"
            listBox1.Size = New Drawing.Size(324, 342)
            listBox1.TabIndex = 2
            ' 
            ' button2
            ' 
            button2.AutoSize = True
            button2.BackColor = Drawing.Color.Red
            button2.Font = New Drawing.Font("Microsoft Sans Serif", 10.2F, Drawing.FontStyle.Bold Or Drawing.FontStyle.Italic, Drawing.GraphicsUnit.Point, 0)
            button2.ForeColor = Drawing.Color.White
            button2.Location = New Drawing.Point(13, 73)
            button2.Margin = New Windows.Forms.Padding(2, 2, 2, 2)
            button2.Name = "button2"
            button2.Size = New Drawing.Size(58, 31)
            button2.TabIndex = 1
            button2.Text = "Stop"
            button2.UseVisualStyleBackColor = False
            AddHandler button2.Click, New EventHandler(AddressOf button2_Click)
            ' 
            ' btn_start
            ' 
            btn_start.BackColor = Drawing.Color.MediumSeaGreen
            btn_start.Font = New Drawing.Font("Microsoft Sans Serif", 10.2F, Drawing.FontStyle.Bold Or Drawing.FontStyle.Italic, Drawing.GraphicsUnit.Point, 0)
            btn_start.ForeColor = Drawing.Color.White
            btn_start.Location = New Drawing.Point(13, 31)
            btn_start.Margin = New Windows.Forms.Padding(2, 2, 2, 2)
            btn_start.Name = "btn_start"
            btn_start.Size = New Drawing.Size(58, 28)
            btn_start.TabIndex = 0
            btn_start.Text = "Start"
            btn_start.UseVisualStyleBackColor = False
            AddHandler btn_start.Click, New EventHandler(AddressOf button1_Click)
            ' 
            ' notifyIcon
            ' 
            notifyIcon.BalloonTipIcon = Windows.Forms.ToolTipIcon.Info
            notifyIcon.BalloonTipText = "RFID READER"
            notifyIcon.BalloonTipTitle = "program runing on background"
            notifyIcon.Icon = CType(resources.GetObject("notifyIcon.Icon"), Drawing.Icon)
            notifyIcon.Text = "notifyIcon"
            AddHandler notifyIcon.DoubleClick, New EventHandler(AddressOf notifyIcon_DoubleClick)
            ' 
            ' status_reader
            ' 
            status_reader.BackColor = Drawing.Color.Red
            status_reader.ForeColor = Drawing.Color.White
            status_reader.Location = New Drawing.Point(337, 7)
            status_reader.Margin = New Windows.Forms.Padding(2, 2, 2, 2)
            status_reader.Name = "status_reader"
            status_reader.Size = New Drawing.Size(81, 23)
            status_reader.TabIndex = 3
            status_reader.Text = "Disconnect"
            status_reader.UseVisualStyleBackColor = False
            ' 
            ' label1
            ' 
            label1.AutoSize = True
            label1.Location = New Drawing.Point(80, 14)
            label1.Margin = New Windows.Forms.Padding(2, 0, 2, 0)
            label1.Name = "label1"
            label1.Size = New Drawing.Size(70, 13)
            label1.TabIndex = 4
            label1.Text = "WebSocket :"
            label1.Visible = False
            ' 
            ' label2
            ' 
            label2.AutoSize = True
            label2.Location = New Drawing.Point(280, 14)
            label2.Margin = New Windows.Forms.Padding(2, 0, 2, 0)
            label2.Name = "label2"
            label2.Size = New Drawing.Size(58, 13)
            label2.TabIndex = 5
            label2.Text = "READER :"
            ' 
            ' status_segnalr
            ' 
            status_segnalr.BackColor = Drawing.Color.Red
            status_segnalr.ForeColor = Drawing.Color.White
            status_segnalr.Location = New Drawing.Point(146, 10)
            status_segnalr.Margin = New Windows.Forms.Padding(2, 2, 2, 2)
            status_segnalr.Name = "status_segnalr"
            status_segnalr.Size = New Drawing.Size(81, 23)
            status_segnalr.TabIndex = 2
            status_segnalr.Text = "Disconnect"
            status_segnalr.UseVisualStyleBackColor = False
            status_segnalr.Visible = False
            ' 
            ' btn_con_signalr
            ' 
            btn_con_signalr.BackColor = Drawing.Color.Transparent
            btn_con_signalr.BackgroundImage = Global.Properties.Resources.redo_7_256x256
            btn_con_signalr.BackgroundImageLayout = Windows.Forms.ImageLayout.Zoom
            btn_con_signalr.FlatAppearance.BorderColor = Drawing.Color.Red
            btn_con_signalr.FlatAppearance.BorderSize = 2
            btn_con_signalr.Location = New Drawing.Point(231, 11)
            btn_con_signalr.Margin = New Windows.Forms.Padding(2, 2, 2, 2)
            btn_con_signalr.Name = "btn_con_signalr"
            btn_con_signalr.Size = New Drawing.Size(19, 20)
            btn_con_signalr.TabIndex = 6
            btn_con_signalr.UseVisualStyleBackColor = False
            btn_con_signalr.Visible = False
            AddHandler btn_con_signalr.Click, New EventHandler(AddressOf btn_con_signalr_Click)
            ' 
            ' Form1
            ' 
            AutoScaleDimensions = New Drawing.SizeF(6F, 13F)
            AutoScaleMode = Windows.Forms.AutoScaleMode.Font
            BackgroundImageLayout = Windows.Forms.ImageLayout.None
            ClientSize = New Drawing.Size(521, 573)
            Controls.Add(btn_con_signalr)
            Controls.Add(label2)
            Controls.Add(label1)
            Controls.Add(status_reader)
            Controls.Add(status_segnalr)
            Controls.Add(groupBox2)
            Controls.Add(groupBox1)
            DoubleBuffered = True
            FormBorderStyle = Windows.Forms.FormBorderStyle.FixedDialog
            Margin = New Windows.Forms.Padding(2, 2, 2, 2)
            MaximizeBox = False
            Name = "Form1"
            StartPosition = Windows.Forms.FormStartPosition.CenterScreen
            Text = "RFID Reader"
            AddHandler FormClosing, New Windows.Forms.FormClosingEventHandler(AddressOf Form1_FormClosing)
            AddHandler Load, New EventHandler(AddressOf Form1_Load)
            AddHandler Resize, New EventHandler(AddressOf Form1_Resize)
            groupBox2.ResumeLayout(False)
            groupBox1.ResumeLayout(False)
            groupBox1.PerformLayout()
            ResumeLayout(False)
            PerformLayout()

        End Sub

#End Region

        Private groupBox1 As Windows.Forms.GroupBox
        Private groupBox2 As Windows.Forms.GroupBox
        Private btn_start As Windows.Forms.Button
        Private button2 As Windows.Forms.Button
        Private listBox1 As Windows.Forms.ListBox
        Private listBox2 As Windows.Forms.ListBox
        Private notifyIcon As Windows.Forms.NotifyIcon
        Private btn_clear As Windows.Forms.Button
        Private status_reader As Windows.Forms.Button
        Private label1 As Windows.Forms.Label
        Private label2 As Windows.Forms.Label
        Private btn_con_signalr As Windows.Forms.Button
        Private status_segnalr As Windows.Forms.Button
    End Class
End Namespace
