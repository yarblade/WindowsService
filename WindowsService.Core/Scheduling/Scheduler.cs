using System;
using System.Linq;

using WindowsService.Core.Entities;
using WindowsService.Core.Settings;



namespace WindowsService.Core.Scheduling
{
	public class Scheduler : IScheduler
	{
		private readonly WorkersSettings _workersSettings;

		public Scheduler(WorkersSettings workersSettings)
		{
			_workersSettings = workersSettings;
		}

		public TimeSpan GetWorkerInterval(string workerName, Loading loading, TimeSpan oldInterval)
		{
			var intervals = _workersSettings.WorkersLoadingIntervals.Single(x => x.WorkerName == workerName);
			
			return intervals.LoadingIntervals.Single(x => x.Loading == loading).Interval;
		}
	}
}
