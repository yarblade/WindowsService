using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Host.Workers;
using WindowsService.Scheduling.Loading;
using WindowsService.Scheduling.Unity.Example.Entities;
using WindowsService.Scheduling.Unity.Example.Repositories;

using Newtonsoft.Json;



namespace WindowsService.Scheduling.Unity.Example.Workers
{
	internal class TimeAsyncWorker : IAsyncWorker<Loading.Loading>
	{
		private readonly int _citiesPerRequest;
		private readonly ICityRepository _cityRepository;
		private readonly string _fileName;

		public TimeAsyncWorker(ICityRepository cityRepository, int citiesPerRequest, string fileName)
		{
			_cityRepository = cityRepository;
			_citiesPerRequest = citiesPerRequest;
			_fileName = fileName;
		}

		public async Task<Loading.Loading> DoWork(CancellationToken token)
		{
			var ids = _cityRepository.GetCityIds(_citiesPerRequest);
			var processed = 0;

			foreach (var id in ids)
			{
				token.ThrowIfCancellationRequested();

				var client = new WebClient { Encoding = Encoding.UTF8 };
				var json = await client.DownloadStringTaskAsync(new Uri(string.Format("https://time.yandex.ru/sync.json?geo={0}", id)));
				var time = JsonConvert.DeserializeObject<UtcTimeWithClocks>(json);

				if (time.Clocks.ContainsKey(id))
				{
					using (var writer = File.AppendText(_fileName))
					{
						var cityClock = time.Clocks[id];

						await writer.WriteLineAsync(
							string.Format("{0:s} : {2:00000000000} : {1}", time.Time + TimeSpan.FromMilliseconds(cityClock.Offset), cityClock.Name, cityClock.Id));

						processed++;
					}
				}
			}

			return LoadingCalculator.Calculate(processed, _citiesPerRequest);
		}
	}
}
