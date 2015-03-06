using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Exceptions;
using WindowsService.Core.Sandboxes;

using Common.Log;



namespace WindowsService.Core.Service
{
	public class WindowsService : ServiceBase
	{
		private readonly ILog _log;
		private readonly IWorkerSandbox[] _sandboxes;
		private readonly IExceptionShield _shield;
		private readonly CancellationTokenSource _tokenSource;

		public WindowsService(IWorkerSandbox[] sandboxes, IExceptionShield shield, ILog log)
		{
			_sandboxes = sandboxes;
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
					foreach (var sandbox in _sandboxes)
					{
						sandbox.StartWorkerExecution(_tokenSource.Token);
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

					Task.WaitAll(_sandboxes.Select(x => x.Completion).ToArray());
				});

			_log.Info("Service stopped.");
		}
	}
}
