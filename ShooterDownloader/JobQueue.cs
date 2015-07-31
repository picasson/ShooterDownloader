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
using System.Threading;

namespace ShooterDownloader
{
	public delegate void AllDoneHandler();

	internal class JobQueue
	{
		private readonly Queue<IJob> _pendingJobs;
		private volatile bool _continue = true;
		private Thread[] _jobHandlers;
		private int _jobsCount;
		private int _maxConcurrentJobs;

		public JobQueue(int maxJobs)
		{
			_pendingJobs = new Queue<IJob>();
			_maxConcurrentJobs = Util.GetGetBoundedValue(maxJobs, 1, ShooterConst.MaxConcurrentJobs);
			InitializeJobHandler();
		}

		public bool Running => _jobsCount > 0;

		private void InitializeJobHandler()
		{
			_jobHandlers = new Thread[_maxConcurrentJobs];
			for(var i = 0; i < _maxConcurrentJobs; i++)
				_jobHandlers[i] = new Thread(JobHandler);
		}

		public event AllDoneHandler AllDone;
		private void FireAllDone() => AllDone?.Invoke();

		public void AddJob(IJob job)
		{
			lock(_pendingJobs)
			{
				_pendingJobs.Enqueue(job);
				_jobsCount++;
				Monitor.Pulse(_pendingJobs);
			}
		}

		public void Start()
		{
			_continue = true;
			foreach(Thread jobHandler in _jobHandlers)
				jobHandler.Start();
		}

		public void Close()
		{
			lock(_pendingJobs)
			{
				_continue = false;
				Monitor.PulseAll(_pendingJobs);
			}

			foreach(Thread jobHandler in _jobHandlers)
				jobHandler.Join();

			_pendingJobs.Clear();
			_jobsCount = 0;
		}

		public void Reset() => Reset(_maxConcurrentJobs);

		public void Reset(int newMaxJobs)
		{
			Close();

			_maxConcurrentJobs = Util.GetGetBoundedValue(newMaxJobs, 1, ShooterConst.MaxConcurrentJobs);
			InitializeJobHandler();

			Start();
		}

		private void JobHandler()
		{
			while(_continue)
			{
				IJob job = null;
				lock(_pendingJobs)
				{
					if(_continue && _pendingJobs.Count == 0)
					{
						//wait until there is a new job.
						Monitor.Wait(_pendingJobs);
					}

					if(_continue && _pendingJobs.Count > 0)
					{
						//get the job.
						job = _pendingJobs.Dequeue();
					}
				}

				if(_continue && job != null)
				{
					//do the job.
					job.Start();

					//job done 
					_jobsCount--;

					if(_jobsCount == 0)
						FireAllDone();
				}
			}
		}
	}
}