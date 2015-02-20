using System;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Entities;
using WindowsService.Core.Executors;
using WindowsService.Core.Extensions;
using WindowsService.Core.Scheduling;

using log4net;



namespace WindowsService.Core.Workers
{
	public class WorkerManager : IWorkerManager
	{
		private readonly IWorkerExecutor[] _executors;
		private readonly IScheduler _scheduler;
		private readonly ILog _log;
		private readonly Timer[] _timers;
		private bool _disposing;

		public WorkerManager(IWorkerExecutor[] executors, IScheduler scheduler, ILog log)
		{
			_executors = executors;
			_scheduler = scheduler;
			_log = log;
			_timers = new Timer[_executors.Length];
		}

		public void Run(CancellationToken token)
		{
			for (int i = 0; i < _executors.Length; i++)
			{
				var index = i;
				var executor = _executors[index];
				var interval = _scheduler.GetWorkerInterval(executor.WorkerName, Loading.Full, TimeSpan.Zero);

				_timers[index] = new Timer(
					x =>
					{
						ExecuteWorker(executor, index, interval, token);
					},
					null,
					TimeSpan.FromMilliseconds(50),
					interval);
			}
		}

		private void ExecuteWorker(IWorkerExecutor executor, int index, TimeSpan interval, CancellationToken token)
		{
			try
			{
				ExecuteAsync(executor, index, interval, token).Wait(CancellationToken.None);
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
			}
			catch (Exception ex)
			{
				if (ex.IsCritical())
				{
					throw;
				}

				_log.Error("Unexpected exception while service controller execution: " + ex);
			}
		}

		private async Task ExecuteAsync(IWorkerExecutor executor, int index, TimeSpan interval, CancellationToken token)
		{
			try
			{
				var loading = await executor.ExecuteAsync(token);
				_log.Debug(string.Format("Worker: \"{0}\" has finished with loading {1}.", executor.WorkerName, loading));
				
				UpdateTimer(_timers[index], executor, loading, interval);
			}
			catch(Exception ex)
			{
				if (!ex.IsCritical())
				{
					UpdateTimer(_timers[index], executor, Loading.Fail, interval);
				}

				throw;
			}
		}

		private void UpdateTimer(Timer timer, IWorkerExecutor executor, Loading loading, TimeSpan oldInterval)
		{
			if (loading != Loading.Skip)
			{
				var interval = _scheduler.GetWorkerInterval(executor.WorkerName, loading, oldInterval);
				if (interval != oldInterval)
				{
					timer.Change(TimeSpan.Zero, interval);
				}
			}
		}

		public void Dispose()
		{
			if (!_disposing)
			{
				_disposing = true;

				var waitHandles = new WaitHandle[_timers.Length];
				for (int i = 0; i < _timers.Length; i++)
				{
					var autoEvent = new AutoResetEvent(false);
					if (_timers[i].Dispose(autoEvent))
					{
						waitHandles[i] = autoEvent;
					}

					_timers[i] = null;
				}

				WaitHandle.WaitAll(waitHandles);
			}
		}
	}
}
