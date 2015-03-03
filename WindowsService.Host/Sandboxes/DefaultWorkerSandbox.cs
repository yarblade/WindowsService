using System;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Host.Workers;



namespace WindowsService.Host.Sandboxes
{
	public class DefaultWorkerSandbox<T> : IWorkerSandbox
	{
		private readonly IAsyncWorker<T> _asyncWorker;
		private readonly TimeSpan _interval;
		private readonly IWorker<T> _worker;
		
		public DefaultWorkerSandbox(IWorker<T> worker, TimeSpan interval)
		{
			_interval = interval;
			_worker = worker;
		}

		public DefaultWorkerSandbox(IAsyncWorker<T> asyncWorker, TimeSpan interval)
		{
			_interval = interval;
			_asyncWorker = asyncWorker;
		}

		public void StartExecution(CancellationToken token)
		{
			Task.Run(() => ExecuteAsync(token), CancellationToken.None);
		}

		private async void ExecuteAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				if (_worker != null)
				{
					_worker.DoWork(token);
				}

				if (_asyncWorker != null)
				{
					await _asyncWorker.DoWork(token);
				}

				await Task.Delay(_interval, token);
			}
		}
	}
}
