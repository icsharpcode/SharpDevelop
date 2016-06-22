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
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTestRunnerTestFixture
	{
		MockTestRunner testRunner;
		
		[SetUp]
		public void Init()
		{
			testRunner = new MockTestRunner();
		}
		
		[Test]
		public void IsDisposeCalledReturnsTrueAfterDisposeMethodCalled()
		{
			testRunner.Dispose();
			Assert.IsTrue(testRunner.IsDisposeCalled);
		}
		
		[Test]
		public void IsDisposeCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(testRunner.IsDisposeCalled);
		}
		
		[Test]
		public void IsStartCalledReturnsTrueAfterStartMethodCalled()
		{
			SelectedTests selectedTests = new SelectedTests(null);
			testRunner.Start(selectedTests);
			Assert.IsTrue(testRunner.IsStartCalled);
		}
		
		[Test]
		public void IsStartCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(testRunner.IsStartCalled);
		}
		
		[Test]
		public void IsStopCalledReturnsTrueAfterStartMethodCalled()
		{
			testRunner.Stop();
			Assert.IsTrue(testRunner.IsStopCalled);
		}
		
		[Test]
		public void IsStopCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(testRunner.IsStopCalled);
		}
		
		[Test]
		public void FireTestFinishedEventTriggersTestFinishedEvent()
		{
			TestResult expectedResult = new TestResult("abc");
			TestResult result = null;
			testRunner.TestFinished += 
				delegate(object source, TestFinishedEventArgs e) { 
					result = e.Result; 
				};
			
			testRunner.FireTestFinishedEvent(expectedResult);
			
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void SelectedTestsPassedToStartMethodIsNullByDefault()
		{
			Assert.IsNull(testRunner.SelectedTestsPassedToStartMethod);
		}
		
		[Test]
		public void SelectedTestsPassedToStartMethodIsSaved()
		{
			SelectedTests expectedSelectedTests = new SelectedTests(null);
			testRunner.Start(expectedSelectedTests);
			
			Assert.AreEqual(expectedSelectedTests, testRunner.SelectedTestsPassedToStartMethod);
		}
		
		[Test]
		public void FireAllTestFinishedsEventTriggersAllTestsFinishedEvent()
		{
			bool fired = false;
			testRunner.AllTestsFinished += 
				delegate(object source, EventArgs e) { 
					fired = true;
				};
			
			testRunner.FireAllTestsFinishedEvent();
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void FireMessageReceivedEventTriggersMessageReceivedEvent()
		{
			string message = null;
			testRunner.MessageReceived += 
				delegate(object source, MessageReceivedEventArgs e) { 
					message = e.Message; 
				};
			
			string expectedMessage = "test";
			testRunner.FireMessageReceivedEvent(expectedMessage);
			
			Assert.AreEqual(expectedMessage, message);
		}
	}
}
