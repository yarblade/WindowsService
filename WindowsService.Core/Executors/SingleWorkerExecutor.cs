using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Entities;



namespace WindowsService.Core.Executors
{
	public class SingleWorkerExecutor : IWorkerExecutor
	{
		private readonly IWorkerExecutor _executor;
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(initialCount: 1);

		public SingleWorkerExecutor(IWorkerExecutor executor)
		{
			_executor = executor;
		}

		public async Task<Loading> ExecuteAsync(CancellationToken token)
		{
			if (!token.IsCancellationRequested)
			{
				var lockTaken = false;

				try
				{
					lockTaken = TryAcquireLock();
					if (lockTaken)
					{
						await _executor.ExecuteAsync(token);
					}
				}
				finally
				{
					if (lockTaken)
					{
						ReleaseLock();
					}
				}
			}

			return default(Loading);
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
