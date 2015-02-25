using System;

using WindowsService.Example.Repositories;
using WindowsService.Example.Workers;
using WindowsService.Host.Entities;
using WindowsService.Host.Registrars;
using WindowsService.Host.Settings;
using WindowsService.Host.Workers;

using Microsoft.Practices.Unity;



namespace WindowsService.Example.Registrars
{
	internal static class TimeWorkerRegistrar
	{
		public static void Register(IUnityContainer container)
		{
			container.RegisterType<ICityRepository, CityRepository>(WorkerNames.TimeWorker, new ContainerControlledLifetimeManager());

			container.RegisterType<IWorker>(
				WorkerNames.TimeWorker,
				new InjectionFactory(
					c => new TimeWorker(c.Resolve<ICityRepository>(WorkerNames.TimeWorker), Settings.CitiesPerRequest, Settings.TimeWorkerFileName)));

			SchedulerRegistrar.Register(container,
				WorkerNames.TimeWorker,
				new WorkerSettings
				{
					LoadingIntervals = new[]
					{
						new LoadingInterval { Loading = Loading.None, Interval = TimeSpan.FromSeconds(10) },
						new LoadingInterval { Loading = Loading.Medium, Interval = TimeSpan.FromSeconds(5) },
						new LoadingInterval { Loading = Loading.Full, Interval = TimeSpan.FromSeconds(1) },
						new LoadingInterval { Loading = Loading.Fail, Interval = TimeSpan.FromMinutes(1) },
					}
				}); 
			ScheduledExecutorRegistrar.Register(container, WorkerNames.TimeWorker);
		}
	}
}
