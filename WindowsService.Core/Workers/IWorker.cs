using System.Threading;



namespace WindowsService.Core.Workers
{
	public interface IWorker<out T>
	{
		T DoWork(CancellationToken token);
	}
}
