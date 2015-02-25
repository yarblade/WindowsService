using System;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Host.Extensions;
using WindowsService.Host.Scheduling;
using WindowsService.Host.Workers;

using log4net;



namespace WindowsService.Host.Executors
{
	public class ScheduledExecutor<T> : IExecutor, IDisposable
	{
		private readonly IWorkerRunner<T> _workerRunner;
		private readonly ILog _log;
		private readonly IScheduler<T> _scheduler;
		private readonly string _workerName;
		private Timer _timer;
		private readonly SemaphoreSlim _semaphore;

		public ScheduledExecutor(string workerName, IWorkerRunner<T> workerRunner, IScheduler<T> scheduler, ILog log)
		{
			_workerName = workerName;
			_workerRunner = workerRunner;
			_scheduler = scheduler;
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
			var interval = _scheduler.GetInitialInterval();

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
				var loading = await _workerRunner.RunAsync(_workerName, token);
				
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

				OnFailure();
			}
			catch (Exception ex)
			{
				if (ex.IsCritical())
				{
					throw;
				}

				_log.Error("Unexpected exception while scheduled execution: " + ex);
			
				OnFailure();
			}
		}

		private void ChangeTimer(T loading)
		{
			if (_scheduler.IsLoadingChanged(loading))
			{
				var interval = _scheduler.GetWorkerInterval(loading);

				_log.InfoFormat("Loading of '{0}' worker changed to {1}. New interval: {2}", _workerName, loading, interval);

				_timer.Change(TimeSpan.Zero, interval);
			}
		}

		private void OnFailure()
		{
			var interval = _scheduler.GetFailureInterval();

			_log.InfoFormat("Worker '{0}' was failed. New interval: {1}", _workerName, interval);

			_timer.Change(TimeSpan.Zero, interval);
		}
	}
}
