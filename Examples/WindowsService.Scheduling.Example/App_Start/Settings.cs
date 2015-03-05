using System.Configuration;



namespace WindowsService.Scheduling.Example
{
	internal static class Settings
	{
		public static int CitiesPerRequest { get { return int.Parse(ConfigurationManager.AppSettings["CitiesPerRequest"]); } }

		public static string TimeAsyncWorkerFileName { get { return ConfigurationManager.AppSettings["TimeAsyncWorkerFileName"]; } }

		public static string TimeWorkerFileName { get { return ConfigurationManager.AppSettings["TimeWorkerFileName"]; } }
	}
}
