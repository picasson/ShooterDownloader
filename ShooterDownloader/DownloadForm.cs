/*
 *   Shooter Subtitle Downloader: Automatic Subtitle Downloader for the http://shooter.cn.
 *   Copyright (C) 2009  John Fung
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU Affero General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU Affero General Public License for more details.
 *
 *   You should have received a copy of the GNU Affero General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShooterDownloader.Properties;

namespace ShooterDownloader
{
	public partial class DownloadForm : Form, ILogger
	{
		private readonly int _currentMaxJobs = 0;
		private readonly SettingsForm _settingsForm;
		private volatile UIStatus _currentUIStatus = UIStatus.StandBy;
		private ILogger _fileLogger;
		private JobQueue _jobQueue;
		private string _lastDir = string.Empty;
		private string _videoFileExt = string.Empty;

		public DownloadForm()
		{
			InitializeComponent();

			LogMan.Instance.RegisterLogger(this);

			Upgrade();

			_settingsForm = new SettingsForm();
			LoadSettings();
		}

		private void DownloadForm_Load(object sender, EventArgs e)
		{
			if(ArgMan.Instance.Files != null &&
				ArgMan.Instance.Files.Length > 0)
				SelectPaths(ArgMan.Instance.Files);
		}

		private void Upgrade()
		{
			if(Settings.Default.FirstRun)
			{
				Settings.Default.Upgrade();
				Settings.Default.FirstRun = false;
				Settings.Default.Save();
			}
		}

		//Log implementation
		private void SetLogText(string message) { toolDownloadMessage.Text = message; }

		private void LoadSettings()
		{
			if(_fileLogger == null && Settings.Default.EnableLog)
			{
				_fileLogger = new FileLogger();
				LogMan.Instance.RegisterLogger(_fileLogger);
			}
			else if(_fileLogger != null && !Settings.Default.EnableLog)
			{
				LogMan.Instance.RemoveLogger(_fileLogger);
				_fileLogger.Close();
				_fileLogger = null;
			}

			_lastDir = Settings.Default.LastDir;
			_videoFileExt = Settings.Default.VideoFileExt;

			int maxJobs = Settings.Default.MaxConcurrentJobs;
			if(maxJobs != _currentMaxJobs)
			{
				if(_jobQueue != null)
				{
					//_jobQueue.Close();
					_jobQueue.Reset(maxJobs);
				}
				else
				{
					_jobQueue = new JobQueue(maxJobs);
					_jobQueue.AllDone += AllDownloadComplete;
					_jobQueue.Start();
				}
			}
		}

		private void UpdateLastDir(string lastDir)
		{
			Settings.Default.LastDir = lastDir;
			Settings.Default.Save();
			_lastDir = lastDir;
		}

		private void DownloadForm_FormClosed(object sender, FormClosedEventArgs e) { _jobQueue.Close(); }

		private void btnSelectDir_Click(object sender, EventArgs e)
		{
			if(_lastDir != string.Empty && Directory.Exists(_lastDir))
				folderBrowserDialog.SelectedPath = _lastDir;
			if(folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				UpdateLastDir(folderBrowserDialog.SelectedPath);
				txtDir.Text = folderBrowserDialog.SelectedPath;

				PopulateFileList(txtDir.Text);
			}
		}

		private void PopulateFileList(string dir) { PopulateFileList(dir, null); }

		private void PopulateFileList(string dir, string[] fileList)
		{
			//list files in the selected directory.
			var dirInfo = new DirectoryInfo(dir);
			FileInfo[] fileInfoList = dirInfo.GetFiles();
			dgvFileList.Rows.Clear();

			Dictionary<string, Dummy> fileDic = null;

			if(fileList != null)
			{
				fileDic = new Dictionary<string, Dummy>();
				foreach(string file in fileList)
				{
					try
					{
						fileDic.Add(file, new Dummy());
					}
					catch(Exception e)
					{
						LogMan.Instance.Log(e.Message);
					}
				}
			}

			foreach(FileInfo fileInfo in fileInfoList)
			{
				var selected = false;
				var isVideo = false;

				if(fileDic != null)
				{
					//Select user specified files.
					if(fileDic.ContainsKey(fileInfo.FullName))
						selected = true;
				}

				if(_videoFileExt.Contains(fileInfo.Extension))
				{
					isVideo = true;

					if(fileDic == null)
					{
						//If user didn't specify which file to select,
						// auto-select all video files.
						selected = true;
					}
				}

				object[] newRow = { selected, fileInfo.Name, "", fileInfo.FullName };
				int rowIdx = dgvFileList.Rows.Add(newRow);

				if(isVideo)
				{
					//change the background color of video files
					DataGridViewRow dataRow = dgvFileList.Rows[rowIdx];
					dataRow.DefaultCellStyle.BackColor = Color.AntiqueWhite;
				}
			}
		}

		private void OnDownloadStatusUpdate(object sender, int progress)
		{
			int jobId = ((IJob)sender).JobId;
			if(progress == ShooterConst.Error)
				dgvFileList.Rows[jobId].Cells["StatusColumn"].Value = Resources.ProgressError;
			else if(progress == ShooterConst.NoSubFound)
				dgvFileList.Rows[jobId].Cells["StatusColumn"].Value = Resources.ProgressNoSub;
			else
			{
				dgvFileList.Rows[jobId].Cells["StatusColumn"].Value = progress + "%";

				//If download is completed.
				if(progress == 100)
				{
					//Uncheck the column.
					dgvFileList.Rows[jobId].Cells["CheckBoxColumn"].Value = false;
					//Makes it easier to resume the download if it's canceled by user.
				}
			}
		}

		private void btnStartBatch_Click(object sender, EventArgs e)
		{
			if(_currentUIStatus == UIStatus.StandBy)
			{
				var shouldDisableInput = true;

				foreach(DataGridViewRow row in dgvFileList.Rows)
				{
					if((bool)row.Cells["CheckBoxColumn"].Value)
					{
						if(shouldDisableInput)
						{
							//disable input if there is at least one checked column.
							//DisableInput();
							ChangeUIStatus(UIStatus.Downloading);
							ClearDownloadStatus();
							shouldDisableInput = false;
						}
						var filePath = (string)row.Cells["FullPathColumn"].Value;

						var dlJob = new ShooterDownloadJob();
						dlJob.VideoFilePath = filePath;
						dlJob.JobId = row.Index;
						dlJob.ProgressUpdate += OnDownloadStatusUpdate;
						_jobQueue.AddJob(dlJob);
					}
				}
			}
			else if(_currentUIStatus == UIStatus.Downloading)
			{
				//Cancel Download
				ChangeUIStatus(UIStatus.Canceling);
				_jobQueue.Reset();
				ChangeUIStatus(UIStatus.StandBy);
				LogMan.Instance.Log(Resources.InfoDownloadCanceled);
			}
		}

		private void btnSelectAll_Click(object sender, EventArgs e)
		{
			foreach(DataGridViewRow row in dgvFileList.Rows)
				row.Cells["CheckBoxColumn"].Value = true;
		}

		private void btnSelectNone_Click(object sender, EventArgs e)
		{
			foreach(DataGridViewRow row in dgvFileList.Rows)
				row.Cells["CheckBoxColumn"].Value = false;
		}

		private void btnSettings_Click(object sender, EventArgs e)
		{
			if(_settingsForm.ShowDialog(this) == DialogResult.OK && _settingsForm.IsDirty)
				LoadSettings();
		}

		private void AllDownloadComplete()
		{
			LogMan.Instance.Log(Resources.InfoAllDownloadOk);

			if(InvokeRequired)
			{
				//EnableInputCallback d = new EnableInputCallback(EnableInput);
				ChangeUIStatusCallBack d = ChangeUIStatus;
				//this.Invoke(d);
				Invoke(d, UIStatus.StandBy);
			}
			else
			{
				//EnableInput();
				ChangeUIStatus(UIStatus.StandBy);
			}
		}

		private void ChangeUIStatus(UIStatus uiStatus)
		{
			if(uiStatus == _currentUIStatus)
				return;

			switch(uiStatus)
			{
				case UIStatus.StandBy:
					btnSelectDir.Enabled = true;
					btnSettings.Enabled = true;
					dgvFileList.Columns["CheckBoxColumn"].ReadOnly = false;
					btnSelectAll.Enabled = true;
					btnSelectNone.Enabled = true;
					btnStartBatch.Enabled = true;
					btnStartBatch.Text = Resources.UiStartDownload;
					Cursor = Cursors.Default;
					foreach(Control ctrl in Controls)
						ctrl.Cursor = Cursors.Default;
					break;
				case UIStatus.Downloading:
					btnSelectDir.Enabled = false;
					btnSettings.Enabled = false;
					dgvFileList.Columns["CheckBoxColumn"].ReadOnly = true;
					btnSelectAll.Enabled = false;
					btnSelectNone.Enabled = false;
					//btnStartBatch.Enabled = false;
					btnStartBatch.Text = Resources.UiCancelDownload;
					Cursor = Cursors.WaitCursor;
					foreach(Control ctrl in Controls)
						ctrl.Cursor = Cursors.WaitCursor;
					break;
				case UIStatus.Canceling:
					btnSelectDir.Enabled = false;
					btnSettings.Enabled = false;
					dgvFileList.Columns["CheckBoxColumn"].ReadOnly = true;
					btnSelectAll.Enabled = false;
					btnSelectNone.Enabled = false;
					btnStartBatch.Enabled = false;
					btnStartBatch.Text = Resources.UiCanceling;
					Cursor = Cursors.WaitCursor;
					foreach(Control ctrl in Controls)
						ctrl.Cursor = Cursors.WaitCursor;
					break;
			}

			_currentUIStatus = uiStatus;
		}

		//private void EnableInput()
		//{
		//    btnSelectDir.Enabled = true;
		//    btnSettings.Enabled = true;
		//    dgvFileList.Columns["CheckBoxColumn"].ReadOnly = false;
		//    btnSelectAll.Enabled = true;
		//    btnSelectNone.Enabled = true;
		//    btnStartBatch.Enabled = true;
		//    this.Cursor = Cursors.Default;
		//    foreach (Control ctrl in this.Controls)
		//    {
		//        ctrl.Cursor = Cursors.Default;
		//    }
		//}

		//private void DisableInput()
		//{
		//    btnSelectDir.Enabled = false;
		//    btnSettings.Enabled = false;
		//    dgvFileList.Columns["CheckBoxColumn"].ReadOnly = true;
		//    btnSelectAll.Enabled = false;
		//    btnSelectNone.Enabled = false;
		//    btnStartBatch.Enabled = false;
		//    this.Cursor = Cursors.WaitCursor;
		//    foreach (Control ctrl in this.Controls)
		//    {
		//        ctrl.Cursor = Cursors.WaitCursor;
		//    }
		//}

		private void ClearDownloadStatus()
		{
			foreach(DataGridViewRow row in dgvFileList.Rows)
				row.Cells["StatusColumn"].Value = string.Empty;
		}

		private void DownloadForm_DragEnter(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void DownloadForm_DragDrop(object sender, DragEventArgs e)
		{
			var paths = (string[])(e.Data.GetData(DataFormats.FileDrop));

			if(paths.Length == 0)
				return;

			SelectPaths(paths);
		}

		private void SelectPaths(string[] paths)
		{
			//If the first object is a directory.
			if(Directory.Exists(paths[0]))
			{
				//Can only handle 1 directory
				txtDir.Text = paths[0];
				PopulateFileList(txtDir.Text);
			}
			else
			{
				string dir = null;
				var fileList = new List<string>();
				foreach(string path in paths)
				{
					if(File.Exists(path))
					{
						if(dir == null)
						{
							//Get the directory of the first file.
							dir = Path.GetDirectoryName(path);
						}
						else
						{
							//Ignore following files if they are not in the same directory.
							if(Path.GetDirectoryName(path) != dir)
								continue;
						}

						fileList.Add(path);
					}
				}

				txtDir.Text = dir;
				PopulateFileList(dir, fileList.ToArray());
			}
		}

		private delegate void SetLogTextCallback(string msg);

		//private delegate void EnableInputCallback();
		private delegate void ChangeUIStatusCallBack(UIStatus uiStatus);

		private struct Dummy { };

		private enum UIStatus
		{
			StandBy,
			Downloading,
			Canceling
		}

		#region ILogger Members
		public void Log(string message)
		{
			if(InvokeRequired)
			{
				SetLogTextCallback d = SetLogText;
				Invoke(d, message);
			}
			else
				SetLogText(message);
		}

		public void Log(string message, params object[] args)
		{
			string formatMessage = string.Format(message, args);

			//Write log while the JobQueue is canceling will cause a deadlock.
			if(InvokeRequired && _currentUIStatus != UIStatus.Canceling)
			{
				SetLogTextCallback d = SetLogText;
				Invoke(d, formatMessage);
			}
			else
				SetLogText(formatMessage);
		}

		void ILogger.Close()
		{
			//no need to release anything
		}
		#endregion
	}
}