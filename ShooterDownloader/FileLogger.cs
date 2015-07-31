using System;
using System.IO;
using System.Windows.Forms;
using ShooterDownloader.Properties;

namespace ShooterDownloader
{
	internal class FileLogger : ILogger, IDisposable
	{
		//private string _logFilePath = String.Empty;
		private readonly StreamWriter _logWriter;

		public FileLogger()
		{
			string logName = Settings.Default.LogFileName;
			string date = DateTime.Now.ToString("yyMMdd");
			string logFilePath = $"{LogDirectory}\\{logName}_{date}.log";
			_logWriter = new StreamWriter(logFilePath, true);
		}

		public static string LogDirectory => Application.LocalUserAppDataPath;

		#region IDisposable Members
		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}
		#endregion

		public void Close() => Dispose();
		//Dispose implementation
		private void Dispose(bool disposing)
		{
			if(disposing)
				_logWriter.Close();
		}

		//Log implementation
		private void WriteMessage(string message)
		{
			//sync log writing
			lock(_logWriter)
			{
				//using (StreamWriter sw = new StreamWriter(_logFilePath, true))
				//{
				//    sw.WriteLine(message);
				//}
				_logWriter.WriteLine(message);
				_logWriter.Flush();
			}
		}

		#region ILogger Members
		public void Log(string message) => WriteMessage(message);

		public void Log(string message, params object[] args)
		{
			string formatMessage = string.Format(message, args);
			WriteMessage(formatMessage);
		}
		#endregion
	}
}