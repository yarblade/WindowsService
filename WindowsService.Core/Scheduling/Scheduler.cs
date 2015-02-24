using System;
using System.Linq;

using WindowsService.Core.Entities;
using WindowsService.Core.Settings;



namespace WindowsService.Core.Scheduling
{
	public class Scheduler : IScheduler
	{
		private readonly WorkerSettings _workerSettings;
		private Loading _previousLoading;

		public Scheduler(WorkerSettings workerSettings)
		{
			_workerSettings = workerSettings;
			_previousLoading = Loading.Full;
		}

		public TimeSpan GetWorkerInterval(Loading loading)
		{
			_previousLoading = loading;

			return _workerSettings.LoadingIntervals.Single(x => x.Loading == loading).Interval;
		}

		public bool NeedChangeInterval(Loading loading)
		{
			return loading != _previousLoading;
		}
	}
}
