namespace Cloud_Server
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
            this.portBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.reportBox = new System.Windows.Forms.RichTextBox();
            this.onlineList = new System.Windows.Forms.ListBox();
            this.onlineLabel = new System.Windows.Forms.Label();
            this.start_groupBox = new System.Windows.Forms.GroupBox();
            this.reportLabel = new System.Windows.Forms.Label();
            this.start_groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(56, 26);
            this.portBox.MaxLength = 4;
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(220, 22);
            this.portBox.TabIndex = 0;
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(12, 26);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(38, 17);
            this.portLabel.TabIndex = 1;
            this.portLabel.Text = "Port:";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(100, 54);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(176, 23);
            this.startButton.TabIndex = 2;
            this.startButton.Text = "START SERVER";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // reportBox
            // 
            this.reportBox.Location = new System.Drawing.Point(12, 130);
            this.reportBox.Name = "reportBox";
            this.reportBox.ReadOnly = true;
            this.reportBox.Size = new System.Drawing.Size(294, 404);
            this.reportBox.TabIndex = 3;
            this.reportBox.Text = "";
            this.reportBox.TextChanged += new System.EventHandler(this.reportBox_TextChanged);
            // 
            // onlineList
            // 
            this.onlineList.FormattingEnabled = true;
            this.onlineList.ItemHeight = 16;
            this.onlineList.Location = new System.Drawing.Point(312, 34);
            this.onlineList.Name = "onlineList";
            this.onlineList.Size = new System.Drawing.Size(176, 500);
            this.onlineList.TabIndex = 4;
            // 
            // onlineLabel
            // 
            this.onlineLabel.AutoSize = true;
            this.onlineLabel.Location = new System.Drawing.Point(312, 10);
            this.onlineLabel.Name = "onlineLabel";
            this.onlineLabel.Size = new System.Drawing.Size(122, 17);
            this.onlineLabel.TabIndex = 5;
            this.onlineLabel.Text = "Connected Clients";
            // 
            // start_groupBox
            // 
            this.start_groupBox.Controls.Add(this.startButton);
            this.start_groupBox.Controls.Add(this.portBox);
            this.start_groupBox.Controls.Add(this.portLabel);
            this.start_groupBox.Location = new System.Drawing.Point(12, 5);
            this.start_groupBox.Name = "start_groupBox";
            this.start_groupBox.Size = new System.Drawing.Size(294, 96);
            this.start_groupBox.TabIndex = 6;
            this.start_groupBox.TabStop = false;
            // 
            // reportLabel
            // 
            this.reportLabel.AutoSize = true;
            this.reportLabel.Location = new System.Drawing.Point(13, 107);
            this.reportLabel.Name = "reportLabel";
            this.reportLabel.Size = new System.Drawing.Size(85, 17);
            this.reportLabel.TabIndex = 7;
            this.reportLabel.Text = "Server Logs";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 543);
            this.Controls.Add(this.reportLabel);
            this.Controls.Add(this.start_groupBox);
            this.Controls.Add(this.onlineLabel);
            this.Controls.Add(this.onlineList);
            this.Controls.Add(this.reportBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Server";
            this.start_groupBox.ResumeLayout(false);
            this.start_groupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.RichTextBox reportBox;
        private System.Windows.Forms.ListBox onlineList;
        private System.Windows.Forms.Label onlineLabel;
        private System.Windows.Forms.GroupBox start_groupBox;
        private System.Windows.Forms.Label reportLabel;
    }
}

