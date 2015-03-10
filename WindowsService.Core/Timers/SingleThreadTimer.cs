using System;
using System.Threading;



namespace WindowsService.Core.Timers
{
	public sealed class SingleThreadTimer : DefaultTimer
	{
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(initialCount: 1);

		public SingleThreadTimer()
		{
		}

		public SingleThreadTimer(TimeSpan interval)
			: base(interval)
		{
		}

		protected override void OnTimerExecute(TimerCallback callback)
		{
			var lockTaken = false;
			try
			{
				lockTaken = _semaphore.Wait(0);
				if (lockTaken)
				{
					base.OnTimerExecute(callback);
				}
			}
			finally
			{
				if (lockTaken)
				{
					_semaphore.Release();
				}
			}
		}
	}
}
