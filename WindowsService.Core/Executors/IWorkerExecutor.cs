using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Entities;



namespace WindowsService.Core.Executors
{
	public interface IWorkerExecutor
	{
		Task<Loading> ExecuteAsync(CancellationToken token);
	}
}
