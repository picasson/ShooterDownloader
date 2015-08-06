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
using System.Text;

namespace ShooterDownloader
{
	internal class FormData
	{
		private readonly Dictionary<string, string> _postData = new Dictionary<string, string>();
		public string Boundary { get; set; }
		public void AddData(string key, string value) { _postData.Add(key, value); }

		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach(KeyValuePair<string, string> pair in _postData)
			{
				sb.AppendFormat("--{0}\r\n", Boundary);
				sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n", pair.Key, pair.Value);
			}
			sb.AppendFormat("--{0}--\r\n", Boundary);
			return sb.ToString();
		}
	}
}