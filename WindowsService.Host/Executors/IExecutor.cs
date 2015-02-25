using System.Threading;



namespace WindowsService.Host.Executors
{
	public interface IExecutor
	{
		void Execute(CancellationToken token);
	}
}
