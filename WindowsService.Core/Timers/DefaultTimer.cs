using System;
using System.Threading;



namespace WindowsService.Core.Timers
{
	public class DefaultTimer : ITimer
	{
		private readonly TimeSpan _initialInterval;
		private Timer _timer;

		public DefaultTimer()
			: this(TimeSpan.Zero)
		{
		}

		public DefaultTimer(TimeSpan initialInterval)
		{
			_initialInterval = initialInterval;
		}

		public void Start(TimerCallback callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback", "Callback can't be null.");
			}

			if (_timer != null)
			{
				throw new InvalidOperationException("Timer already started.");
			}

			_timer = new Timer(x => OnTimerExecute(callback), null, TimeSpan.Zero, _initialInterval);
		}

		public void Change(TimeSpan interval)
		{
			if (_timer != null)
			{
				_timer.Change(TimeSpan.Zero, interval);
			}
		}

		public void Stop()
		{
			if (_timer != null)
			{
				var autoEvent = new AutoResetEvent(false);
				if (_timer.Dispose(autoEvent))
				{
					autoEvent.WaitOne();
				}

				_timer = null;
			}
		}

		protected virtual void OnTimerExecute(TimerCallback callback)
		{
			callback(null);
		}
	}
}
