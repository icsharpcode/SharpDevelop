// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTestResultsMonitorTestFixture
	{
		MockTestResultsMonitor testResultsMonitor;
		
		[SetUp]
		public void Init()
		{
			testResultsMonitor = new MockTestResultsMonitor();
		}
		
		[Test]
		public void IsStopMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(testResultsMonitor.IsStopMethodCalled);
		}
		
		[Test]
		public void IsStopMethodCalledReturnsTrueAfterStopMethodCalled()
		{
			testResultsMonitor.Stop();
			Assert.IsTrue(testResultsMonitor.IsStopMethodCalled);
		}
		
		[Test]
		public void IsStopMethodCalledCanBeResetAfterStopMethodIsCalled()
		{
			testResultsMonitor.Stop();
			testResultsMonitor.IsStopMethodCalled = false;
			Assert.IsFalse(testResultsMonitor.IsStopMethodCalled);
		}
		
		[Test]
		public void IsDisposeMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(testResultsMonitor.IsDisposeMethodCalled);
		}
		
		[Test]
		public void IsDisposeMethodCalledReturnsTrueAfterStopMethodCalled()
		{
			testResultsMonitor.Dispose();
			Assert.IsTrue(testResultsMonitor.IsDisposeMethodCalled);
		}
		
		[Test]
		public void IsStartMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(testResultsMonitor.IsStartMethodCalled);
		}
		
		[Test]
		public void IsStopMethodCalledReturnsTrueAfterStartMethodCalled()
		{
			testResultsMonitor.Start();
			Assert.IsTrue(testResultsMonitor.IsStartMethodCalled);
		}
		
		[Test]
		public void FileNamePropertyReturnsNullByDefault()
		{
			Assert.IsNull(testResultsMonitor.FileName);
		}
		
		[Test]
		public void FileNamePropertyReturnsFileNameSet()
		{
			testResultsMonitor.FileName = "test.xml";
			Assert.AreEqual("test.xml", testResultsMonitor.FileName);
		}
		
		[Test]
		public void FireTestFinishedEventTriggersTestFinishedEvent()
		{
			TestResult expectedResult = new TestResult("abc");
			TestResult result = null;
			testResultsMonitor.TestFinished += 
				delegate(object source, TestFinishedEventArgs e) { 
					result = e.Result; 
				};
			
			testResultsMonitor.FireTestFinishedEvent(expectedResult);
			
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void IsReadMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(testResultsMonitor.IsReadMethodCalled);
		}
		
		[Test]
		public void IsReadMethodCalledReturnsTrueAfterReadMethodCalled()
		{
			testResultsMonitor.Read();
			Assert.IsTrue(testResultsMonitor.IsReadMethodCalled);
		}
	}
}
