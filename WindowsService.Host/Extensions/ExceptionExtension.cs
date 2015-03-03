using System;
using System.Threading;



namespace WindowsService.Host.Extensions
{
	public static class ExceptionExtension
	{
		public static bool IsCritical(this Exception ex)
		{
			return ex is StackOverflowException || ex is OutOfMemoryException || ex is ThreadAbortException || ex is AccessViolationException;
		}
	}
}
