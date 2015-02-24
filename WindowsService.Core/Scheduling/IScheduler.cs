using System;

using WindowsService.Core.Entities;



namespace WindowsService.Core.Scheduling
{
	public interface IScheduler
	{
		TimeSpan GetWorkerInterval(Loading loading);
		bool NeedChangeInterval(Loading loading);
	}
}