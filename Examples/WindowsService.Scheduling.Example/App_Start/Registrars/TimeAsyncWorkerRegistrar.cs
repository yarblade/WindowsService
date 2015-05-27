using System.Configuration;

using WindowsService.Scheduling.Example.Repositories;
using WindowsService.Scheduling.Example.Web;
using WindowsService.Scheduling.Example.Workers;
using WindowsService.Scheduling.Settings;
using WindowsService.Scheduling.Unity.Registrars;

using Microsoft.Practices.Unity;



namespace WindowsService.Scheduling.Example.Registrars
{
	internal static class TimeAsyncWorkerRegistrar
	{
		public static void Register(IUnityContainer container)
		{
			container.RegisterType<ICityRepository, CityRepository>(WorkerNames.TimeAsyncWorker, new ContainerControlledLifetimeManager());
		    container.RegisterInstance(
		        WorkerNames.TimeAsyncWorker,
		        (IWorkerSettings)ConfigurationManager.GetSection("timeAsyncWorkerSettings"));

			WorkerRegistrar.Register(
				container,
				WorkerNames.TimeAsyncWorker,
				c => new TimeAsyncWorker(
					c.Resolve<ICityRepository>(WorkerNames.TimeAsyncWorker),
					c.Resolve<IHttpClient>(),
					Settings.CitiesPerRequest,
					Settings.TimeAsyncWorkerFileName));
		}
	}
}
