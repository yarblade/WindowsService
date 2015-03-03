using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Host.Workers;



namespace WindowsService.Scheduling.WorkerRunners
{
	public class DefaultWorkerRunner<T> : IWorkerRunner<T>
	{
		private readonly IDictionary<string, Func<IWorker<T>>> _workers;
		private readonly IDictionary<string, Func<IAsyncWorker<T>>> _asyncWorkers;

		public DefaultWorkerRunner(IDictionary<string, Func<IAsyncWorker<T>>> asyncWorkers)
		{
			_asyncWorkers = asyncWorkers;
		}

		public DefaultWorkerRunner(IDictionary<string, Func<IWorker<T>>> workers)
		{
			_workers = workers;
		}

		public async Task<T> RunAsync(string workerName, CancellationToken token)
		{
			if (_asyncWorkers != null && _asyncWorkers.ContainsKey(workerName))
			{
				return await _asyncWorkers[workerName]().DoWork(token);
			}

			if (_workers != null && _workers.ContainsKey(workerName))
			{
				return await Task.Run(() => _workers[workerName]().DoWork(token), CancellationToken.None);
			}

			throw new InvalidOperationException(string.Format("Can't resolve '{0}' worker", workerName));
		}
	}
}
