using System;
using System.Linq;

using WindowsService.Host.Entities;
using WindowsService.Host.Settings;



namespace WindowsService.Host.Scheduling
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
