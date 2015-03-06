using System;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Extensions;
using WindowsService.Core.Sandboxes;
using WindowsService.Core.Timers;
using WindowsService.Core.Workers;
using WindowsService.Scheduling.Schedulers;

using Common.Log;



namespace WindowsService.Scheduling.Sandboxes
{
	public class ScheduledWorkerSandbox<T> : IWorkerSandbox
	{
		private readonly TaskCompletionSource<bool> _completionSource;
		private readonly ILog _log;
		private readonly IScheduler<T> _scheduler;
		private readonly ITimer _timer;
		private readonly string _workerName;
		private readonly IWorkerRunner<T> _workerRunner;

		public ScheduledWorkerSandbox(string workerName, ITimer timer, IWorkerRunner<T> workerRunner, IScheduler<T> scheduler, ILog log)
		{
			_workerName = workerName;
			_timer = timer;
			_workerRunner = workerRunner;
			_scheduler = scheduler;
			_log = log;
			_completionSource = new TaskCompletionSource<bool>();
		}

		public Task Completion { get { return _completionSource.Task; } }

		public void StartWorkerExecution(CancellationToken token)
		{
			_timer.Start(x => ExecuteAsync(token));
		}

		private async Task ExecuteAsync(CancellationToken token)
		{
			if (token.IsCancellationRequested)
			{
				StopExecution();
				return;
			}

			try
			{
				var loading = await _workerRunner.RunAsync(_workerName, token);

				ChangeTimer(loading);
			}
			catch (OperationCanceledException ex)
			{
				_log.Info(ex.Message);
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

			if (token.IsCancellationRequested)
			{
				StopExecution();
			}
		}

		private void StopExecution()
		{
			_timer.Stop();
			_completionSource.TrySetResult(true);
		}

		private void ChangeTimer(T loading)
		{
			if (_scheduler.IsLoadingChanged(loading))
			{
				var interval = _scheduler.GetWorkerInterval(loading);

				_log.Info("Loading of '{0}' worker changed to {1}. New interval: {2}", _workerName, loading, interval);

				_timer.Change(interval);
			}
		}

		private void OnFailure()
		{
			var interval = _scheduler.GetFailureInterval();

			_log.Info("Worker '{0}' was failed. New interval: {1}", _workerName, interval);

			_timer.Change(interval);
		}
	}
}
