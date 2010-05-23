// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Util;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RunNUnitTestsForMethodTestFixture
	{
		MockNUnitTestRunnerContext context;
		SelectedTests selectedTests;
		NUnitTestRunner testRunner;
		
		[SetUp]
		public void Init()
		{
			selectedTests = SelectedTestsHelper.CreateSelectedTestMethod();
			context = new MockNUnitTestRunnerContext();
			FileUtility.ApplicationRootPath = @"C:\SharpDevelop";
			
			testRunner = context.CreateNUnitTestRunner();
			testRunner.Start(selectedTests);
		}
		
		[Test]
		public void StartMethodCallsTestResultsMonitorStartMethod()
		{
			Assert.IsTrue(context.MockTestResultsMonitor.IsStartMethodCalled);
		}
		
		[Test]
		public void StopMethodCallsTestResultsMonitorStopMethod()
		{
			testRunner.Stop();
			Assert.IsTrue(context.MockTestResultsMonitor.IsStopMethodCalled);
		}
		
		[Test]
		public void DisposeMethodCallsTestResultsMonitorDisposeMethod()
		{
			testRunner.Dispose();
			Assert.IsTrue(context.MockTestResultsMonitor.IsDisposeMethodCalled);
		}
		
		[Test]
		public void NUnitTestRunnerIsIDisposable()
		{
			Assert.IsNotNull(testRunner as IDisposable);
		}
		
		[Test]
		public void StopMethodCallsTestResultsReadMethodToEnsureAllTestsAreRead()
		{
			testRunner.Stop();
			Assert.IsTrue(context.MockTestResultsMonitor.IsReadMethodCalled);
		}
		
		[Test]
		public void StopMethodCallsProcessRunnerKillMethod()
		{
			testRunner.Stop();
			Assert.IsTrue(context.MockProcessRunner.IsKillMethodCalled);
		}
		
		[Test]
		public void FiringTestResultsMonitorTestFinishedEventFiresNUnitTestRunnerTestFinishedEvent()
		{
			TestResult testResultToFire = new TestResult("abc");
			TestResult resultFromEventHandler = FireTestResult(testResultToFire);
			Assert.IsNotNull(resultFromEventHandler);
		}
		
		TestResult FireTestResult(TestResult testResult)
		{
			TestResult resultFired = null;
			testRunner.TestFinished += delegate(object source, TestFinishedEventArgs e) {
				resultFired = e.Result;
			};
			
			context.MockTestResultsMonitor.FireTestFinishedEvent(testResult);
			return resultFired;
		}
		
		[Test]
		public void FiringTestResultsMonitorTestFinishedCreatesNUnitTestResultWithCorrectNameFromNUnitTestRunnerTestFinishedEvent()
		{
			TestResult testResultToFire = new TestResult("abc");
			NUnitTestResult resultFromEventHandler = FireTestResult(testResultToFire) as NUnitTestResult;
			Assert.AreEqual("abc", resultFromEventHandler.Name);
		}
		
		[Test]
		public void FiringTestResultsMonitorTestFinishedEventAfterDisposingTestRunnerDoesNotGenerateTestFinishedEvent()
		{
			bool fired = false;
			testRunner.TestFinished += delegate(object source, TestFinishedEventArgs e) {
				fired = true;
			};
			
			testRunner.Dispose();
			
			TestResult result = new TestResult("abc");
			context.MockTestResultsMonitor.FireTestFinishedEvent(result);
			Assert.IsFalse(fired);
		}
		
		[Test]
		public void NUnitTestRunnerImplementsITestRunner()
		{
			Assert.IsNotNull(testRunner as ITestRunner);
		}
		
		[Test]
		public void FiringProcessExitEventCausesTestRunnerAllTestsFinishedEventToFire()
		{
			bool fired = false;
			testRunner.AllTestsFinished += delegate (object source, EventArgs e) {
				fired = true;
			};
			context.MockProcessRunner.FireProcessExitedEvent();
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void NUnitConsoleExeProcessIsStarted()
		{
			string expectedCommand = @"C:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe";
			Assert.AreEqual(expectedCommand, context.MockProcessRunner.CommandPassedToStartMethod);
		}
		
		[Test]
		public void NUnitConsoleExeProcessIsStartedWithArgumentsToTestSingleMethod()
		{
			string expectedArgs = 
				"\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\" " +
				"/results=\"c:\\temp\\tmp66.tmp\" " +
				"/run=\"MyTests.MyTestClass.MyTestMethod\"";
			Assert.AreEqual(expectedArgs, context.MockProcessRunner.CommandArgumentsPassedToStartMethod);
		}
		
		[Test]
		public void NUnitConsoleWorkingDirectoryIsUsedByProcessRunner()
		{
			string expectedDirectory = @"C:\SharpDevelop\bin\Tools\NUnit";
			Assert.AreEqual(expectedDirectory, context.MockProcessRunner.WorkingDirectory);
		}
		
		[Test]
		public void ProcessRunnerLogStandardOutputAndErrorIsFalse()
		{
			Assert.IsFalse(context.MockProcessRunner.LogStandardOutputAndError);
		}
		
		[Test]
		public void FiringProcessRunnerOutputLineReceivedEventFiresTestRunnerMessageReceivedEvent()
		{
			string message = null;
			testRunner.MessageReceived += delegate (object o, MessageReceivedEventArgs e) {
				message = e.Message;
			};
			
			string expectedMessage = "test";
			context.MockProcessRunner.FireOutputLineReceivedEvent(new LineReceivedEventArgs(expectedMessage));
			
			Assert.AreEqual(expectedMessage, message);
		}
		
		[Test]
		public void FiringProcessRunnerErrorLineReceivedEventFiresTestRunnerMessageReceivedEvent()
		{
			string message = null;
			testRunner.MessageReceived += delegate (object o, MessageReceivedEventArgs e) {
				message = e.Message;
			};
			
			string expectedMessage = "test";
			context.MockProcessRunner.FireErrorLineReceivedEvent(new LineReceivedEventArgs(expectedMessage));
			
			Assert.AreEqual(expectedMessage, message);
		}
		
		[Test]
		public void FiringProcessRunnerOutputLineReceivedEventAfterDisposingTestRunnerDoesNotMessageReceivedEvent()
		{
			string message = null;
			testRunner.MessageReceived += delegate (object o, MessageReceivedEventArgs e) {
				message = e.Message;
			};
			
			testRunner.Dispose();
			context.MockProcessRunner.FireOutputLineReceivedEvent(new LineReceivedEventArgs("Test"));
			
			Assert.IsNull(message);
		}
		
		[Test]
		public void FiringProcessRunnerErrorLineReceivedEventAfterDisposingTestRunnerDoesNotMessageReceivedEvent()
		{
			string message = null;
			testRunner.MessageReceived += delegate (object o, MessageReceivedEventArgs e) {
				message = e.Message;
			};
			
			testRunner.Dispose();
			context.MockProcessRunner.FireErrorLineReceivedEvent(new LineReceivedEventArgs("Test"));
			
			Assert.IsNull(message);
		}
	}
}
