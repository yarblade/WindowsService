using System;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Host.Workers;
using WindowsService.Scheduling.WorkerRunners;

using Microsoft.Practices.Unity;



namespace WindowsService.Scheduling.Unity.WorkerRunners
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
				if (childContainer.IsRegistered<IAsyncWorker<T>>(workerName))
				{
					return await childContainer.Resolve<IAsyncWorker<T>>(workerName).DoWork(token);
				}

				if (childContainer.IsRegistered<IWorker<T>>(workerName))
				{
					var worker = childContainer.Resolve<IWorker<T>>(workerName);
					return await Task.Run(() => worker.DoWork(token), CancellationToken.None);
				}

				throw new InvalidOperationException(string.Format("Can't resolve '{0}' worker", workerName));
			}
		}
	}
}
