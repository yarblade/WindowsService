using System.Diagnostics.CodeAnalysis;
using System.Threading;

using WindowsService.Core.UnitTests.Entities;
using WindowsService.Core.Workers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;



namespace WindowsService.Core.UnitTests.Workers
{
	[TestClass, ExcludeFromCodeCoverage]
	public class AsyncWorkerAdapterTests
	{
		private AsyncWorkerAdapter<TestEntity> _adapter;
		private Mock<IWorker<TestEntity>> _worker;

		[TestInitialize]
		public void TestInit()
		{
			_worker = new Mock<IWorker<TestEntity>>(MockBehavior.Strict);

			_adapter = new AsyncWorkerAdapter<TestEntity>(_worker.Object);
		}

		[TestMethod]
		public void DoWorkTest()
		{
			var token = CancellationToken.None;
			var entity = new TestEntity();

			_worker.Setup(x => x.DoWork(token)).Returns(() => entity);

			var actual = _adapter.DoWork(token);
			Assert.AreEqual(entity, actual.Result, "Wrong worker result.");

			_worker.Verify(x => x.DoWork(token), Times.Once);
		}
	}
}
