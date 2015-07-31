namespace ShooterDownloader
{
    partial class DownloadForm
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadForm));
			this.txtDir = new System.Windows.Forms.TextBox();
			this.btnSelectDir = new System.Windows.Forms.Button();
			this.dgvFileList = new System.Windows.Forms.DataGridView();
			this.CheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.FileNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.StatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FullPathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.btnStartBatch = new System.Windows.Forms.Button();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolDownloadMessage = new System.Windows.Forms.ToolStripStatusLabel();
			this.btnSelectAll = new System.Windows.Forms.Button();
			this.btnSelectNone = new System.Windows.Forms.Button();
			this.btnSettings = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dgvFileList)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtDir
			// 
			this.txtDir.Location = new System.Drawing.Point(12, 12);
			this.txtDir.Name = "txtDir";
			this.txtDir.ReadOnly = true;
			this.txtDir.Size = new System.Drawing.Size(242, 23);
			this.txtDir.TabIndex = 5;
			// 
			// btnSelectDir
			// 
			this.btnSelectDir.Location = new System.Drawing.Point(260, 9);
			this.btnSelectDir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnSelectDir.Name = "btnSelectDir";
			this.btnSelectDir.Size = new System.Drawing.Size(100, 28);
			this.btnSelectDir.TabIndex = 6;
			this.btnSelectDir.Text = "选择目录...";
			this.btnSelectDir.UseVisualStyleBackColor = true;
			this.btnSelectDir.Click += new System.EventHandler(this.btnSelectDir_Click);
			// 
			// dgvFileList
			// 
			this.dgvFileList.AllowUserToAddRows = false;
			this.dgvFileList.AllowUserToDeleteRows = false;
			this.dgvFileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvFileList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvFileList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvFileList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CheckBoxColumn,
            this.FileNameColumn,
            this.StatusColumn,
            this.FullPathColumn});
			this.dgvFileList.Location = new System.Drawing.Point(12, 45);
			this.dgvFileList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.dgvFileList.Name = "dgvFileList";
			this.dgvFileList.RowHeadersVisible = false;
			this.dgvFileList.RowTemplate.Height = 24;
			this.dgvFileList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvFileList.Size = new System.Drawing.Size(460, 452);
			this.dgvFileList.TabIndex = 8;
			// 
			// CheckBoxColumn
			// 
			this.CheckBoxColumn.FillWeight = 20F;
			this.CheckBoxColumn.HeaderText = "";
			this.CheckBoxColumn.Name = "CheckBoxColumn";
			this.CheckBoxColumn.Width = 55;
			// 
			// FileNameColumn
			// 
			this.FileNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.FileNameColumn.FillWeight = 80F;
			this.FileNameColumn.HeaderText = "文件名";
			this.FileNameColumn.Name = "FileNameColumn";
			this.FileNameColumn.ReadOnly = true;
			// 
			// StatusColumn
			// 
			this.StatusColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.StatusColumn.FillWeight = 20F;
			this.StatusColumn.HeaderText = "状态";
			this.StatusColumn.Name = "StatusColumn";
			this.StatusColumn.ReadOnly = true;
			// 
			// FullPathColumn
			// 
			this.FullPathColumn.HeaderText = "";
			this.FullPathColumn.Name = "FullPathColumn";
			this.FullPathColumn.Visible = false;
			this.FullPathColumn.Width = 5;
			// 
			// btnStartBatch
			// 
			this.btnStartBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStartBatch.Location = new System.Drawing.Point(352, 505);
			this.btnStartBatch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnStartBatch.Name = "btnStartBatch";
			this.btnStartBatch.Size = new System.Drawing.Size(120, 28);
			this.btnStartBatch.TabIndex = 16;
			this.btnStartBatch.Text = "下载";
			this.btnStartBatch.UseVisualStyleBackColor = true;
			this.btnStartBatch.Click += new System.EventHandler(this.btnStartBatch_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolDownloadMessage});
			this.statusStrip1.Location = new System.Drawing.Point(0, 539);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
			this.statusStrip1.Size = new System.Drawing.Size(484, 22);
			this.statusStrip1.TabIndex = 18;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolDownloadMessage
			// 
			this.toolDownloadMessage.Name = "toolDownloadMessage";
			this.toolDownloadMessage.Size = new System.Drawing.Size(72, 17);
			this.toolDownloadMessage.Text = "请选择目录";
			// 
			// btnSelectAll
			// 
			this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnSelectAll.Location = new System.Drawing.Point(12, 505);
			this.btnSelectAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnSelectAll.Name = "btnSelectAll";
			this.btnSelectAll.Size = new System.Drawing.Size(100, 28);
			this.btnSelectAll.TabIndex = 19;
			this.btnSelectAll.Text = "全选";
			this.btnSelectAll.UseVisualStyleBackColor = true;
			this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
			// 
			// btnSelectNone
			// 
			this.btnSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnSelectNone.Location = new System.Drawing.Point(120, 505);
			this.btnSelectNone.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnSelectNone.Name = "btnSelectNone";
			this.btnSelectNone.Size = new System.Drawing.Size(100, 28);
			this.btnSelectNone.TabIndex = 20;
			this.btnSelectNone.Text = "全不选";
			this.btnSelectNone.UseVisualStyleBackColor = true;
			this.btnSelectNone.Click += new System.EventHandler(this.btnSelectNone_Click);
			// 
			// btnSettings
			// 
			this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSettings.Location = new System.Drawing.Point(372, 9);
			this.btnSettings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnSettings.Name = "btnSettings";
			this.btnSettings.Size = new System.Drawing.Size(100, 28);
			this.btnSettings.TabIndex = 21;
			this.btnSettings.Text = "配置";
			this.btnSettings.UseVisualStyleBackColor = true;
			this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
			// 
			// DownloadForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ClientSize = new System.Drawing.Size(484, 561);
			this.Controls.Add(this.btnSettings);
			this.Controls.Add(this.btnSelectDir);
			this.Controls.Add(this.txtDir);
			this.Controls.Add(this.btnSelectNone);
			this.Controls.Add(this.btnSelectAll);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.btnStartBatch);
			this.Controls.Add(this.dgvFileList);
			this.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "DownloadForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "射手网字幕下载工具";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DownloadForm_FormClosed);
			this.Load += new System.EventHandler(this.DownloadForm_Load);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DownloadForm_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DownloadForm_DragEnter);
			((System.ComponentModel.ISupportInitialize)(this.dgvFileList)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.Button btnSelectDir;
        private System.Windows.Forms.DataGridView dgvFileList;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button btnStartBatch;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolDownloadMessage;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSettings;
		private System.Windows.Forms.DataGridViewCheckBoxColumn CheckBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn FileNameColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn FullPathColumn;
	}
}

