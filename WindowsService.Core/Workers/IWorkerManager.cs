using System;
using System.Threading;



namespace WindowsService.Core.Workers
{
	public interface IWorkerManager : IDisposable
	{
		void Run(CancellationToken token);
	}
}
