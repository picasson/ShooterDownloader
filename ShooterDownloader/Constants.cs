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

namespace ShooterDownloader
{
	internal class ShooterConst
	{
		public const int Error = -1;
		public const int NoSubFound = -2;
		public const int MaxConcurrentJobs = 3;
		public const int MaxHttpTimeout = 60;
		public const int MinHttpTimeout = 10;
		public const int HttpTimeoutIncrement = 10;
		public const string DontConversion = "不转换";
		public const string AutoChtToChsConversion = "繁体 -> 简体";
		public const string AutoChsToChtConversion = "简体 -> 繁体";
	}
}