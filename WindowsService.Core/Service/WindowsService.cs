using System;
using System.ServiceProcess;
using System.Threading;

using WindowsService.Core.Extensions;
using WindowsService.Core.Workers;

using log4net;



namespace WindowsService.Core.Service
{
	public class WindowsService : ServiceBase
	{
		private readonly IWorkerManager _workerManager;
		private readonly ILog _log;
		private readonly CancellationTokenSource _tokenSource;
		
		public WindowsService(IWorkerManager workerManager, ILog log)
		{
			_workerManager = workerManager;
			_log = log;
			_tokenSource = new CancellationTokenSource();
		}

		public void Run()
		{
			if (Environment.UserInteractive)
			{
				_workerManager.Run(_tokenSource.Token);
				Console.WriteLine(@"Press any key to stop service");
				Console.ReadKey();
				_tokenSource.Cancel();
			}
			else
			{
				ServiceBase.Run(this);
			}
		}

		protected override void OnStart(string[] args)
		{
			_log.Info("Service starting.");

			try
			{
				_workerManager.Run(_tokenSource.Token);
			}
			catch (Exception ex)
			{
				if (!ex.IsCritical())
				{
					_log.Error(ex.ToString());
				}

				throw;
			}

			_log.Info("Service started.");
		}

		protected override void OnStop()
		{
			_log.Info("Service stopping.");

			try
			{
				_tokenSource.Cancel();
				_workerManager.Dispose();
			}
			catch (Exception ex)
			{
				if (!ex.IsCritical())
				{
					_log.Error(ex.ToString());
				}

				throw;
			}

			_log.Info("Service stopped.");
		}
	}
}
