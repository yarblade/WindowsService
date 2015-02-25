using System;



namespace WindowsService.Host.Scheduling
{
	public interface IScheduler<in T>
	{
		TimeSpan GetInitialInterval();
		TimeSpan GetWorkerInterval(T loading);
		TimeSpan GetFailureInterval();
		bool IsLoadingChanged(T loading);
	}
}