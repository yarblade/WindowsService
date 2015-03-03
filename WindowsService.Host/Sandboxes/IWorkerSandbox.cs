using System.Threading;



namespace WindowsService.Host.Sandboxes
{
	public interface IWorkerSandbox
	{
		void StartExecution(CancellationToken token);
	}
}
