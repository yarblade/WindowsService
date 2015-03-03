using System;
using System.Linq;

using WindowsService.Scheduling.Settings;



namespace WindowsService.Scheduling.Schedulers
{
	public class Scheduler : IScheduler<Loading.Loading>
	{
		private readonly WorkerSettings _workerSettings;
		private Loading.Loading _previousLoading;
		
		public Scheduler(WorkerSettings workerSettings)
		{
			_workerSettings = workerSettings;
			_previousLoading = Loading.Loading.Full;
		}

		public TimeSpan GetInitialInterval()
		{
			return GetWorkerInterval(Loading.Loading.Full);
		}

		public TimeSpan GetFailureInterval()
		{
			return _workerSettings.FailureInterval;
		}

		public TimeSpan GetWorkerInterval(Loading.Loading loading)
		{
			_previousLoading = loading;

			return _workerSettings.LoadingIntervals.Single(x => x.Loading == loading).Interval;
		}

		public bool IsLoadingChanged(Loading.Loading loading)
		{
			return _previousLoading != loading;
		}
	}
}
