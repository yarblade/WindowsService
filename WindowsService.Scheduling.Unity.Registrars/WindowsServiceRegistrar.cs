using System.Linq;

using WindowsService.Core.Sandboxes;

using Microsoft.Practices.Unity;



namespace WindowsService.Scheduling.Unity.Registrars
{
	public static class WindowsServiceRegistrar
	{
		public static void Register(IUnityContainer container)
		{
			container.RegisterType<IWorkerSandbox[]>(new InjectionFactory(c => c.ResolveAll<IWorkerSandbox>().ToArray()));
			container.RegisterType<Core.Service.WindowsService>();
		}
	}
}
