using System;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Entities;
using WindowsService.Core.Workers;

using log4net;

using Microsoft.Practices.Unity;



namespace WindowsService.Core.Executors
{
	public class WorkerExecutor : IWorkerExecutor
	{
		private readonly IUnityContainer _container;
		private readonly ILog _log;
		private readonly string _workerName;
		private readonly SemaphoreSlim _semaphore;

		public WorkerExecutor(string workerName, IUnityContainer container, int maxInstanceCount, ILog log)
		{
			_workerName = workerName;
			_container = container;
			_log = log;
			_semaphore = new SemaphoreSlim(maxInstanceCount);
		}

		public string WorkerName { get { return _workerName; } }

		public async Task<Loading> ExecuteAsync(CancellationToken token)
		{
			if (token.IsCancellationRequested)
			{
				return Loading.Skip;
			}

			var lockTaken = false;

			try
			{
				lockTaken = TryAcquireLock();

				if (!lockTaken)
				{
					return Loading.Skip;
				}

				var loading = await ResolveAndExecute(_workerName, token);
				_log.Debug(string.Format("Worker: \"{0}\" has finished with loading {1}.", _workerName, loading));

				return loading;
			}
			finally
			{
				if (lockTaken)
				{
					ReleaseLock();
				}
			}
		}

		private async Task<Loading> ResolveAndExecute(string workerName, CancellationToken token)
		{
			using (var childContainer = _container.CreateChildContainer())
			{
				if (childContainer.IsRegistered<IAsyncWorker>(workerName))
				{
					return await childContainer.Resolve<IAsyncWorker>(workerName).DoWork(token);
				}

				if (childContainer.IsRegistered<IWorker>(workerName))
				{
					var worker = childContainer.Resolve<IWorker>(workerName);
					return await Task.Run(() => worker.DoWork(token), CancellationToken.None);
				}

				throw new InvalidOperationException(string.Format("Can't resolve '{0}' worker", workerName));
			}
		}

		private bool TryAcquireLock()
		{
			return _semaphore.Wait(0);
		}

		private void ReleaseLock()
		{
			_semaphore.Release();
		}
	}
}
