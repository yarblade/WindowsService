using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Workers;
using WindowsService.Scheduling.Example.Entities;
using WindowsService.Scheduling.Example.Repositories;
using WindowsService.Scheduling.Example.Web;
using WindowsService.Scheduling.Loading;

using Newtonsoft.Json;



namespace WindowsService.Scheduling.Example.Workers
{
	internal class TimeAsyncWorker : IAsyncWorker<Loading.Loading>
	{
		private readonly int _citiesPerRequest;
		private readonly ICityRepository _cityRepository;
		private readonly IHttpClient _client;
		private readonly string _fileName;

		public TimeAsyncWorker(ICityRepository cityRepository, IHttpClient client, int citiesPerRequest, string fileName)
		{
			_cityRepository = cityRepository;
			_client = client;
			_citiesPerRequest = citiesPerRequest;
			_fileName = fileName;
		}

		public async Task<Loading.Loading> DoWork(CancellationToken token)
		{
			var ids = _cityRepository.GetCityIds(_citiesPerRequest);
			var processed = 0;
			token.ThrowIfCancellationRequested();

			foreach (var id in ids)
			{
				var json = await _client.DownloadStringTaskAsync(string.Format("https://time.yandex.ru/sync.json?geo={0}", id));
				var time = JsonConvert.DeserializeObject<UtcTimeWithClocks>(json);

				if (time != null && time.Clocks.ContainsKey(id))
				{
					using (var writer = File.AppendText(_fileName))
					{
						var cityClock = time.Clocks[id];

						await writer.WriteLineAsync(
							string.Format("{0:s} : {2:00000000000} : {1}", time.DateTime + TimeSpan.FromMilliseconds(cityClock.Offset), cityClock.Name, cityClock.Id));

						processed++;
					}
				}
			}

			return LoadingCalculator.Calculate(processed, _citiesPerRequest);
		}
	}
}
