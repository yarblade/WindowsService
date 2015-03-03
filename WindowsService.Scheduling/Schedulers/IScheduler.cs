using System;



namespace WindowsService.Scheduling.Schedulers
{
	public interface IScheduler<in T>
	{
		TimeSpan GetInitialInterval();
		TimeSpan GetWorkerInterval(T loading);
		TimeSpan GetFailureInterval();
		bool IsLoadingChanged(T loading);
	}
}