using System.Threading;
using System.Threading.Tasks;

using WindowsService.Host.Entities;



namespace WindowsService.Host.Executors
{
	public interface IAsyncExecutor
	{
		Task<Loading> ExecuteAsync(CancellationToken token);
	}
}
