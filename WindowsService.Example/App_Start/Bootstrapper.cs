using System.Reflection;

using WindowsService.Example.Registrars;
using WindowsService.Host.Exceptions;
using WindowsService.Host.Workers;

using log4net;
using log4net.Config;

using Microsoft.Practices.Unity;



namespace WindowsService.Example
{
	internal static class Bootstrapper
	{
		public static void Initialize(IUnityContainer container)
		{
			XmlConfigurator.Configure();

			container.RegisterType<ILog>(new InjectionFactory(c => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType)));
			container.RegisterType<IExceptionShield, ExceptionShield>();

			TimeAsyncWorkerRegistrar.Register(container);
			TimeWorkerRegistrar.Register(container);

			container.RegisterType<IWorkerSandbox[]>(
				new InjectionFactory(
					c => new[]
					{
						c.Resolve<IWorkerSandbox>(WorkerNames.TimeAsyncWorker),
						c.Resolve<IWorkerSandbox>(WorkerNames.TimeWorker)
					}));
			container.RegisterType<Host.Service.WindowsService>();
		}
	}
}
