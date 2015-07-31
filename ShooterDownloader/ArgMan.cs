using System.Collections.Generic;
using System.IO;

namespace ShooterDownloader
{
	internal class ArgMan
	{
		private static ArgMan _instance;
		private static readonly object _instanceLock = new object();
		private string _listFilePath = string.Empty;
		private bool _removeListFile;
		private bool _useListFile;

		private ArgMan() { }

		public static ArgMan Instance {
			get {
				if(_instance == null)
				{
					lock(_instanceLock)
					{
						if(_instance == null)
							_instance = new ArgMan();
					}
				}

				return _instance;
			}
		}

		public string[] Files { get; private set; }
		public bool CodeConversionOnly { get; private set; }

		public void ParseArgs(string[] args)
		{
			var fileList = new List<string>();

			foreach(string arg in args)
			{
				if(arg.StartsWith("-lst="))
				{
					fileList.Clear();
					_useListFile = true;
					_listFilePath = ExtractArgValue(arg);
					if(File.Exists(_listFilePath))
					{
						var reader = new StreamReader(_listFilePath);

						string line = reader.ReadLine();
						while(line != null)
						{
							fileList.Add(line);
							line = reader.ReadLine();
						}
						reader.Close();
					}
				}
				else if(arg == "/c")
					CodeConversionOnly = true;
				else if(arg == "/r")
					_removeListFile = true;
				else if(!arg.StartsWith("-") && !arg.StartsWith("/"))
				{
					if(!_useListFile && File.Exists(arg))
						fileList.Add(arg);
				}
			}

			if(_removeListFile && _listFilePath != null)
			{
				if(File.Exists(_listFilePath))
					File.Delete(_listFilePath);
			}

			if(fileList.Count > 0)
				Files = fileList.ToArray();
		}

		private string ExtractArgValue(string arg)
		{
			int idx = arg.IndexOf('=');
			if((idx + 1) < arg.Length)
				return arg.Substring(idx + 1);
			return string.Empty;
		}
	}
}