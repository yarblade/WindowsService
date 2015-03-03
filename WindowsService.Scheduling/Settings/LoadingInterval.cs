using System;



namespace WindowsService.Scheduling.Settings
{
	public class LoadingInterval
	{
		public Loading.Loading Loading { get; set; }

		public TimeSpan Interval { get; set; }
	}
}
