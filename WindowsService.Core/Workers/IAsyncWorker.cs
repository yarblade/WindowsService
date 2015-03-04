using System.Threading;
using System.Threading.Tasks;



namespace WindowsService.Core.Workers
{
	public interface IAsyncWorker<T>
	{
		Task<T> DoWork(CancellationToken token);
	}
}
