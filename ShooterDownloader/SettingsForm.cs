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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using ShooterDownloader.Properties;

namespace ShooterDownloader
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();

			lblTitleVersion.Text = $"{Resources.InfoTitle} {Application.ProductVersion}";

			for(var i = 1; i <= ShooterConst.MaxConcurrentJobs; i++)
				cbConcurrenctNum.Items.Add(i);

			for(int i = ShooterConst.MinHttpTimeout; i <= ShooterConst.MaxHttpTimeout;
				i += ShooterConst.HttpTimeoutIncrement)
				cbHttpTimeout.Items.Add(i);

			UpdateShellExtButton();
		}

		//Indicate at least one of the setting
		public bool IsDirty { get; private set; }

		public bool IsShellExtRegistered {
			get {
				RegistryKey hkcr = Registry.ClassesRoot;
				string subkey = $"CLSID\\{Settings.Default.ShellExtClsid}";
				try
				{
					if(hkcr.OpenSubKey(subkey, false) == null)
						return false;
				}
				catch(Exception)
				{
					return false;
				}
				hkcr.Close();
				return true;
			}
		}

		private void SettingsForm_Load(object sender, EventArgs e)
		{
			//Load settings and check value format.

			//1 <= concurrentNum <= ShooterConst.MaxConcurrentJobs
			int concurrentNum =
				Util.GetGetBoundedValue(Settings.Default.MaxConcurrentJobs,
					1, ShooterConst.MaxConcurrentJobs);
			SelectCbItemByValue(cbConcurrenctNum, concurrentNum);

			int maxHttpTimeout = Util.GetGetBoundedValue(Settings.Default.HttpTimeout, ShooterConst.MinHttpTimeout, ShooterConst.MaxHttpTimeout);
			maxHttpTimeout = maxHttpTimeout - (maxHttpTimeout % 10);
			SelectCbItemByValue(cbHttpTimeout, maxHttpTimeout);

			txtVideoFileExt.Text = Settings.Default.VideoFileExt;

			chkEnableLog.Checked = Settings.Default.EnableLog;

			chkEnableConvert.Checked = Settings.Default.AutoChsToChtConversion;

			if(concurrentNum != Settings.Default.MaxConcurrentJobs)
				IsDirty = true;
			else if(maxHttpTimeout != Settings.Default.HttpTimeout)
				IsDirty = true;
			else
				IsDirty = false;

			Util.AddShieldToButton(btnEnableShellExt);
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			if(IsDirty)
			{
				Settings.Default.MaxConcurrentJobs = GetCbSelectedValueInt(cbConcurrenctNum, 1);
				Settings.Default.VideoFileExt = txtVideoFileExt.Text;
				Settings.Default.EnableLog = chkEnableLog.Checked;
				Settings.Default.AutoChsToChtConversion = chkEnableConvert.Checked;
				Settings.Default.HttpTimeout = GetCbSelectedValueInt(cbHttpTimeout, ShooterConst.MaxHttpTimeout);
				Settings.Default.Save();
			}

			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btnOpenLogFolder_Click(object sender, EventArgs e)
		{
			string logDir = FileLogger.LogDirectory;
			if(!Directory.Exists(logDir))
				Directory.CreateDirectory(logDir);

			Process.Start(logDir);
		}

		private void cbConcurrenctNum_SelectedIndexChanged(object sender, EventArgs e) => IsDirty = true;
		private void cbHttpTimeout_SelectedIndexChanged(object sender, EventArgs e) => IsDirty = true;
		private void txtVideoFileExt_TextChanged(object sender, EventArgs e) => IsDirty = true;
		private void chkEnableLog_CheckedChanged(object sender, EventArgs e) => IsDirty = true;
		private void chkEnableConvert_CheckedChanged(object sender, EventArgs e) => IsDirty = true;

		private void btnEnableShellExt_Click(object sender, EventArgs e)
		{
			string shellExtPath = Util.Is64BitOS ? $"{Application.StartupPath}\\{Settings.Default.ShellExtFileNameX64}" : $"{Application.StartupPath}\\{Settings.Default.ShellExtFileName}";

			if(!IsShellExtRegistered)
			{
				if(Util.RegisterDll(shellExtPath))
				{
					MessageBox.Show(
						Resources.InfoEnableShellExtOk,
						Resources.InfoTitle,
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show(
						Resources.ErrEnableShellExt,
						Resources.InfoTitle,
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
			}
			else
			{
				if(Util.UnregisterDll(shellExtPath))
				{
					MessageBox.Show(
						Resources.InfoDisableShellExtOk,
						Resources.InfoTitle,
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show(
						Resources.ErrDisableShellExt,
						Resources.InfoTitle,
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
			}

			UpdateShellExtButton();
		}

		private void UpdateShellExtButton() => btnEnableShellExt.Text = IsShellExtRegistered ? Resources.UiDisableShellExt : Resources.UiEnableShellExt;

		private static void SelectCbItemByValue(ComboBox cb, object value)
		{
			for(var i = 0; i < cb.Items.Count; i++)
			{
				if(cb.Items[i].ToString() == value.ToString())
				{
					cb.SelectedIndex = i;
					return;
				}
			}
		}

		private static int GetCbSelectedValueInt(ComboBox cb, int defaultValue)
		{
			int value;
			try
			{
				value = int.Parse(cb.Text);
			}
			catch(Exception)
			{
				value = defaultValue;
			}

			return value;
		}
	}
}