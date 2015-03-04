using System;
using System.IO;
using System.Threading;

using WindowsService.Core.Workers;
using WindowsService.Scheduling.Loading;
using WindowsService.Scheduling.Unity.Example.Entities;
using WindowsService.Scheduling.Unity.Example.Repositories;
using WindowsService.Scheduling.Unity.Example.Web;

using Newtonsoft.Json;



namespace WindowsService.Scheduling.Unity.Example.Workers
{
	internal class TimeWorker : IWorker<Loading.Loading>
	{
		private readonly int _citiesPerRequest;
		private readonly ICityRepository _cityRepository;
		private readonly IHttpClient _client;
		private readonly string _fileName;

		public TimeWorker(ICityRepository cityRepository, IHttpClient client, int citiesPerRequest, string fileName)
		{
			_cityRepository = cityRepository;
			_client = client;
			_citiesPerRequest = citiesPerRequest;
			_fileName = fileName;
		}

		public Loading.Loading DoWork(CancellationToken token)
		{
			token.ThrowIfCancellationRequested();

			var ids = _cityRepository.GetCityIds(_citiesPerRequest);
			var json = _client.DownloadString(string.Format("https://time.yandex.ru/sync.json?geo={0}", string.Join(",", ids)));
			var time = JsonConvert.DeserializeObject<UtcTimeWithClocks>(json);

			if (time == null)
			{
				return Loading.Loading.None;
			}
			
			using (var writer = File.AppendText(_fileName))
			{
				foreach (var pair in time.Clocks)
				{
					writer.WriteLine("{0:s} : {2:00000000000} : {1}", time.DateTime + TimeSpan.FromMilliseconds(pair.Value.Offset), pair.Value.Name, pair.Value.Id);
				}
			}

			return LoadingCalculator.Calculate(time.Clocks.Count, _citiesPerRequest);
		}
	}
}
