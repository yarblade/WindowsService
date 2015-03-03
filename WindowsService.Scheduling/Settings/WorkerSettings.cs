using System;



namespace WindowsService.Scheduling.Settings
{
	public class WorkerSettings
	{
		public TimeSpan MinInterval { get; set; }

		public TimeSpan MaxInterval { get; set; }

		public TimeSpan FailureInterval { get; set; }

		public LoadingInterval[] LoadingIntervals { get; set; }
	}
}
