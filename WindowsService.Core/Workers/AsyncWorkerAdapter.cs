using System.Threading;
using System.Threading.Tasks;



namespace WindowsService.Core.Workers
{
	public class AsyncWorkerAdapter<T> : IAsyncWorker<T>
	{
		private readonly IWorker<T> _worker;

		public AsyncWorkerAdapter(IWorker<T> worker)
		{
			_worker = worker;
		}

		public async Task<T> DoWork(CancellationToken token)
		{
			return await Task.Run(() => _worker.DoWork(token), CancellationToken.None);
		}
	}
}
