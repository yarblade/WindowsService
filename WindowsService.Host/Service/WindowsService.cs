using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

using WindowsService.Host.Exceptions;
using WindowsService.Host.Executors;

using log4net;



namespace WindowsService.Host.Service
{
	public class WindowsService : ServiceBase
	{
		private readonly IExecutor[] _executors;
		private readonly ILog _log;
		private readonly IExceptionShield _shield;
		private readonly CancellationTokenSource _tokenSource;

		public WindowsService(IExecutor[] executors, IExceptionShield shield, ILog log)
		{
			_executors = executors;
			_shield = shield;
			_log = log;
			_tokenSource = new CancellationTokenSource();
		}

		public void Run()
		{
			if (Environment.UserInteractive)
			{
				OnStart(new string[0]);
				Console.WriteLine(@"Press any key to stop service");
				Console.ReadKey();
				OnStop();
			}
			else
			{
				Run(this);
			}
		}

		protected override void OnStart(string[] args)
		{
			_log.Info("Service starting.");

			_shield.Process(
				() =>
				{
					foreach (var executor in _executors)
					{
						executor.Execute(_tokenSource.Token);
					}
				});

			_log.Info("Service started.");
		}

		protected override void OnStop()
		{
			_log.Info("Service stopping.");

			_shield.Process(
				() =>
				{
					_tokenSource.Cancel();

					foreach (var executor in _executors.OfType<IDisposable>())
					{
						executor.Dispose();
					}
				});

			_log.Info("Service stopped.");
		}
	}
}
