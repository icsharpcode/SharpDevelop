// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.UnitTesting;
using Gallio.Runner.Events;
using Gallio.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace Gallio.SharpDevelop.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTestRunnerEventsTestFixture
	{
		MockTestRunnerEvents testRunnerEvents;
		TestStepFinishedEventArgs testStepFinishedEventArgs;
		
		[SetUp]
		public void Init()
		{
			testRunnerEvents = new MockTestRunnerEvents();
			testRunnerEvents.TestStepFinished += delegate (object source, TestStepFinishedEventArgs e) {
				testStepFinishedEventArgs = e;
			};
		}
		
		[Test]
		public void FireTestStepFinishedEventWithTestResultCreatesGallioTestDataWithExpectedFullName()
		{
			testRunnerEvents.FireTestStepFinishedEvent("MyNamespace/MyTests/MyTestMethod");
			
			string expectedFullName = "MyNamespace/MyTests/MyTestMethod";
			string actualFullName = testStepFinishedEventArgs.Test.FullName;
			Assert.AreEqual(expectedFullName, actualFullName);
		}
		
		[Test]
		public void FireTestStepFinishedEventCreatesGallioTestDataWithIsTestCaseSetToTrue()
		{
			testRunnerEvents.FireTestStepFinishedEvent("testName");
			
			Assert.IsTrue(testStepFinishedEventArgs.Test.IsTestCase);
		}
		
		[Test]
		public void FireTestStepFinishedEventForNonTestCaseCreatesGallioTestDataIsTestCaseSetToFalse()
		{
			testRunnerEvents.FireTestStepFinishedEventForNonTestCase("testName");
			
			Assert.IsFalse(testStepFinishedEventArgs.Test.IsTestCase);
		}
		
		[Test]
		public void FireDisposeStartedEventTriggersEvent()
		{
			DisposeStartedEventArgs eventArgs = null;
			testRunnerEvents.DisposeStarted += delegate(object sender, DisposeStartedEventArgs e) { 
				eventArgs = e;
			};
			testRunnerEvents.FireDisposeStartedEvent();
			
			Assert.IsNotNull(eventArgs);
		}
	}
}
