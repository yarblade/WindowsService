using System;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Workers;



namespace WindowsService.Core.Sandboxes
{
	public class DefaultWorkerSandbox<T> : IWorkerSandbox
	{
		private readonly TaskCompletionSource<bool> _completionSource;
		private readonly TimeSpan _interval;
		private readonly string _workerName;
		private readonly IWorkerRunner<T> _workerRunner;

		public DefaultWorkerSandbox(IWorkerRunner<T> workerRunner, string workerName, TimeSpan interval)
		{
			_workerRunner = workerRunner;
			_workerName = workerName;
			_interval = interval;
			_completionSource = new TaskCompletionSource<bool>();
		}

		public Task Completion { get { return _completionSource.Task; } }

		public void StartWorkerExecution(CancellationToken token)
		{
			Task.Run(() => ExecuteAsync(token), CancellationToken.None);
		}

		private async Task ExecuteAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				await _workerRunner.RunAsync(_workerName, token);

				if (token.IsCancellationRequested)
				{
					break;
				}

				await Task.Delay(_interval, CancellationToken.None);
			}

			_completionSource.SetResult(true);
		}
	}
}
