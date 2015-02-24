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
		private readonly IAsyncExecutor _executor;
		private readonly ILog _log;
		private readonly IScheduler _scheduler;
		private readonly string _workerName;
		private Timer _timer;
		private readonly SemaphoreSlim _semaphore;

		public ScheduledExecutor(IAsyncExecutor executor, IScheduler scheduler, string workerName, ILog log)
		{
			_executor = executor;
			_scheduler = scheduler;
			_workerName = workerName;
			_log = log;
			_semaphore = new SemaphoreSlim(initialCount: 1);
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
			if (token.IsCancellationRequested)
			{
				return;
			}

			var lockTaken = false;
			try
			{
				lockTaken = _semaphore.Wait(0);
				if (lockTaken)
				{
					await ExecuteAsyncImpl(token);
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

		private async Task ExecuteAsyncImpl(CancellationToken token)
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
						_log.Error("Unexpected exception while scheduled execution: " + x);

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

				_log.Error("Unexpected exception while scheduled execution: " + ex);
			
				ChangeTimer(Loading.Fail);
			}
		}

		private void ChangeTimer(Loading loading)
		{
			if (_scheduler.NeedChangeInterval(loading))
			{
				var interval = _scheduler.GetWorkerInterval(loading);

				_log.InfoFormat("Loading of '{0}' worker was changed to {1}. New interval: {2}", _workerName, loading, interval);

				_timer.Change(TimeSpan.Zero, interval);
			}
		}
	}
}
