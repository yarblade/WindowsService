using Microsoft.Practices.Unity;



namespace WindowsService.Example
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			using (var container = new UnityContainer())
			{
				Bootstrapper.Initialize(container);

				container.Resolve<Host.Service.WindowsService>().Run();
			}
		}
	}
}
