using System.Threading;



namespace WindowsService.Host.Workers
{
	public interface IWorkerSandbox
	{
		void StartExecution(CancellationToken token);
	}
}
