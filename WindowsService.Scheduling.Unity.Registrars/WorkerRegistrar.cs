﻿using System;

using WindowsService.Core.Sandboxes;
using WindowsService.Core.Timers;
using WindowsService.Core.Workers;
using WindowsService.Scheduling.Sandboxes;
using WindowsService.Scheduling.Schedulers;
using WindowsService.Scheduling.Settings;
using WindowsService.Unity;

using Common.Logging;

using Microsoft.Practices.Unity;



namespace WindowsService.Scheduling.Unity.Registrars
{
	public static class WorkerRegistrar
	{
		public static void Register<T>(IUnityContainer container, string workerName, Func<IUnityContainer, IWorker<T>> resolver)
		{
			container.RegisterType<IAsyncWorker<T>>(workerName, new InjectionFactory(c => new AsyncWorkerAdapter<T>(resolver(c))));

			RegisterScheduler<T>(container, workerName);
			RegisterSandbox<T>(container, workerName);
		}

		public static void Register<T>(IUnityContainer container, string workerName, Func<IUnityContainer, IAsyncWorker<T>> resolver)
		{
			container.RegisterType<IAsyncWorker<T>>(workerName, new InjectionFactory(resolver));

			RegisterScheduler<T>(container, workerName);
			RegisterSandbox<T>(container, workerName);
		}

		private static void RegisterSandbox<T>(IUnityContainer container, string workerName)
		{
			container.RegisterType<ITimer, SingleThreadTimer>();
			container.RegisterType<IWorkerSandbox>(
				workerName,
				new InjectionFactory(
					c => new ScheduledWorkerSandbox<T>(workerName,
						new SingleThreadTimer(),
						new StatelessWorkerRunner<T>(c),
						c.Resolve<IScheduler<T>>(workerName),
						c.Resolve<ILog>())));
		}

		private static void RegisterScheduler<T>(IUnityContainer container, string workerName)
		{
			container.RegisterType<IScheduler<T>>(workerName, new InjectionFactory(c => new Scheduler(c.Resolve<IWorkerSettings>(workerName))));
		}
	}
}
