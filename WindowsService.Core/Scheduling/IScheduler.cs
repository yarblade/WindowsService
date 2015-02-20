using System;

using WindowsService.Core.Entities;



namespace WindowsService.Core.Scheduling
{
	public interface IScheduler
	{
		TimeSpan GetWorkerInterval(string workerName, Loading loading, TimeSpan oldInterval);
	}
}