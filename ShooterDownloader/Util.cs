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
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using org.mozilla.intl.chardet;

namespace ShooterDownloader
{
	internal class Util
	{
		public enum ConversionResult
		{
			OK,
			NoConversion,
			Error
		}

		//For ConvertChsToCht
		private const int LOCALE_SYSTEM_DEFAULT = 0x0800;
		private const int LOCALE_TAIWAN = 1028;
		private const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
		private const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;
		private const int BCM_FIRST = 0x1600;
		private const int BCM_SETSHIELD = (BCM_FIRST + 0x000C);
		//For DetectEncoding
		private static volatile bool encodingFound;
		private static volatile string encodingName = string.Empty;

		public static bool IsAdmin {
			get {
				WindowsIdentity id = WindowsIdentity.GetCurrent();
				var p = new WindowsPrincipal(id);
				return p.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}

		public static bool Is64BitOS {
			get {
				if(IntPtr.Size == 8)
				{
					//Application running in 64-bit mode, must be 64-bit OS.
					return true;
				}
				//Dynamicallt load kernel32 and call IsWow64Process.
				IntPtr module = LoadLibrary("Kernel32.dll");
				if(module == IntPtr.Zero)
					return false;

				IntPtr addr = GetProcAddress(module, "IsWow64Process");
				if(addr == IntPtr.Zero)
					return false;

				//Check if application is running in WOW64 mode.
				// IsWow64Process only works on Windows XP sp2 or above.
				//  Dynamically invoke it to avoid unnecessary dependency.
				var dlg = (IsWow64Process)Marshal.GetDelegateForFunctionPointer(addr, typeof(IsWow64Process));
				bool retval;
				dlg.Invoke(Process.GetCurrentProcess().Handle, out retval);

				return retval;
			}
		}

		public static string CaculateFileHash(string filePath)
		{
			var hashString = "";
			var file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			long fileLength = file.Length;
			var offset = new long[4];
			if(fileLength < 8192)
			{
				//a video file less then 8k? impossible! <-- says SPlayer
			}
			else
			{
				const int BlockSize = 4096;
				const int NumOfSegments = 4;

				offset[3] = fileLength - 8192;
				offset[2] = fileLength / 3;
				offset[1] = fileLength / 3 * 2;
				offset[0] = BlockSize;

				MD5 md5 = new MD5CryptoServiceProvider();

				var reader = new BinaryReader(file);
				var sb = new StringBuilder();
				for(var i = 0; i < NumOfSegments; i++)
				{
					file.Seek(offset[i], SeekOrigin.Begin);
					byte[] dataBlock = reader.ReadBytes(BlockSize);
					MD5 md5Crypt = new MD5CryptoServiceProvider();
					byte[] hash = md5Crypt.ComputeHash(dataBlock);
					if(sb.Length > 0)
						sb.Append(';');
					foreach(byte a in hash)
					{
						if(a < 16)
							sb.AppendFormat("0{0}", a.ToString("x"));
						else
							sb.Append(a.ToString("x"));
					}
				}

				reader.Close();
				hashString = sb.ToString();
			}

			return hashString;
		}

		public static int BytesToInt32(byte[] bytes, ByteOrder byteOrder)
		{
			if(byteOrder == ByteOrder.BigEndian)
				Array.Reverse(bytes);

			return BitConverter.ToInt32(bytes, 0);
		}

		public static bool UnGZip(string inFile, string outFile)
		{
			var ret = false;
			FileStream inStream = null;
			FileStream outStream = null;
			GZipStream decompressStream = null;
			try
			{
				inStream = new FileStream(inFile, FileMode.Open);
				decompressStream = new GZipStream(inStream, CompressionMode.Decompress);
				outStream = new FileStream(outFile, FileMode.OpenOrCreate);

				var buffer = new byte[4096];
				var accuRead = 0;
				while((accuRead = decompressStream.Read(buffer, 0, buffer.Length)) > 0)
					outStream.Write(buffer, 0, accuRead);
				ret = true;
			}
			catch(Exception ex)
			{
				LogMan.Instance.Log(ex.Message);
			}
			finally
			{
				if(decompressStream != null)
					decompressStream.Close();
				else if(inStream != null)
					inStream.Close();

				if(outStream != null)
					outStream.Close();
			}

			return ret;
		}

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int LCMapString(int Locale, int dwMapFlags, string lpSrcStr, int cchSrc
			, [Out] string lpDestStr, int cchDest);

		public static ConversionResult ConvertChsToCht(string inFile, string outFile) { return ConvertChsToCht(inFile, outFile, true); }

		public static ConversionResult ConvertChsToCht(string inFile, string outFile, bool detectEncoding)
		{
			var ret = ConversionResult.Error;
			StreamReader reader = null;
			FileStream outStream = null;
			StreamWriter writer = null;

			try
			{
				Encoding inEncoding = DetectEncoding(inFile);
				if(inEncoding != null)
				{
					if(!detectEncoding)
					{
						//if detectEncoding is false, overwrite the result of 
						//  encoding detection unless it's GB or unicode.
						if(inEncoding.CodePage != 936 && inEncoding.CodePage != 54936 &&
							inEncoding != Encoding.UTF8 && inEncoding != Encoding.Unicode)
							inEncoding = Encoding.GetEncoding(936);
					}
					//If the encoding is GB2312, GB18030 or Unicode
					if(inEncoding.CodePage == 936 || inEncoding.CodePage == 54936 ||
						inEncoding == Encoding.UTF8 || inEncoding == Encoding.Unicode)
					{
						reader = new StreamReader(inFile, inEncoding);
						outStream = new FileStream(outFile, FileMode.OpenOrCreate);
						Encoding outEncoding;
						if(inEncoding == Encoding.UTF8 || inEncoding == Encoding.Unicode)
						{
							//if the encoding of the source is UTF8 or UTF16, output encoding should be Unicode too.
							outEncoding = inEncoding;
						}
						else
							outEncoding = Encoding.GetEncoding("Big5");
						writer = new StreamWriter(outStream, outEncoding);
						string line = null;
						while((line = reader.ReadLine()) != null)
						{
							var chtLine = new string(' ', line.Length);
							LCMapString(LOCALE_TAIWAN, LCMAP_TRADITIONAL_CHINESE
								, line, line.Length, chtLine, chtLine.Length);
							writer.WriteLine(chtLine);
						}

						ret = ConversionResult.OK;
					}
					else
						ret = ConversionResult.NoConversion;
				}
				else
					ret = ConversionResult.Error;
			}
			catch(Exception ex)
			{
				LogMan.Instance.Log(ex.Message);
				ret = ConversionResult.Error;
			}
			finally
			{
				if(reader != null)
					reader.Close();

				if(writer != null)
					writer.Close();
				else if(outStream != null)
					outStream.Close();
			}

			return ret;
		}

		public static Encoding DetectEncoding(string filePath)
		{
			var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			Encoding encoding = null;

			var det = new nsDetector(2);
			var not = new Notifier();
			det.Init(not);

			var done = false;
			var isAscii = true;

			var buf = new byte[1024];
			int len = fs.Read(buf, 0, buf.Length);

			//For some reason NCharDet can't detect Unicode.
			//Manual detect Unicode here.
			if(len >= 2 && buf[0] == 0xFF && buf[1] == 0xFE)
			{
				fs.Close();
				det.DataEnd();
				return Encoding.Unicode;
			}

			while(len > 0)
			{
				// Check if the stream is only ascii.
				if(isAscii)
					isAscii = det.isAscii(buf, len);

				// DoIt if non-ascii and not done yet.
				if(!isAscii && !done)
					done = det.DoIt(buf, len, false);

				len = fs.Read(buf, 0, buf.Length);
			}
			fs.Close();
			det.DataEnd();

			if(isAscii)
			{
				encodingFound = true;
				encoding = Encoding.ASCII;
			}

			if(!encodingFound)
			{
				string[] prob = det.getProbableCharsets();
				encodingName = prob[0];
			}

			if(encoding == null)
				encoding = Encoding.GetEncoding(encodingName);

			return encoding;
		}

		public static void RunProc(string filePath, string args, bool needElevation)
		{
			var procInfo = new ProcessStartInfo();
			procInfo.UseShellExecute = true;
			procInfo.FileName = filePath;
			procInfo.Arguments = args;
			procInfo.WorkingDirectory = Environment.CurrentDirectory;
			if(needElevation)
				procInfo.Verb = "runas";

			Process proc = Process.Start(procInfo);
			const int timeout = 5000;
			//proc.WaitForInputIdle();
			proc.WaitForExit(timeout);
		}

		public static bool RegisterDll(string dllPath)
		{
			string regsvr32Path = string.Format("\"{0}\\regsvr32.exe\"",
				Environment.GetFolderPath(Environment.SpecialFolder.System));
			string arg = string.Format("/s \"{0}\"", dllPath);

			try
			{
				//Need administrative privilege to register a COM DLL.
				RunProc(regsvr32Path, arg, !IsAdmin);
			}
			catch(Exception)
			{
				return false;
			}

			return true;
		}

		public static bool UnregisterDll(string dllPath)
		{
			string regsvr32Path = string.Format("\"{0}\\regsvr32.exe\"",
				Environment.GetFolderPath(Environment.SpecialFolder.System));
			string arg = string.Format("/s /u \"{0}\"", dllPath);

			try
			{
				//Need administrative privilege to register a COM DLL.
				RunProc(regsvr32Path, arg, !IsAdmin);
			}
			catch(Exception)
			{
				return false;
			}

			return true;
		}

		[DllImport("user32")]
		private static extern uint SendMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);

		//Add a shield ICON to the button to inform user privilege elevation is required.
		// Only work on Vista or above.
		public static void AddShieldToButton(Button b)
		{
			b.FlatStyle = FlatStyle.System;
			SendMessage(b.Handle, BCM_SETSHIELD, 0, 0xFFFFFFFF);
		}

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr LoadLibrary(string path);

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool FreeLibrary(IntPtr hdl);

		[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hdl, string name);

		public static int GetGetBoundedValue(int intendValue, int lowerBound, int upperBound)
		{
			int trueValue;
			trueValue = Math.Min(intendValue, upperBound);
			trueValue = Math.Max(lowerBound, intendValue);
			return trueValue;
		}

		private class Notifier : nsICharsetDetectionObserver
		{
			public void Notify(string charset)
			{
				encodingFound = true;
				encodingName = charset;
			}
		}

		private delegate bool IsWow64Process(
			[In] IntPtr hProcess,
			[Out] out bool wow64Process);
	}

	//For BytesToInt32
	public enum ByteOrder
	{
		LittleEndian,
		BigEndian
	}
}