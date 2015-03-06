using System.Threading;
using System.Threading.Tasks;



namespace WindowsService.Core.Sandboxes
{
	public interface IWorkerSandbox
	{
		Task Completion { get; }
		void StartWorkerExecution(CancellationToken token);
	}
}
