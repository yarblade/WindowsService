using System;



namespace WindowsService.Core.Exceptions
{
	public interface IExceptionShield
	{
		void Process(Action action);
		T Process<T>(Func<T> func);
	}
}
