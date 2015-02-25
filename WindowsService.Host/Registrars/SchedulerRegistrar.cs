using WindowsService.Host.Scheduling;
using WindowsService.Host.Settings;

using Microsoft.Practices.Unity;



namespace WindowsService.Host.Registrars
{
	public static class SchedulerRegistrar
	{
		public static void Register(IUnityContainer container, string workerName, WorkerSettings settings)
		{
			container.RegisterType<IScheduler>(workerName, new InjectionFactory(c => new Scheduler(settings)));
		}
	}
}
