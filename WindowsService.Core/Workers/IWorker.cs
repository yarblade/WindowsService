using System.Threading;

using WindowsService.Core.Entities;



namespace WindowsService.Core.Workers
{
	public interface IWorker
	{
		Loading DoWork(CancellationToken token);
	}
}
