using WindowsService.Example.Repositories;
using WindowsService.Example.Workers;
using WindowsService.Scheduling.Settings;
using WindowsService.Scheduling.Unity;

using Microsoft.Practices.Unity;

using SettingsReader;



namespace WindowsService.Example.Registrars
{
	internal static class TimeWorkerRegistrar
	{
		public static void Register(IUnityContainer container)
		{
			container.RegisterType<ICityRepository, CityRepository>(WorkerNames.TimeWorker, new ContainerControlledLifetimeManager());
			container.RegisterType<WorkerSettings>(
				WorkerNames.TimeWorker,
				new InjectionFactory(c => c.Resolve<ISettingsReader>().Read<WorkerSettings>("timeWorkerSettings")));


			WorkerRegistrar.Register(
				container,
				WorkerNames.TimeWorker,
				c => new TimeWorker(c.Resolve<ICityRepository>(WorkerNames.TimeWorker), Settings.CitiesPerRequest, Settings.TimeWorkerFileName));
		}
	}
}
