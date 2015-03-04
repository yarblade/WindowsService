using WindowsService.Scheduling.Settings;
using WindowsService.Scheduling.Unity.Example.Repositories;
using WindowsService.Scheduling.Unity.Example.Web;
using WindowsService.Scheduling.Unity.Example.Workers;

using Microsoft.Practices.Unity;

using SettingsReader;



namespace WindowsService.Scheduling.Unity.Example.Registrars
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
				c => new TimeWorker(
					c.Resolve<ICityRepository>(WorkerNames.TimeWorker),
					c.Resolve<IHttpClient>(),
					Settings.CitiesPerRequest,
					Settings.TimeWorkerFileName));
		}
	}
}
