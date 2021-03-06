﻿using WindowsService.Scheduling.Entities;



namespace WindowsService.Scheduling.Helpers
{
	public static class LoadingCalculator
	{
		public static Loading Calculate(long processed, long total)
		{
			var percent = 100 * processed / total;
			if (percent == (long)Loading.None)
			{
				return Loading.None;
			}

			if (percent > (long)Loading.Medium)
			{
				return Loading.Full;
			}

			return Loading.Medium;
		}
	}
}
