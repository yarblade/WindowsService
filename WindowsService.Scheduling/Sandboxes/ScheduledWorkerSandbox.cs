using System;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Host.Extensions;
using WindowsService.Host.Sandboxes;
using WindowsService.Scheduling.Schedulers;
using WindowsService.Scheduling.WorkerRunners;

using Common.Log;



namespace WindowsService.Scheduling.Sandboxes
{
	public class ScheduledWorkerSandbox<T> : IWorkerSandbox, IDisposable
	{
		private readonly IWorkerRunner<T> _workerRunner;
		private readonly ILog _log;
		private readonly IScheduler<T> _scheduler;
		private readonly string _workerName;
		private readonly SemaphoreSlim _semaphore;
		private Timer _timer;
		
		public ScheduledWorkerSandbox(string workerName, IWorkerRunner<T> workerRunner, IScheduler<T> scheduler, ILog log)
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

		public void StartExecution(CancellationToken token)
		{
			var interval = _scheduler.GetInitialInterval();

			_timer = new Timer(_ => ExecuteOnlyOneWorkerAtSameTime(token), null, TimeSpan.Zero, interval);
		}

		private async Task ExecuteOnlyOneWorkerAtSameTime(CancellationToken token)
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
					await ExecuteAsync(token);
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

		private async Task ExecuteAsync(CancellationToken token)
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

				_log.Info("Loading of '{0}' worker changed to {1}. New interval: {2}", _workerName, loading, interval);

				_timer.Change(TimeSpan.Zero, interval);
			}
		}

		private void OnFailure()
		{
			var interval = _scheduler.GetFailureInterval();

			_log.Info("Worker '{0}' was failed. New interval: {1}", _workerName, interval);

			_timer.Change(TimeSpan.Zero, interval);
		}
	}
}
