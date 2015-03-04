using System.Net;
using System.Text;
using System.Threading.Tasks;



namespace WindowsService.Scheduling.Unity.Example.Web
{
	public class HttpClient : IHttpClient
	{
		public async Task<string> DownloadStringTaskAsync(string url)
		{
			try
			{
				return await new WebClient { Encoding = Encoding.UTF8 }.DownloadStringTaskAsync(url);
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					return string.Empty;
				}

				throw;
			}
		}

		public string DownloadString(string url)
		{
			try
			{
				return new WebClient { Encoding = Encoding.UTF8 }.DownloadString(url);
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					return string.Empty;
				}

				throw;
			}
		}
	}
}
