using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using WindowsService.Core.Exceptions;

using Common.Log;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;



namespace WindowsService.Core.UnitTests.Exceptions
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ExceptionShieldTests
	{
		private readonly Mock<ILog> _log = new Mock<ILog>();
		private IExceptionShield _shield;

		[TestInitialize]
		public void TestInit()
		{
			_shield = new ExceptionShield(_log.Object);
		}

		[TestMethod]
		[ExpectedException(typeof(OutOfMemoryException))]
		public void ProcessThreadsTest()
		{
			try
			{
				_shield.Process(
					() => Task.Factory.StartNew(
						() =>
						{
							throw new OutOfMemoryException("test");
						})
						.Wait());
			}
			catch (AggregateException ex)
			{
				_log.Verify(x => x.Fatal(It.IsAny<string>()), Times.Never(), "Critical exceptions should not be logged.");

				throw ex.InnerException;
			}
		}

		[TestMethod]
		public void ProcessTest()
		{
			var exception = new Exception();
			try
			{
				_shield.Process(
					() =>
					{
						throw exception;
					});
			}
			catch (Exception ex)
			{
				Assert.AreSame(exception, ex, "Invalid exception rethrown");
				_log.Verify(x => x.Fatal(It.IsAny<string>()), Times.Once());
				return;
			}

			Assert.Fail("Should not be here.");
		}
	}
}
