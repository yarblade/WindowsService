using System;
using System.Diagnostics.CodeAnalysis;

using WindowsService.Core.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace WindowsService.Core.UnitTests.Extensions
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ExceptionExtensionTests
	{
		[TestMethod]
		public void IsCriticalTest()
		{
			Assert.IsTrue(new StackOverflowException().IsCritical(), "StackOverflowException should be crititcal.");
			Assert.IsTrue(new OutOfMemoryException().IsCritical(), "StackOverflowException should be crititcal.");
			Assert.IsTrue(new AccessViolationException().IsCritical(), "StackOverflowException should be crititcal.");
			Assert.IsFalse(new ArgumentException().IsCritical(), "ArgumentException shouldn't be crititcal.");
			Assert.IsFalse(new Exception().IsCritical(), "Exception shouldn't be crititcal.");
		}
	}
}
