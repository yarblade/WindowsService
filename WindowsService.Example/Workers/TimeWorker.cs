using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

using WindowsService.Core.Calculators;
using WindowsService.Core.Entities;
using WindowsService.Core.Workers;
using WindowsService.Example.Entities;
using WindowsService.Example.Repositories;

using Newtonsoft.Json;



namespace WindowsService.Example.Workers
{
	internal class TimeWorker : IWorker
	{
		private readonly ICityRepository _cityRepository;
		private readonly int _citiesPerRequest;
		private readonly string _fileName;

		public TimeWorker(ICityRepository cityRepository, int citiesPerRequest, string fileName)
		{
			_cityRepository = cityRepository;
			_citiesPerRequest = citiesPerRequest;
			_fileName = fileName;
		}

		public Loading DoWork(CancellationToken token)
		{
			token.ThrowIfCancellationRequested();

			var ids = _cityRepository.GetCityIds(_citiesPerRequest);
			var client = new WebClient { Encoding = Encoding.UTF8 };
			var json = client.DownloadString(string.Format("https://time.yandex.ru/sync.json?geo={0}", string.Join(",", ids)));
			var time = JsonConvert.DeserializeObject<UtcTimeWithClocks>(json);

			using (var writer = File.AppendText(_fileName))
			{
				foreach (var pair in time.Clocks)
				{
					writer.WriteLine("{0:s} : {2:00000000000} : {1}", time.Time + TimeSpan.FromMilliseconds(pair.Value.Offset), pair.Value.Name, pair.Value.Id);
				}
			}

			return LoadingCalculator.Calculate(time.Clocks.Count, _citiesPerRequest);
		}
	}
}
