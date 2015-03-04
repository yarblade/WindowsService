using System.Threading.Tasks;



namespace WindowsService.Scheduling.Unity.Example.Web
{
	public interface IHttpClient
	{
		Task<string> DownloadStringTaskAsync(string url);
		string DownloadString(string url);
	}
}