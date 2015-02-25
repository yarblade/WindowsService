using System;



namespace WindowsService.Host.Settings
{
	public class WorkerSettings
	{
		public TimeSpan MinInterval { get; set; }

		public TimeSpan MaxInterval { get; set; }

		public LoadingInterval[] LoadingIntervals { get; set; }
	}
}
