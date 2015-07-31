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

using System.Collections.Generic;

namespace ShooterDownloader
{
	internal class LogMan
	{
		private static LogMan _instance;
		private static readonly object _instanceLock = new object();
		private readonly List<ILogger> _loggers;
		private readonly object _logLock = new object();
		private readonly object _registerLock = new object();
		private LogMan() { _loggers = new List<ILogger>(); }

		public static LogMan Instance {
			get {
				if(_instance == null)
				{
					lock(_instanceLock)
					{
						if(_instance == null)
							_instance = new LogMan();
					}
				}

				return _instance;
			}
		}

		public void RegisterLogger(ILogger logger)
		{
			lock(_registerLock)
				_loggers.Add(logger);
		}

		public void RemoveLogger(ILogger logger)
		{
			lock(_registerLock)
				_loggers.Remove(logger);
		}

		public void Log(string message)
		{
			lock(_logLock)
			{
				if(_loggers.Count > 0)
				{
					foreach(ILogger logger in _loggers)
						logger.Log(message);
				}
			}
		}

		public void Log(string message, params object[] args)
		{
			lock(_logLock)
			{
				if(_loggers.Count <= 0) return;
				foreach(ILogger logger in _loggers)
					logger.Log(message, args);
			}
		}
	}
}