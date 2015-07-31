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
using System.Net;
using System.Text;
using ShooterDownloader.Properties;

namespace ShooterDownloader
{
	internal class ShooterDownloadJob : IJob
	{
		private readonly string _boundary;
		private readonly string _contentType;
		private readonly int _httpTimeout; //in milliseconds
		private readonly string _queryUrl;
		private readonly string _userAgent;
		private string _videoFileName = string.Empty;
		private string _videoFilePath = string.Empty;
		//ShooterNet
		public ShooterDownloadJob()
		{
			_queryUrl = Settings.Default.QueryUrl;
			_userAgent = Settings.Default.UserAgent;
			_contentType = Settings.Default.ContentType;
			_boundary = Settings.Default.Boundary;
			_httpTimeout = Settings.Default.HttpTimeout * 1000;
		}

		public string VideoFilePath {
			get { return _videoFilePath; }
			set {
				_videoFilePath = value;
				_videoFileName = Path.GetFileName(_videoFilePath);
			}
		}

		public void QuerySubByVideoPathOrHash(string filePath, string fileHash, string vHash, string langId)
		{
			UpdateProgress(0);

			var formData = new FormData();
			formData.Boundary = _boundary;
			formData.AddData("pathinfo", filePath);
			formData.AddData("filehash", fileHash);

			string formDataString = formData.ToString();

			byte[] formDataUtf8 = Encoding.UTF8.GetBytes(formDataString);
			var request = WebRequest.Create(_queryUrl) as HttpWebRequest;
			request.Method = "POST";
			request.UserAgent = _userAgent;
			request.ContentType = _contentType;
			request.ContentLength = formDataUtf8.Length;
			request.Timeout = _httpTimeout;
			Stream requestStream = null;
			var bReqOk = false;
			try
			{
				requestStream = request.GetRequestStream();
				requestStream.Write(formDataUtf8, 0, formDataUtf8.Length);
				requestStream.Flush();
				requestStream.Close();
				bReqOk = true;
			}
			catch(Exception ex)
			{
				UpdateProgress(ShooterConst.Error);
				LogMan.Instance.Log(Resources.ErrHttpRequest, _videoFileName, ex.Message);
			}
			finally
			{
				requestStream?.Close();
			}

			string tempFilePath = null;
			var bRespOk = false;

			if(bReqOk)
			{
				UpdateProgress(50);

				HttpWebResponse response = null;
				Stream responseStream = null;
				FileStream outputStream = null;

				tempFilePath = Path.GetTempFileName();

				try
				{
					response = request.GetResponse() as HttpWebResponse;
					if(response.StatusCode == HttpStatusCode.OK)
					{
						responseStream = response.GetResponseStream();
						var buffer = new byte[1024];
						var len = 0;
						outputStream = new FileStream(tempFilePath, FileMode.OpenOrCreate);
						while((len = responseStream.Read(buffer, 0, buffer.Length)) != 0)
							outputStream.Write(buffer, 0, len);
						bRespOk = true;
					}
					else
						LogMan.Instance.Log(Resources.ErrHttpResponse, _videoFileName, response.StatusCode.ToString());
				}
				catch(Exception ex)
				{
					UpdateProgress(ShooterConst.Error);
					LogMan.Instance.Log(Resources.ErrHttpResponse, _videoFileName, ex.Message);
				}
				finally
				{
					outputStream?.Close();
					response?.Close();
					responseStream?.Close();
				}
			}

			if(bRespOk)
			{
				//Update progress
				UpdateProgress(90);

				//Extract subtitle
				var extractor = new ShooterSubExtractor {
					VideoFilePath = filePath,
					DumpFilePath = tempFilePath
				};
				ShooterSubExtractor.SubExtractResult result = extractor.ExtractSubtitles();

				if(result == ShooterSubExtractor.SubExtractResult.Ok)
					UpdateProgress(100);
				else if(result == ShooterSubExtractor.SubExtractResult.NoSubFound)
					UpdateProgress(ShooterConst.NoSubFound);
				else
				{
					//error
					UpdateProgress(ShooterConst.Error);
				}

				File.Delete(tempFilePath);
				LogMan.Instance.Log(Resources.InfoSubDownloadOk, _videoFileName);
			}
		}

		#region IJob Members
		public int JobId { get; set; } = -1;

		public void Start()
		{
			string fileHash = Util.CaculateFileHash(_videoFilePath);
			LogMan.Instance.Log(Resources.InfoFindSub, _videoFileName);
			QuerySubByVideoPathOrHash(_videoFilePath, fileHash, "", "");
		}

		public event ProgressHandler ProgressUpdate;

		private void UpdateProgress(int progress) => ProgressUpdate?.Invoke(this, progress);
		#endregion
	}
}