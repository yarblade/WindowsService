using Microsoft.Practices.Unity;



namespace WindowsService.Scheduling.Unity.Example
{
	internal static class Program
	{
		private static void Main()
		{
			using (var container = new UnityContainer())
			{
				Bootstrapper.Initialize(container);

				container.Resolve<Core.Service.WindowsService>().Run();
			}
		}
	}
}
