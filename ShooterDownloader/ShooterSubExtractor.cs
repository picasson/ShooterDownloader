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
using System.IO;
using System.Text;
using ShooterDownloader.Properties;

namespace ShooterDownloader
{
	internal class ShooterSubExtractor
	{
		public enum SubExtractResult
		{
			Ok,
			Error,
			NoSubFound
		}

		private readonly Dictionary<string, int> _extCounter;
		private string _outputDir = string.Empty;
		private string _subFileNameWithoutExt = string.Empty;
		private string _videoFileName = string.Empty;
		private string _videoFilePath = string.Empty;
		public ShooterSubExtractor() { _extCounter = new Dictionary<string, int>(); }

		public string VideoFilePath {
			get { return _videoFilePath; }
			set {
				_videoFilePath = value;
				_outputDir = Path.GetDirectoryName(_videoFilePath);
				_videoFileName = Path.GetFileName(_videoFilePath);
				int dotIndex = _videoFileName.LastIndexOf('.');
				_subFileNameWithoutExt = _videoFileName.Substring(0, dotIndex);
			}
		}

		public string DumpFilePath { get; set; } = string.Empty;

		public SubExtractResult ExtractSubtitles()
		{
			var result = SubExtractResult.Ok;
			FileStream subTempStream;
			BinaryReader subTempReader = null;
			try
			{
				subTempStream = new FileStream(DumpFilePath, FileMode.Open, FileAccess.Read);
				subTempReader = new BinaryReader(subTempStream);
				var statCode = (sbyte)subTempReader.ReadByte();
				var bOk = true;
				if(statCode < 0)
				{
					if(statCode == -1)
					{
						//subtitle not found
						LogMan.Instance.Log(Resources.ErrSubNotFound, _videoFileName);
						result = SubExtractResult.NoSubFound;
					}
					else
					{
						//data connection error
						LogMan.Instance.Log(Resources.ErrSubDownloadFailed, _videoFileName);
						result = SubExtractResult.Error;
					}

					bOk = false;
				}
				else
				{
					//sub found
					LogMan.Instance.Log(Resources.InfoSubFound, _videoFileName, statCode);
				}

				if(bOk)
				{
					for(var i = 0; i < statCode; i++)
						HandleSubPackage(subTempReader);
				}
			}
			catch(Exception ex)
			{
				LogMan.Instance.Log(Resources.ErrSubExtraction, _videoFileName, ex.Message);
				result = SubExtractResult.Error;
			}
			finally
			{
				if(subTempReader != null)
					subTempReader.Close();
			}

			return result;
		}

		private void HandleSubPackage(BinaryReader reader)
		{
			//TODO: Handle errors
			//package header
			//Can't use BinaryReader.ReadInt32(). 
			//The ReadInt32 reads value in LE order, while the value is encoded in BE order.
			//int packageLen = reader.ReadInt32();
			int packageLen = Util.BytesToInt32(reader.ReadBytes(4), ByteOrder.BigEndian);
			int descLen = Util.BytesToInt32(reader.ReadBytes(4), ByteOrder.BigEndian);
			byte[] desc = reader.ReadBytes(descLen);

			//file data header
			int fileDataLen = Util.BytesToInt32(reader.ReadBytes(4), ByteOrder.BigEndian);
			var numOfFiles = (sbyte)reader.ReadByte();

			for(var i = 0; i < numOfFiles; i++)
				HandleSingleSub(reader);
		}

		private void HandleSingleSub(BinaryReader reader)
		{
			//TODO: Handle errors
			int singleFilePackLen = Util.BytesToInt32(reader.ReadBytes(4), ByteOrder.BigEndian);
			int extLen = Util.BytesToInt32(reader.ReadBytes(4), ByteOrder.BigEndian);
			byte[] ext = reader.ReadBytes(extLen);
			string extString = Encoding.UTF8.GetString(ext);
			int fileLen = Util.BytesToInt32(reader.ReadBytes(4), ByteOrder.BigEndian);

			int leftToRead = fileLen;
			const int bufferSize = 4096;
			var buffer = new byte[bufferSize];
			string tempFilePath = Path.GetTempFileName();
			var tempStream =
				new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite);
			var bGzipped = false;

			do
			{
				int needToRead = Math.Min(leftToRead, bufferSize);
				int accuRead = reader.Read(buffer, 0, needToRead);

				if(leftToRead == fileLen) //if it's the first round.
				{
					//check if the data is gzipped.
					bGzipped = (buffer[0] == 0x1f) && (buffer[1] == 0x8b) && (buffer[2] == 0x08);
				}

				if(accuRead == 0)
					break;
				leftToRead -= accuRead;

				//Output
				tempStream.Write(buffer, 0, accuRead);
			}
			while(leftToRead > 0);
			tempStream.Close();

			var bOk = true;
			//decompress
			if(bGzipped)
			{
				string flatTempFilePath = Path.GetTempFileName();
				bool ret = Util.UnGZip(tempFilePath, flatTempFilePath);
				File.Delete(tempFilePath);
				if(!ret)
				{
					LogMan.Instance.Log(Resources.ErrUnGZipFailed, _videoFileName);
					File.Delete(flatTempFilePath);
					bOk = false;
				}
				else
				{
					//continue processing the decompressed file.
					tempFilePath = flatTempFilePath;
				}
			}

			if(bOk)
			{
				if(Settings.Default.AutoChsToChtConversion)
				{
					//Do converision
					string chtTempPath = Path.GetTempFileName();
					LogMan.Instance.Log(Resources.InfoStartChtConversion, _videoFileName);
					Util.ConversionResult ret = Util.ConvertChsToCht(tempFilePath, chtTempPath);

					if(ret == Util.ConversionResult.OK)
					{
						LogMan.Instance.Log(Resources.InfoChtConversionOk, _videoFileName);
						File.Delete(tempFilePath);
						//continue processing the decompressed file.
						tempFilePath = chtTempPath;
					}
					else if(ret == Util.ConversionResult.NoConversion)
					{
						//No conversion happened
						LogMan.Instance.Log(Resources.InfoNoConversion, _videoFileName);
						File.Delete(chtTempPath);
					}
					else
					{
						LogMan.Instance.Log(Resources.ErrChtConvertion, _videoFileName);
						File.Delete(chtTempPath);
					}
				}
			}

			if(bOk)
			{
				//final output
				File.Move(tempFilePath, GetOutputFilename(extString));
			}
		}

		private string GetOutputFilename(string ext)
		{
			const string langId = "chn";
			int count;

			string outputPath;
			if(!_extCounter.TryGetValue(ext, out count))
			{
				outputPath = $"{_outputDir}\\{_subFileNameWithoutExt}.{langId}.{ext}";
				count = 1;
				_extCounter.Add(ext, count);
			}
			else
			{
				outputPath = $"{_outputDir}\\{_subFileNameWithoutExt}.{langId}{count}.{ext}";
				count++;
				_extCounter[ext] = count;
			}

			//if the generated file name already exists, gen a new one
			while(File.Exists(outputPath))
			{
				outputPath = $"{_outputDir}\\{_subFileNameWithoutExt}.{langId}{count}.{ext}";
				count++;
				_extCounter[ext] = count;
			}

			return outputPath;
		}
	}
}