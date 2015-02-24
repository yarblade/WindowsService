using System;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Entities;
using WindowsService.Core.Extensions;
using WindowsService.Core.Scheduling;

using log4net;



namespace WindowsService.Core.Executors
{
	public class ScheduledExecutor : IExecutor, IDisposable
	{
		private readonly IWorkerExecutor _executor;
		private readonly ILog _log;
		private readonly IScheduler _scheduler;
		private Timer _timer;

		public ScheduledExecutor(IWorkerExecutor executor, IScheduler scheduler, ILog log)
		{
			_executor = executor;
			_scheduler = scheduler;
			_log = log;
		}

		public void Dispose()
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

		public void Execute(CancellationToken token)
		{
			var interval = _scheduler.GetWorkerInterval(Loading.Full);

			_timer = new Timer(_ => ExecuteAsync(token), null, TimeSpan.Zero, interval);
		}

		private async Task ExecuteAsync(CancellationToken token)
		{
			try
			{
				var loading = await _executor.ExecuteAsync(token);
				
				ChangeTimer(loading);
			}
			catch (OperationCanceledException ex)
			{
				_log.Info(ex.Message);
			}
			catch (AggregateException ex)
			{
				ex.Flatten().Handle(
					x =>
					{
						_log.Error("Unexpected exception while service controller execution: " + x);

						return !ex.IsCritical();
					});
			
				ChangeTimer(Loading.Fail);
			}
			catch (Exception ex)
			{
				if (ex.IsCritical())
				{
					throw;
				}

				_log.Error("Unexpected exception while service controller execution: " + ex);
			
				ChangeTimer(Loading.Fail);
			}
		}

		private void ChangeTimer(Loading loading)
		{
			if (_scheduler.NeedChangeInterval(loading))
			{
				_timer.Change(TimeSpan.Zero, _scheduler.GetWorkerInterval(loading));
			}
		}
	}
}
