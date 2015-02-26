using System;

using WindowsService.Host.Scheduling;
using WindowsService.Host.Settings;
using WindowsService.Host.Workers;

using log4net;

using Microsoft.Practices.Unity;



namespace WindowsService.Host.Registrars
{
	public static class WorkerRegistrar
	{
		public static void Register<T>(IUnityContainer container, string workerName, Func<IUnityContainer, IWorker<T>> resolver)
		{
			container.RegisterType<IWorker<T>>(workerName, new InjectionFactory(resolver));

			RegisterScheduler<T>(container, workerName);
			RegisterSandbox<T>(container, workerName);
		}

		public static void Register<T>(IUnityContainer container, string workerName, Func<IUnityContainer, IAsyncWorker<T>> resolver)
		{
			container.RegisterType<IAsyncWorker<T>>(workerName, new InjectionFactory(resolver));

			RegisterScheduler<T>(container, workerName);
			RegisterSandbox<T>(container, workerName);
		}

		public static void RegisterSandbox<T>(IUnityContainer container, string workerName)
		{
			container.RegisterType<IWorkerSandbox>(
				workerName,
				new InjectionFactory(
					c => new ScheduledWorkerSandbox<T>(workerName, new WorkerRunner<T>(c), c.Resolve<IScheduler<T>>(workerName), c.Resolve<ILog>())));
		}

		public static void RegisterScheduler<T>(IUnityContainer container, string workerName)
		{
			container.RegisterType<IScheduler<Loading.Loading>>(workerName, new InjectionFactory(c => new Scheduler(c.Resolve<WorkerSettings>(workerName))));
		}
	}
}
