using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using WindowsService.Core.Timers;

using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace WindowsService.Core.UnitTests.Timers
{
	[TestClass,ExcludeFromCodeCoverage]
	public class SingleThreadTimerTests
	{
		private SingleThreadTimer _timer;

		[TestInitialize]
		public void TestInit()
		{
			_timer = new SingleThreadTimer(TimeSpan.FromMilliseconds(100));
		}

		[TestMethod]
		public void SingleThreadTest()
		{
			var counter = 0;

			_timer.Start(
				x =>
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(200));
					counter++;
				});

			Thread.Sleep(TimeSpan.FromMilliseconds(300));
			Assert.AreEqual(1, counter, "Timer should be executed 1 time.");
		}
	}
}
