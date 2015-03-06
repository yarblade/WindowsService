using System;
using System.Linq;

using WindowsService.Scheduling.Entities;
using WindowsService.Scheduling.Settings;



namespace WindowsService.Scheduling.Schedulers
{
	public class Scheduler : IScheduler<Loading>
	{
		private readonly WorkerSettings _workerSettings;
		private Loading _previousLoading;
		
		public Scheduler(WorkerSettings workerSettings)
		{
			_workerSettings = workerSettings;
		}

		public TimeSpan GetFailureInterval()
		{
			return _workerSettings.FailureInterval;
		}

		public TimeSpan GetWorkerInterval(Loading loading)
		{
			_previousLoading = loading;

			return _workerSettings.LoadingIntervals.Single(x => x.Loading == loading).Interval;
		}

		public bool IsLoadingChanged(Loading loading)
		{
			return _previousLoading != loading;
		}
	}
}
