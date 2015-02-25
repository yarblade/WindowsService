using System.Threading;

using WindowsService.Host.Entities;



namespace WindowsService.Host.Workers
{
	public interface IWorker
	{
		Loading DoWork(CancellationToken token);
	}
}
