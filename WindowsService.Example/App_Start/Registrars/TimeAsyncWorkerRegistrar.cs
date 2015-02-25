﻿using System;

using WindowsService.Example.Repositories;
using WindowsService.Example.Workers;
using WindowsService.Host.Loading;
using WindowsService.Host.Registrars;
using WindowsService.Host.Settings;
using WindowsService.Host.Workers;

using Microsoft.Practices.Unity;



namespace WindowsService.Example.Registrars
{
	internal static class TimeAsyncWorkerRegistrar
	{
		public static void Register(IUnityContainer container)
		{
			container.RegisterType<ICityRepository, CityRepository>(WorkerNames.TimeAsyncWorker, new ContainerControlledLifetimeManager());

			container.RegisterType<IAsyncWorker<Loading>>(
				WorkerNames.TimeAsyncWorker,
				new InjectionFactory(
					c => new TimeAsyncWorker(c.Resolve<ICityRepository>(WorkerNames.TimeAsyncWorker), Settings.CitiesPerRequest, Settings.TimeAsyncWorkerFileName)));

			SchedulerRegistrar.Register(container,
				WorkerNames.TimeAsyncWorker,
				new WorkerSettings
				{
					FailureInterval = TimeSpan.FromMinutes(1),
					LoadingIntervals = new[]
					{
						new LoadingInterval { Loading = Loading.None, Interval = TimeSpan.FromSeconds(30) },
						new LoadingInterval { Loading = Loading.Medium, Interval = TimeSpan.FromSeconds(15) },
						new LoadingInterval { Loading = Loading.Full, Interval = TimeSpan.FromSeconds(5) }
					}
				});
			ScheduledExecutorRegistrar.Register<Loading>(container, WorkerNames.TimeAsyncWorker);
		}
	}
}
