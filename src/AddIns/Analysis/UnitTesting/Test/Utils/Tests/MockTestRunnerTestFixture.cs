// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
