using System.Threading;
using System.Threading.Tasks;



namespace WindowsService.Scheduling.WorkerRunners
{
	public interface IWorkerRunner<T>
	{
		Task<T> RunAsync(string workerName, CancellationToken token);
	}
}
