using System;

using WindowsService.Core.Entities;



namespace WindowsService.Core.Settings
{
	public class LoadingInterval
	{
		public Loading Loading { get; set; }

		public TimeSpan Interval { get; set; }
	}
}
