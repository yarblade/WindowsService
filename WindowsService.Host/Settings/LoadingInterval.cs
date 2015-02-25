using System;

using WindowsService.Host.Entities;



namespace WindowsService.Host.Settings
{
	public class LoadingInterval
	{
		public Loading Loading { get; set; }

		public TimeSpan Interval { get; set; }
	}
}
