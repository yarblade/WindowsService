using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Entities;



namespace WindowsService.Core.Workers
{
	public interface IAsyncWorker
	{
		Task<Loading> DoWork(CancellationToken token);
	}
}
