using System.Threading;
using System.Threading.Tasks;

using WindowsService.Host.Entities;



namespace WindowsService.Host.Workers
{
	public interface IAsyncWorker
	{
		Task<Loading> DoWork(CancellationToken token);
	}
}
