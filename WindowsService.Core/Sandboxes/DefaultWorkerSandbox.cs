using System;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Workers;



namespace WindowsService.Core.Sandboxes
{
	public class DefaultWorkerSandbox<T> : IWorkerSandbox
	{
		private readonly TimeSpan _interval;
		private readonly string _workerName;
		private readonly IWorkerRunner<T> _workerRunner;
		private Task _task;

		public DefaultWorkerSandbox(IWorkerRunner<T> workerRunner, string workerName, TimeSpan interval)
		{
			_workerRunner = workerRunner;
			_workerName = workerName;
			_interval = interval;
		}

		public void StartWorkerExecution(CancellationToken token)
		{
			_task = Task.Run(() => ExecuteAsync(token), CancellationToken.None);
		}

		private async void ExecuteAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				await _workerRunner.RunAsync(_workerName, token);

				await Task.Delay(_interval, token);
			}
		}

		public void Dispose()
		{
			if (_task != null)
			{
				_task.Wait();
				_task = null;
			}
		}
	}
}
