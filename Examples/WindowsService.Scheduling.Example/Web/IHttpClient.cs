using System.Threading.Tasks;



namespace WindowsService.Scheduling.Example.Web
{
	public interface IHttpClient
	{
		Task<string> DownloadStringTaskAsync(string url);
		string DownloadString(string url);
	}
}