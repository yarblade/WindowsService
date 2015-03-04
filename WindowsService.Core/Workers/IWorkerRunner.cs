using System.Threading;
using System.Threading.Tasks;



namespace WindowsService.Core.Workers
{
	public interface IWorkerRunner<T>
	{
		Task<T> RunAsync(string workerName, CancellationToken token);
	}
}
