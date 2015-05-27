using System.Configuration;

using WindowsService.Scheduling.Example.Repositories;
using WindowsService.Scheduling.Example.Web;
using WindowsService.Scheduling.Example.Workers;
using WindowsService.Scheduling.Settings;
using WindowsService.Scheduling.Unity.Registrars;

using Microsoft.Practices.Unity;



namespace WindowsService.Scheduling.Example.Registrars
{
	internal static class TimeWorkerRegistrar
	{
		public static void Register(IUnityContainer container)
		{
			container.RegisterType<ICityRepository, CityRepository>(WorkerNames.TimeWorker, new ContainerControlledLifetimeManager());
            container.RegisterInstance(
                WorkerNames.TimeWorker,
                (IWorkerSettings)ConfigurationManager.GetSection("timeWorkerSettings"));

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
