namespace ShooterDownloader
{
    partial class SettingsForm
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
			this.chkEnableLog = new System.Windows.Forms.CheckBox();
			this.btnOpenLogFolder = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtVideoFileExt = new System.Windows.Forms.TextBox();
			this.cbConcurrenctNum = new System.Windows.Forms.ComboBox();
			this.chkEnableConvert = new System.Windows.Forms.CheckBox();
			this.lblTitleVersion = new System.Windows.Forms.Label();
			this.btnEnableShellExt = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.cbHttpTimeout = new System.Windows.Forms.ComboBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// chkEnableLog
			// 
			this.chkEnableLog.AutoSize = true;
			this.chkEnableLog.Location = new System.Drawing.Point(17, 108);
			this.chkEnableLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.chkEnableLog.Name = "chkEnableLog";
			this.chkEnableLog.Size = new System.Drawing.Size(75, 21);
			this.chkEnableLog.TabIndex = 0;
			this.chkEnableLog.Text = "启用日志";
			this.chkEnableLog.UseVisualStyleBackColor = true;
			this.chkEnableLog.CheckedChanged += new System.EventHandler(this.chkEnableLog_CheckedChanged);
			// 
			// btnOpenLogFolder
			// 
			this.btnOpenLogFolder.Location = new System.Drawing.Point(119, 103);
			this.btnOpenLogFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnOpenLogFolder.Name = "btnOpenLogFolder";
			this.btnOpenLogFolder.Size = new System.Drawing.Size(254, 28);
			this.btnOpenLogFolder.TabIndex = 4;
			this.btnOpenLogFolder.Text = "打开日志所在文件夹";
			this.btnOpenLogFolder.UseVisualStyleBackColor = true;
			this.btnOpenLogFolder.Click += new System.EventHandler(this.btnOpenLogFolder_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(92, 17);
			this.label1.TabIndex = 5;
			this.label1.Text = "最大同時下载数";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 75);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 17);
			this.label2.TabIndex = 6;
			this.label2.Text = "视频后缀名";
			// 
			// txtVideoFileExt
			// 
			this.txtVideoFileExt.Location = new System.Drawing.Point(119, 72);
			this.txtVideoFileExt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtVideoFileExt.Name = "txtVideoFileExt";
			this.txtVideoFileExt.Size = new System.Drawing.Size(254, 23);
			this.txtVideoFileExt.TabIndex = 8;
			this.txtVideoFileExt.TextChanged += new System.EventHandler(this.txtVideoFileExt_TextChanged);
			// 
			// cbConcurrenctNum
			// 
			this.cbConcurrenctNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbConcurrenctNum.FormattingEnabled = true;
			this.cbConcurrenctNum.Location = new System.Drawing.Point(120, 9);
			this.cbConcurrenctNum.Name = "cbConcurrenctNum";
			this.cbConcurrenctNum.Size = new System.Drawing.Size(253, 25);
			this.cbConcurrenctNum.TabIndex = 9;
			this.cbConcurrenctNum.SelectedIndexChanged += new System.EventHandler(this.cbConcurrenctNum_SelectedIndexChanged);
			// 
			// chkEnableConvert
			// 
			this.chkEnableConvert.AutoSize = true;
			this.chkEnableConvert.Location = new System.Drawing.Point(17, 139);
			this.chkEnableConvert.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.chkEnableConvert.Name = "chkEnableConvert";
			this.chkEnableConvert.Size = new System.Drawing.Size(171, 21);
			this.chkEnableConvert.TabIndex = 10;
			this.chkEnableConvert.Text = "自动将简体字幕转化成繁体";
			this.chkEnableConvert.UseVisualStyleBackColor = true;
			this.chkEnableConvert.CheckedChanged += new System.EventHandler(this.chkEnableConvert_CheckedChanged);
			// 
			// lblTitleVersion
			// 
			this.lblTitleVersion.AutoSize = true;
			this.lblTitleVersion.ForeColor = System.Drawing.SystemColors.GrayText;
			this.lblTitleVersion.Location = new System.Drawing.Point(9, 241);
			this.lblTitleVersion.Name = "lblTitleVersion";
			this.lblTitleVersion.Size = new System.Drawing.Size(80, 17);
			this.lblTitleVersion.TabIndex = 11;
			this.lblTitleVersion.Text = "Title Version";
			// 
			// btnEnableShellExt
			// 
			this.btnEnableShellExt.Location = new System.Drawing.Point(12, 168);
			this.btnEnableShellExt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnEnableShellExt.Name = "btnEnableShellExt";
			this.btnEnableShellExt.Size = new System.Drawing.Size(361, 28);
			this.btnEnableShellExt.TabIndex = 12;
			this.btnEnableShellExt.Text = "启用右键菜单";
			this.btnEnableShellExt.UseVisualStyleBackColor = true;
			this.btnEnableShellExt.Click += new System.EventHandler(this.btnEnableShellExt_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 43);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 17);
			this.label3.TabIndex = 13;
			this.label3.Text = "HTTP超时(秒)";
			// 
			// cbHttpTimeout
			// 
			this.cbHttpTimeout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbHttpTimeout.FormattingEnabled = true;
			this.cbHttpTimeout.Location = new System.Drawing.Point(120, 40);
			this.cbHttpTimeout.Name = "cbHttpTimeout";
			this.cbHttpTimeout.Size = new System.Drawing.Size(253, 25);
			this.cbHttpTimeout.TabIndex = 14;
			this.cbHttpTimeout.SelectedIndexChanged += new System.EventHandler(this.cbHttpTimeout_SelectedIndexChanged);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(253, 204);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(120, 28);
			this.btnCancel.TabIndex = 16;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(119, 204);
			this.btnOk.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(120, 28);
			this.btnOk.TabIndex = 15;
			this.btnOk.Text = "确定";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// SettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(384, 267);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.cbHttpTimeout);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnEnableShellExt);
			this.Controls.Add(this.lblTitleVersion);
			this.Controls.Add(this.chkEnableConvert);
			this.Controls.Add(this.cbConcurrenctNum);
			this.Controls.Add(this.txtVideoFileExt);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOpenLogFolder);
			this.Controls.Add(this.chkEnableLog);
			this.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "设置";
			this.Load += new System.EventHandler(this.SettingsForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkEnableLog;
        private System.Windows.Forms.Button btnOpenLogFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtVideoFileExt;
        private System.Windows.Forms.ComboBox cbConcurrenctNum;
        private System.Windows.Forms.CheckBox chkEnableConvert;
        private System.Windows.Forms.Label lblTitleVersion;
        private System.Windows.Forms.Button btnEnableShellExt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbHttpTimeout;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
	}
}