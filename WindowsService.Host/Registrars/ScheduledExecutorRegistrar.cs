using WindowsService.Host.Executors;
using WindowsService.Host.Scheduling;

using log4net;

using Microsoft.Practices.Unity;



namespace WindowsService.Host.Registrars
{
	public static class ScheduledExecutorRegistrar
	{
		public static void Register(IUnityContainer container, string workerName)
		{
			container.RegisterType<IAsyncExecutor>(workerName, new InjectionFactory(c => new WorkerExecutor(c, workerName)));

			container.RegisterType<IExecutor>(
				workerName,
				new InjectionFactory(c => new ScheduledExecutor(c.Resolve<IAsyncExecutor>(workerName), c.Resolve<IScheduler>(workerName), workerName, c.Resolve<ILog>())));
		}
	}
}
