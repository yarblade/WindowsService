using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Extensions;
using WindowsService.Core.Sandboxes;

using Common.Log;



namespace WindowsService.Core.Service
{
	public class WindowsService : ServiceBase
	{
		private readonly ILog _log;
		private readonly IWorkerSandbox[] _sandboxes;
		private readonly CancellationTokenSource _tokenSource;

		public WindowsService(IWorkerSandbox[] sandboxes, ILog log)
		{
			_sandboxes = sandboxes;
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

			HandleException(
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

            HandleException(
				() =>
				{
					_tokenSource.Cancel();

					Task.WaitAll(_sandboxes.Select(x => x.Completion).ToArray());
				});

			_log.Info("Service stopped.");
		}

        public void HandleException(Action action)
        {
            try
            {
                action();
            }
            catch (AggregateException ex)
            {
                ex.Handle(
                    x =>
                    {
                        if (!x.IsCritical())
                        {
                            _log.Fatal("Unexpected error: " + x);
                        }

                        return false;
                    });

                throw;
            }
            catch (Exception ex)
            {
                if (!ex.IsCritical())
                {
                    _log.Fatal("Unexpected error: " + ex);
                }

                throw;
            }
        }
	}
}
