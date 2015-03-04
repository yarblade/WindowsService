using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Workers;

using Microsoft.Practices.Unity;



namespace WindowsService.Scheduling.Unity
{
	public class StatelessWorkerRunner<T> : IWorkerRunner<T>
	{
		private readonly IUnityContainer _container;

		public StatelessWorkerRunner(IUnityContainer container)
		{
			_container = container;
		}

		public async Task<T> RunAsync(string workerName, CancellationToken token)
		{
			using (var childContainer = _container.CreateChildContainer())
			{
				return await childContainer.Resolve<IAsyncWorker<T>>(workerName).DoWork(token);
			}
		}
	}
}
