using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using WindowsService.Core.Sandboxes;
using WindowsService.Core.UnitTests.Entities;
using WindowsService.Core.Workers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Ploeh.AutoFixture;



namespace WindowsService.Core.UnitTests.Sandboxes
{
	[TestClass, ExcludeFromCodeCoverage]
	public class DefaultWorkerSandboxTests
	{
		private IFixture _fixture;
		private DefaultWorkerSandbox<TestEntity> _sandbox;
		private CancellationTokenSource _tokenSource;
		private string _workerName;
		private Mock<IWorkerRunner<TestEntity>> _workerRunner;

		[TestInitialize]
		public void TestInit()
		{
			_fixture = new Fixture();
			_workerRunner = new Mock<IWorkerRunner<TestEntity>>(MockBehavior.Strict);
			_workerName = _fixture.Create<string>();
			_tokenSource = new CancellationTokenSource();

			_sandbox = new DefaultWorkerSandbox<TestEntity>(_workerRunner.Object, _workerName, TimeSpan.FromSeconds(1));
		}

		[TestMethod]
		public void StartWorkerExecutionTest()
		{
			_workerRunner.Setup(x => x.RunAsync(_workerName, _tokenSource.Token)).Returns(Task.FromResult(new TestEntity()));
			
			_sandbox.StartWorkerExecution(_tokenSource.Token);
			Thread.Sleep(TimeSpan.FromSeconds(3));
			_tokenSource.Cancel();

			_workerRunner.Verify(x => x.RunAsync(_workerName, _tokenSource.Token), Times.Exactly(3));
		}

		[TestMethod]
		public void CompletionTest()
		{
			var completed = false;

			_workerRunner.Setup(x => x.RunAsync(_workerName, _tokenSource.Token))
				.Returns(Task.FromResult(new TestEntity()))
				.Callback(() =>
				{
					Thread.Sleep(TimeSpan.FromSeconds(2));
					completed = true;
				});

			_sandbox.StartWorkerExecution(_tokenSource.Token);
			Thread.Sleep(TimeSpan.FromSeconds(0.5));
			_tokenSource.Cancel();
			
			Assert.IsFalse(completed, "Should be uncompleted yet.");
			
			_sandbox.Completion.Wait();
			Assert.IsTrue(completed, "Should be completed already.");
		}
	}
}
