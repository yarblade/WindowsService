using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using WindowsService.Core.Timers;

using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace WindowsService.Core.UnitTests.Timers
{
	[TestClass, ExcludeFromCodeCoverage]
	public class DefaultTimerTests
	{
		private DefaultTimer _timer;

		[TestInitialize]
		public void TestInit()
		{
			_timer = new DefaultTimer(TimeSpan.FromMilliseconds(100));
		}

		[TestMethod]
		public void StartTest()
		{
			var counter = 0;

			_timer.Start(x => counter++);
			Thread.Sleep(TimeSpan.FromMilliseconds(150));

			Assert.AreEqual(2, counter, "Timer should be executed 2 times.");
			_timer.Stop();
			Thread.Sleep(TimeSpan.FromMilliseconds(100));
			Assert.AreEqual(2, counter, "Timer should be stopped.");
		}

		[TestMethod]
		public void StopTest()
		{
			var counter = 0;

			_timer.Start(
				x =>
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(200));
					counter++;
				});
			Thread.Sleep(TimeSpan.FromMilliseconds(50));
			_timer.Stop();
			
			Assert.AreEqual(1, counter, "Timer should be executed.");
			Thread.Sleep(TimeSpan.FromMilliseconds(100));
			Assert.AreEqual(1, counter, "Timer should be stopped.");
		}

		[TestMethod]
		public void ChangeTest()
		{
			var counter = 0;

			_timer.Start(x => counter++);
			Thread.Sleep(TimeSpan.FromMilliseconds(150));

			Assert.AreEqual(2, counter, "Timer should be executed 2 times.");
			_timer.Change(TimeSpan.FromSeconds(200));
			Thread.Sleep(TimeSpan.FromMilliseconds(200));
			Assert.AreEqual(3, counter, "Timer should be executed 3 times.");
		}
	}
}
