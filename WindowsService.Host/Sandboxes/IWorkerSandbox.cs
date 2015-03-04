using System;
using System.Threading;



namespace WindowsService.Host.Sandboxes
{
	public interface IWorkerSandbox : IDisposable
	{
		void StartWorkerExecution(CancellationToken token);
	}
}
