using System.Threading;
using System.Threading.Tasks;



namespace WindowsService.Host.Workers
{
	public interface IWorkerRunner<T>
	{
		Task<T> RunAsync(string workerName, CancellationToken token);
	}
}
