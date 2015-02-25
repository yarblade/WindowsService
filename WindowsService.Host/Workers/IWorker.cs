using System.Threading;



namespace WindowsService.Host.Workers
{
	public interface IWorker<out T>
	{
		T DoWork(CancellationToken token);
	}
}
