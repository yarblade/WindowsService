using System;

using WindowsService.Scheduling.Entities;



namespace WindowsService.Scheduling.Settings
{
	public class LoadingInterval
	{
		public Loading Loading { get; set; }

		public TimeSpan Interval { get; set; }
	}
}
