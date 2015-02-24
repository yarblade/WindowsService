using System.Threading;



namespace WindowsService.Core.Executors
{
	public interface IExecutor
	{
		void Execute(CancellationToken token);
	}
}
