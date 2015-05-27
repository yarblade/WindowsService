using WindowsService.Scheduling.Example.Registrars;
using WindowsService.Scheduling.Example.Web;
using WindowsService.Scheduling.Unity.Registrars;

using Common.Logging;

using Microsoft.Practices.Unity;



namespace WindowsService.Scheduling.Example
{
    internal static class Bootstrapper
    {
        public static void Initialize(IUnityContainer container)
        {
            container.RegisterType<ILog>(new InjectionFactory(c => LogManager.GetLogger("LOG")));
            container.RegisterType<IHttpClient, HttpClient>();

            TimeAsyncWorkerRegistrar.Register(container);
            TimeWorkerRegistrar.Register(container);
            WindowsServiceRegistrar.Register(container);
        }
    }
}
