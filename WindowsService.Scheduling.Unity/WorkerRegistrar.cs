﻿using System;

using WindowsService.Host.Sandboxes;
using WindowsService.Host.Workers;
using WindowsService.Scheduling.Sandboxes;
using WindowsService.Scheduling.Schedulers;
using WindowsService.Scheduling.Settings;
using WindowsService.Scheduling.Unity.WorkerRunners;

using Common.Log;

using Microsoft.Practices.Unity;



namespace WindowsService.Scheduling.Unity
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

		private static void RegisterSandbox<T>(IUnityContainer container, string workerName)
		{
			container.RegisterType<IWorkerSandbox>(
				workerName,
				new InjectionFactory(
					c => new ScheduledWorkerSandbox<T>(workerName, new StatelessWorkerRunner<T>(c), c.Resolve<IScheduler<T>>(workerName), c.Resolve<ILog>())));
		}

		private static void RegisterScheduler<T>(IUnityContainer container, string workerName)
		{
			container.RegisterType<IScheduler<Loading.Loading>>(workerName, new InjectionFactory(c => new Scheduler(c.Resolve<WorkerSettings>(workerName))));
		}
	}
}