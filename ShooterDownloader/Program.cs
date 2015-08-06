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
using System.IO;
using System.Windows.Forms;
using ShooterDownloader.Properties;

namespace ShooterDownloader
{
	internal static class Program
	{
		/// <summary>The main entry point for the application.</summary>
		[STAThread]
		private static void Main(string[] args)
		{
			ArgMan.Instance.ParseArgs(args);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if(ArgMan.Instance.CodeConversionOnly && ArgMan.Instance.Files != null)
			{
				foreach(string filePath in ArgMan.Instance.Files)
				{
					if(File.Exists(filePath))
					{
						string backupFilePath = $"{filePath}.bak";
						//Cleanup existing backup file.
						if(File.Exists(backupFilePath))
							File.Delete(backupFilePath);
						File.Move(filePath, backupFilePath);

						//If no conversion happened, restore the file.
						Util.ConversionResult ret;
						if(Settings.Default.AutoChsToChtConversion)
							ret = Util.ConvertChsToCht(backupFilePath, filePath, false);
						else
							ret = Util.ConvertChtToChs(backupFilePath, filePath, false);
						if(ret == Util.ConversionResult.NoConversion || ret == Util.ConversionResult.Error)
						{
							if(File.Exists(filePath))
								File.Delete(filePath);

							File.Move(backupFilePath, filePath);
						}
					}
				}

				MessageBox.Show(Resources.InfoEasyConversionOk, Resources.InfoTitle);
			}
			else
				Application.Run(new DownloadForm());
		}
	}
}