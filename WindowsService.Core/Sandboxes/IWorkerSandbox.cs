using System;
using System.Threading;



namespace WindowsService.Core.Sandboxes
{
	public interface IWorkerSandbox : IDisposable
	{
		void StartWorkerExecution(CancellationToken token);
	}
}
