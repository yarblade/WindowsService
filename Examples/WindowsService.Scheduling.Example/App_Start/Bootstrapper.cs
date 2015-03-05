using System.Reflection;

using WindowsService.Scheduling.Example.Registrars;
using WindowsService.Scheduling.Example.Web;
using WindowsService.Scheduling.Unity.Registrars;

using Common.Log.log4net;

using log4net;
using log4net.Config;

using Microsoft.Practices.Unity;

using SettingsReader;
using SettingsReader.Readers;



namespace WindowsService.Scheduling.Example
{
	internal static class Bootstrapper
	{
		public static void Initialize(IUnityContainer container)
		{
			XmlConfigurator.Configure();

			container.RegisterType<Common.Log.ILog>(new InjectionFactory(c => new LogAdapter(LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType))));
			container.RegisterType<ISettingsReader, ConfigurationSectionReader>();
			container.RegisterType<IHttpClient, HttpClient>();

			TimeAsyncWorkerRegistrar.Register(container);
			TimeWorkerRegistrar.Register(container);
			WindowsServiceRegistrar.Register(container);
		}
	}
}
