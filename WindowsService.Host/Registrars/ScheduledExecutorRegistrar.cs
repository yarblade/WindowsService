using WindowsService.Host.Executors;
using WindowsService.Host.Scheduling;
using WindowsService.Host.Workers;

using log4net;

using Microsoft.Practices.Unity;



namespace WindowsService.Host.Registrars
{
	public static class ScheduledExecutorRegistrar
	{
		public static void Register<T>(IUnityContainer container, string workerName)
		{
			container.RegisterType<IWorkerRunner<T>>(workerName, new InjectionFactory(c => new WorkerRunner<T>(c)));

			container.RegisterType<IExecutor>(
				workerName,
				new InjectionFactory(
					c => new ScheduledExecutor<T>(workerName, c.Resolve<IWorkerRunner<T>>(workerName), c.Resolve<IScheduler<T>>(workerName), c.Resolve<ILog>())));
		}
	}
}
