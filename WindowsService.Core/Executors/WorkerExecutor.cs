using System;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Entities;
using WindowsService.Core.Workers;

using Microsoft.Practices.Unity;



namespace WindowsService.Core.Executors
{
	public class WorkerExecutor : IAsyncExecutor
	{
		private readonly IUnityContainer _container;
		private readonly string _workerName;

		public WorkerExecutor(IUnityContainer container, string workerName)
		{
			_container = container;
			_workerName = workerName;
		}

		public async Task<Loading> ExecuteAsync(CancellationToken token)
		{
			using (var childContainer = _container.CreateChildContainer())
			{
				if (childContainer.IsRegistered<IAsyncWorker>(_workerName))
				{
					return await childContainer.Resolve<IAsyncWorker>(_workerName).DoWork(token);
				}

				if (childContainer.IsRegistered<IWorker>(_workerName))
				{
					var worker = childContainer.Resolve<IWorker>(_workerName);
					return await Task.Run(() => worker.DoWork(token), CancellationToken.None);
				}

				throw new InvalidOperationException(string.Format("Can't resolve '{0}' worker", _workerName));
			}
		}
	}
}
