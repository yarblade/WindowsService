using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.UnitTests.Entities;
using WindowsService.Core.Workers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Ploeh.AutoFixture;



namespace WindowsService.Core.UnitTests.Workers
{
	[TestClass, ExcludeFromCodeCoverage]
	public class DefaultWorkerRunnerTests
	{
		private Mock<IAsyncWorker<TestEntity>> _asyncWorker;
		private IFixture _fixture;
		private DefaultWorkerRunner<TestEntity> _workerRunner;
		private Mock<IDictionary<string, Func<IAsyncWorker<TestEntity>>>> _workers;

		[TestInitialize]
		public void TestInit()
		{
			_fixture = new Fixture();
			_asyncWorker = new Mock<IAsyncWorker<TestEntity>>(MockBehavior.Strict);
			_workers = new Mock<IDictionary<string, Func<IAsyncWorker<TestEntity>>>>(MockBehavior.Strict);

			_workerRunner = new DefaultWorkerRunner<TestEntity>(_workers.Object);
		}

		[TestMethod]
		public void RunAsyncTest()
		{
			var workerName = _fixture.Create<string>();
			var token = CancellationToken.None;
			var entity = new TestEntity();

			_workers.Setup(x => x.ContainsKey(workerName)).Returns(true);
			_workers.Setup(x => x[workerName]).Returns(() => _asyncWorker.Object);
			_asyncWorker.Setup(x => x.DoWork(token)).Returns(Task.FromResult(entity));

			var actual = _workerRunner.RunAsync(workerName, token);
			Assert.AreEqual(entity, actual.Result, "Wrong worker runner result.");

			_workers.Verify(x => x.ContainsKey(workerName), Times.Once);
			_workers.Verify(x => x[workerName], Times.Once);
			_asyncWorker.Verify(x => x.DoWork(token), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void RunAsync_InvalidOperationExceptionTest()
		{
			var workerName = _fixture.Create<string>();
			var token = CancellationToken.None;

			_workers.Setup(x => x.ContainsKey(workerName)).Returns(false);

			try
			{
				_workerRunner.RunAsync(workerName, token).Wait(token);
			}
			catch (AggregateException ex)
			{
				throw ex.InnerException;
			}
		}
	}
}
