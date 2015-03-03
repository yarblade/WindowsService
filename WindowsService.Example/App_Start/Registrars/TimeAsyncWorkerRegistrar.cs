using WindowsService.Example.Repositories;
using WindowsService.Example.Workers;
using WindowsService.Scheduling.Settings;
using WindowsService.Scheduling.Unity;

using Microsoft.Practices.Unity;

using SettingsReader;



namespace WindowsService.Example.Registrars
{
	internal static class TimeAsyncWorkerRegistrar
	{
		public static void Register(IUnityContainer container)
		{
			container.RegisterType<ICityRepository, CityRepository>(WorkerNames.TimeAsyncWorker, new ContainerControlledLifetimeManager());
			container.RegisterType<WorkerSettings>(
				WorkerNames.TimeAsyncWorker,
				new InjectionFactory(c => c.Resolve<ISettingsReader>().Read<WorkerSettings>("timeAsyncWorkerSettings")));

			WorkerRegistrar.Register(
				container,
				WorkerNames.TimeAsyncWorker,
				c => new TimeAsyncWorker(c.Resolve<ICityRepository>(WorkerNames.TimeAsyncWorker), Settings.CitiesPerRequest, Settings.TimeAsyncWorkerFileName));
		}
	}
}
