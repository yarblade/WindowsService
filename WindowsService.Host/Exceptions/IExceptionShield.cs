using System;



namespace WindowsService.Host.Exceptions
{
	public interface IExceptionShield
	{
		void Process(Action action);
		T Process<T>(Func<T> func);
	}
}
