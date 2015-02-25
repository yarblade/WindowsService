using System;

using WindowsService.Host.Entities;



namespace WindowsService.Host.Scheduling
{
	public interface IScheduler
	{
		TimeSpan GetWorkerInterval(Loading loading);
		bool NeedChangeInterval(Loading loading);
	}
}