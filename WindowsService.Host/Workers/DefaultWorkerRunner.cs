using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;



namespace WindowsService.Host.Workers
{
	public class DefaultWorkerRunner<T> : IWorkerRunner<T>
	{
		private readonly IDictionary<string, Func<IAsyncWorker<T>>> _workers;

		public DefaultWorkerRunner(IDictionary<string, Func<IAsyncWorker<T>>> workers)
		{
			_workers = workers;
		}
		
		public async Task<T> RunAsync(string workerName, CancellationToken token)
		{
			if (!_workers.ContainsKey(workerName))
			{
				throw new InvalidOperationException(string.Format("Can't find {0} worker", workerName));
			}

			return await _workers[workerName]().DoWork(token);
		}
	}
}
