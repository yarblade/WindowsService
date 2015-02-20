using System;



namespace WindowsService.Core.Settings
{
	public class WorkerLoadingIntervals
	{
		public string WorkerName { get; set; }

		public TimeSpan MinInterval { get; set; }

		public TimeSpan MaxInterval { get; set; }

		public LoadingInterval[] LoadingIntervals { get; set; }
	}
}
