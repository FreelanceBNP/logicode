namespace LGC.BNP.MIKUNI.Read.Tag
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.listBox2 = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btn_clear = new System.Windows.Forms.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.button2 = new System.Windows.Forms.Button();
			this.btn_start = new System.Windows.Forms.Button();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.status_reader = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.status_segnalr = new System.Windows.Forms.Button();
			this.btn_con_signalr = new System.Windows.Forms.Button();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.listBox2);
			this.groupBox2.Location = new System.Drawing.Point(61, 546);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.groupBox2.Size = new System.Drawing.Size(608, 135);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Log";
			// 
			// listBox2
			// 
			this.listBox2.FormattingEnabled = true;
			this.listBox2.ItemHeight = 16;
			this.listBox2.Location = new System.Drawing.Point(17, 27);
			this.listBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.listBox2.Name = "listBox2";
			this.listBox2.Size = new System.Drawing.Size(529, 84);
			this.listBox2.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.groupBox1.Controls.Add(this.btn_clear);
			this.groupBox1.Controls.Add(this.listBox1);
			this.groupBox1.Controls.Add(this.button2);
			this.groupBox1.Controls.Add(this.btn_start);
			this.groupBox1.Location = new System.Drawing.Point(61, 46);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.groupBox1.Size = new System.Drawing.Size(608, 494);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "IMPINJ";
			// 
			// btn_clear
			// 
			this.btn_clear.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.btn_clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(222)));
			this.btn_clear.ForeColor = System.Drawing.Color.Snow;
			this.btn_clear.Location = new System.Drawing.Point(19, 143);
			this.btn_clear.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.btn_clear.Name = "btn_clear";
			this.btn_clear.Size = new System.Drawing.Size(75, 36);
			this.btn_clear.TabIndex = 3;
			this.btn_clear.Text = "Clear";
			this.btn_clear.UseVisualStyleBackColor = false;
			this.btn_clear.Click += new System.EventHandler(this.btn_clear_DoubleClick);
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 16;
			this.listBox1.Location = new System.Drawing.Point(116, 27);
			this.listBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(431, 420);
			this.listBox1.TabIndex = 2;
			// 
			// button2
			// 
			this.button2.AutoSize = true;
			this.button2.BackColor = System.Drawing.Color.Red;
			this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button2.ForeColor = System.Drawing.Color.White;
			this.button2.Location = new System.Drawing.Point(17, 90);
			this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(77, 38);
			this.button2.TabIndex = 1;
			this.button2.Text = "Stop";
			this.button2.UseVisualStyleBackColor = false;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// btn_start
			// 
			this.btn_start.BackColor = System.Drawing.Color.MediumSeaGreen;
			this.btn_start.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btn_start.ForeColor = System.Drawing.Color.White;
			this.btn_start.Location = new System.Drawing.Point(17, 38);
			this.btn_start.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.btn_start.Name = "btn_start";
			this.btn_start.Size = new System.Drawing.Size(77, 34);
			this.btn_start.TabIndex = 0;
			this.btn_start.Text = "Start";
			this.btn_start.UseVisualStyleBackColor = false;
			this.btn_start.Click += new System.EventHandler(this.button1_Click);
			// 
			// notifyIcon
			// 
			this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.notifyIcon.BalloonTipText = "RFID READER";
			this.notifyIcon.BalloonTipTitle = "program runing on background";
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Text = "notifyIcon";
			this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
			// 
			// status_reader
			// 
			this.status_reader.BackColor = System.Drawing.Color.Red;
			this.status_reader.ForeColor = System.Drawing.Color.White;
			this.status_reader.Location = new System.Drawing.Point(449, 9);
			this.status_reader.Name = "status_reader";
			this.status_reader.Size = new System.Drawing.Size(108, 28);
			this.status_reader.TabIndex = 3;
			this.status_reader.Text = "Disconnect";
			this.status_reader.UseVisualStyleBackColor = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(107, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(84, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "WebSocket :";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(373, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "READER :";
			// 
			// status_segnalr
			// 
			this.status_segnalr.BackColor = System.Drawing.Color.Red;
			this.status_segnalr.ForeColor = System.Drawing.Color.White;
			this.status_segnalr.Location = new System.Drawing.Point(195, 12);
			this.status_segnalr.Name = "status_segnalr";
			this.status_segnalr.Size = new System.Drawing.Size(108, 28);
			this.status_segnalr.TabIndex = 2;
			this.status_segnalr.Text = "Disconnect";
			this.status_segnalr.UseVisualStyleBackColor = false;
			// 
			// btn_con_signalr
			// 
			this.btn_con_signalr.BackColor = System.Drawing.Color.Transparent;
			this.btn_con_signalr.BackgroundImage = global::Properties.Resources.redo_7_256x256;
			this.btn_con_signalr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.btn_con_signalr.FlatAppearance.BorderColor = System.Drawing.Color.Red;
			this.btn_con_signalr.FlatAppearance.BorderSize = 2;
			this.btn_con_signalr.Location = new System.Drawing.Point(308, 14);
			this.btn_con_signalr.Name = "btn_con_signalr";
			this.btn_con_signalr.Size = new System.Drawing.Size(25, 24);
			this.btn_con_signalr.TabIndex = 6;
			this.btn_con_signalr.UseVisualStyleBackColor = false;
			this.btn_con_signalr.Click += new System.EventHandler(this.btn_con_signalr_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ClientSize = new System.Drawing.Size(695, 705);
			this.Controls.Add(this.btn_con_signalr);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.status_reader);
			this.Controls.Add(this.status_segnalr);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "RFID Reader";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Resize += new System.EventHandler(this.Form1_Resize);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Button btn_clear;
		private System.Windows.Forms.Button status_reader;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btn_con_signalr;
		private System.Windows.Forms.Button status_segnalr;
	}
}

