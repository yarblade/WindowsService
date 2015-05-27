using System;
using System.Threading;



namespace WindowsService.Core.Timers
{
	public interface ITimer
	{
		void Start(TimerCallback callback);
		void Change(TimeSpan interval);
		void Stop();
	}
}
