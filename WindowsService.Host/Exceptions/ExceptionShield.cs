using System;

using WindowsService.Host.Extensions;

using Common.Log;



namespace WindowsService.Host.Exceptions
{
	public class ExceptionShield : IExceptionShield
	{
		private readonly ILog _log;

		public ExceptionShield(ILog log)
		{
			_log = log;
		}

		public void Process(Action action)
		{
			Process<object>(
				() =>
				{
					action();
					return null;
				});
		}

		public T Process<T>(Func<T> func)
		{
			try
			{
				return func();
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
