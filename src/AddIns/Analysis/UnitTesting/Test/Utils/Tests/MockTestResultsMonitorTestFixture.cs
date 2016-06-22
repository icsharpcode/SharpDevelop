// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
