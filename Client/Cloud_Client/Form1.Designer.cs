namespace Cloud_Client
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
            this.ipBox = new System.Windows.Forms.TextBox();
            this.portBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.getfilesButton = new System.Windows.Forms.Button();
            this.downloadButton = new System.Windows.Forms.Button();
            this.reportBox = new System.Windows.Forms.RichTextBox();
            this.ipLabel = new System.Windows.Forms.Label();
            this.portLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nickBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.renameGroupBox = new System.Windows.Forms.GroupBox();
            this.renameButton = new System.Windows.Forms.Button();
            this.newFileNameBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.oldFileNameBox = new System.Windows.Forms.TextBox();
            this.FileDataList = new System.Windows.Forms.ListView();
            this.nameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sizeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.typeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.progressColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.uploadButton = new System.Windows.Forms.Button();
            this.deleteGroupBox = new System.Windows.Forms.GroupBox();
            this.deleteTypingButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.deleteTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.renameGroupBox.SuspendLayout();
            this.deleteGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ipBox
            // 
            this.ipBox.Location = new System.Drawing.Point(50, 20);
            this.ipBox.Name = "ipBox";
            this.ipBox.Size = new System.Drawing.Size(120, 22);
            this.ipBox.TabIndex = 0;
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(50, 48);
            this.portBox.MaxLength = 5;
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(120, 22);
            this.portBox.TabIndex = 1;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(179, 19);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(101, 51);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // getfilesButton
            // 
            this.getfilesButton.Location = new System.Drawing.Point(314, 405);
            this.getfilesButton.Name = "getfilesButton";
            this.getfilesButton.Size = new System.Drawing.Size(146, 32);
            this.getfilesButton.TabIndex = 5;
            this.getfilesButton.Text = "Get All Files";
            this.getfilesButton.UseVisualStyleBackColor = true;
            this.getfilesButton.Click += new System.EventHandler(this.getfilesButton_Click);
            // 
            // downloadButton
            // 
            this.downloadButton.Location = new System.Drawing.Point(831, 405);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(146, 32);
            this.downloadButton.TabIndex = 4;
            this.downloadButton.Text = "Download File";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // reportBox
            // 
            this.reportBox.Location = new System.Drawing.Point(12, 126);
            this.reportBox.Name = "reportBox";
            this.reportBox.ReadOnly = true;
            this.reportBox.Size = new System.Drawing.Size(291, 269);
            this.reportBox.TabIndex = 15;
            this.reportBox.Text = "";
            this.reportBox.TextChanged += new System.EventHandler(this.reportBox_TextChanged);
            // 
            // ipLabel
            // 
            this.ipLabel.AutoSize = true;
            this.ipLabel.Location = new System.Drawing.Point(21, 23);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(20, 17);
            this.ipLabel.TabIndex = 7;
            this.ipLabel.Text = "IP";
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(7, 50);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(34, 17);
            this.portLabel.TabIndex = 8;
            this.portLabel.Text = "Port";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nickBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ipLabel);
            this.groupBox1.Controls.Add(this.portLabel);
            this.groupBox1.Controls.Add(this.portBox);
            this.groupBox1.Controls.Add(this.ipBox);
            this.groupBox1.Controls.Add(this.connectButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(291, 110);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // nickBox
            // 
            this.nickBox.Location = new System.Drawing.Point(50, 76);
            this.nickBox.MaxLength = 2000;
            this.nickBox.Name = "nickBox";
            this.nickBox.Size = new System.Drawing.Size(231, 22);
            this.nickBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "Nick";
            // 
            // renameGroupBox
            // 
            this.renameGroupBox.Controls.Add(this.renameButton);
            this.renameGroupBox.Controls.Add(this.newFileNameBox);
            this.renameGroupBox.Controls.Add(this.label3);
            this.renameGroupBox.Controls.Add(this.label2);
            this.renameGroupBox.Controls.Add(this.oldFileNameBox);
            this.renameGroupBox.Location = new System.Drawing.Point(315, 5);
            this.renameGroupBox.Name = "renameGroupBox";
            this.renameGroupBox.Size = new System.Drawing.Size(328, 110);
            this.renameGroupBox.TabIndex = 10;
            this.renameGroupBox.TabStop = false;
            this.renameGroupBox.Text = "Rename a File";
            // 
            // renameButton
            // 
            this.renameButton.Location = new System.Drawing.Point(168, 78);
            this.renameButton.Name = "renameButton";
            this.renameButton.Size = new System.Drawing.Size(146, 24);
            this.renameButton.TabIndex = 18;
            this.renameButton.Text = "Rename File";
            this.renameButton.UseVisualStyleBackColor = true;
            this.renameButton.Click += new System.EventHandler(this.renameButton_Click);
            // 
            // newFileNameBox
            // 
            this.newFileNameBox.Location = new System.Drawing.Point(119, 50);
            this.newFileNameBox.Name = "newFileNameBox";
            this.newFileNameBox.Size = new System.Drawing.Size(195, 22);
            this.newFileNameBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "New File Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Old File Name:";
            // 
            // oldFileNameBox
            // 
            this.oldFileNameBox.Location = new System.Drawing.Point(119, 22);
            this.oldFileNameBox.Name = "oldFileNameBox";
            this.oldFileNameBox.Size = new System.Drawing.Size(195, 22);
            this.oldFileNameBox.TabIndex = 0;
            // 
            // FileDataList
            // 
            this.FileDataList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.sizeColumn,
            this.timeColumn,
            this.typeColumn,
            this.progressColumn});
            this.FileDataList.FullRowSelect = true;
            this.FileDataList.GridLines = true;
            this.FileDataList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.FileDataList.HideSelection = false;
            this.FileDataList.Location = new System.Drawing.Point(314, 126);
            this.FileDataList.MultiSelect = false;
            this.FileDataList.Name = "FileDataList";
            this.FileDataList.Size = new System.Drawing.Size(662, 269);
            this.FileDataList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.FileDataList.TabIndex = 16;
            this.FileDataList.UseCompatibleStateImageBehavior = false;
            this.FileDataList.View = System.Windows.Forms.View.Details;
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "FileName";
            this.nameColumn.Width = 190;
            // 
            // sizeColumn
            // 
            this.sizeColumn.Text = "Size (Bytes)";
            this.sizeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.sizeColumn.Width = 102;
            // 
            // timeColumn
            // 
            this.timeColumn.Text = "UploadTime";
            this.timeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.timeColumn.Width = 100;
            // 
            // typeColumn
            // 
            this.typeColumn.Text = "Type";
            this.typeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.typeColumn.Width = 100;
            // 
            // progressColumn
            // 
            this.progressColumn.Text = "Progress";
            this.progressColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.progressColumn.Width = 68;
            // 
            // uploadButton
            // 
            this.uploadButton.Enabled = false;
            this.uploadButton.Location = new System.Drawing.Point(13, 405);
            this.uploadButton.Name = "uploadButton";
            this.uploadButton.Size = new System.Drawing.Size(290, 32);
            this.uploadButton.TabIndex = 3;
            this.uploadButton.Text = "Upload New File";
            this.uploadButton.UseVisualStyleBackColor = true;
            this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
            // 
            // deleteGroupBox
            // 
            this.deleteGroupBox.Controls.Add(this.deleteTypingButton);
            this.deleteGroupBox.Controls.Add(this.label5);
            this.deleteGroupBox.Controls.Add(this.deleteTextBox);
            this.deleteGroupBox.Location = new System.Drawing.Point(649, 5);
            this.deleteGroupBox.Name = "deleteGroupBox";
            this.deleteGroupBox.Size = new System.Drawing.Size(328, 110);
            this.deleteGroupBox.TabIndex = 19;
            this.deleteGroupBox.TabStop = false;
            this.deleteGroupBox.Text = "Delete a File";
            // 
            // deleteTypingButton
            // 
            this.deleteTypingButton.Location = new System.Drawing.Point(168, 64);
            this.deleteTypingButton.Name = "deleteTypingButton";
            this.deleteTypingButton.Size = new System.Drawing.Size(146, 24);
            this.deleteTypingButton.TabIndex = 18;
            this.deleteTypingButton.Text = "Delete File";
            this.deleteTypingButton.UseVisualStyleBackColor = true;
            this.deleteTypingButton.Click += new System.EventHandler(this.deleteTypingButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 17);
            this.label5.TabIndex = 1;
            this.label5.Text = "File Name:";
            // 
            // deleteTextBox
            // 
            this.deleteTextBox.Location = new System.Drawing.Point(119, 33);
            this.deleteTextBox.Name = "deleteTextBox";
            this.deleteTextBox.Size = new System.Drawing.Size(195, 22);
            this.deleteTextBox.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 447);
            this.Controls.Add(this.deleteGroupBox);
            this.Controls.Add(this.getfilesButton);
            this.Controls.Add(this.FileDataList);
            this.Controls.Add(this.uploadButton);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.renameGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.reportBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.renameGroupBox.ResumeLayout(false);
            this.renameGroupBox.PerformLayout();
            this.deleteGroupBox.ResumeLayout(false);
            this.deleteGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox ipBox;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button getfilesButton;
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.RichTextBox reportBox;
        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox renameGroupBox;
        private System.Windows.Forms.Button uploadButton;
        private System.Windows.Forms.TextBox nickBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView FileDataList;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader sizeColumn;
        private System.Windows.Forms.ColumnHeader timeColumn;
        private System.Windows.Forms.ColumnHeader typeColumn;
        private System.Windows.Forms.ColumnHeader progressColumn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox oldFileNameBox;
        private System.Windows.Forms.Button renameButton;
        private System.Windows.Forms.TextBox newFileNameBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox deleteGroupBox;
        private System.Windows.Forms.Button deleteTypingButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox deleteTextBox;
    }
}

