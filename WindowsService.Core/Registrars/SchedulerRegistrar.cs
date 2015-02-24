using WindowsService.Core.Scheduling;
using WindowsService.Core.Settings;

using Microsoft.Practices.Unity;



namespace WindowsService.Core.Registrars
{
	public static class SchedulerRegistrar
	{
		public static void Register(IUnityContainer container, string workerName, WorkerSettings settings)
		{
			container.RegisterType<IScheduler>(workerName, new InjectionFactory(c => new Scheduler(settings)));
		}
	}
}
