// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		void CreateNUnitTestRunner()
		{
			selectedTests = SelectedTestsHelper.CreateSelectedTestMember();
			context = new MockNUnitTestRunnerContext();
			FileUtility.ApplicationRootPath = @"C:\SharpDevelop";
			
			testRunner = context.CreateNUnitTestRunner();
		}
		
		void StartNUnitTestRunner()
		{
			testRunner.Start(selectedTests);
		}
		
		[Test]
		public void StartMethodCallsTestResultsMonitorStartMethod()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			Assert.IsTrue(context.MockTestResultsMonitor.IsStartMethodCalled);
		}
		
		[Test]
		public void StopMethodCallsTestResultsMonitorStopMethod()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			testRunner.Stop();
			Assert.IsTrue(context.MockTestResultsMonitor.IsStopMethodCalled);
		}
		
		[Test]
		public void DisposeMethodCallsTestResultsMonitorDisposeMethod()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			testRunner.Dispose();
			Assert.IsTrue(context.MockTestResultsMonitor.IsDisposeMethodCalled);
		}
		
		[Test]
		public void NUnitTestRunnerIsIDisposable()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			Assert.IsNotNull(testRunner as IDisposable);
		}
		
		[Test]
		public void StopMethodCallsTestResultsReadMethodToEnsureAllTestsAreRead()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			testRunner.Stop();
			Assert.IsTrue(context.MockTestResultsMonitor.IsReadMethodCalled);
		}
		
		[Test]
		public void StopMethodCallsProcessRunnerKillMethod()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			testRunner.Stop();
			Assert.IsTrue(context.MockProcessRunner.IsKillMethodCalled);
		}
		
		[Test]
		public void FiringTestResultsMonitorTestFinishedEventFiresNUnitTestRunnerTestFinishedEvent()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			
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
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
					
			TestResult testResultToFire = new TestResult("abc");
			NUnitTestResult resultFromEventHandler = FireTestResult(testResultToFire) as NUnitTestResult;
			Assert.AreEqual("abc", resultFromEventHandler.Name);
		}
		
		[Test]
		public void FiringTestResultsMonitorTestFinishedEventAfterDisposingTestRunnerDoesNotGenerateTestFinishedEvent()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();

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
			CreateNUnitTestRunner();
			Assert.IsNotNull(testRunner as ITestRunner);
		}
		
		[Test]
		public void FiringProcessExitEventCausesTestRunnerAllTestsFinishedEventToFire()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			
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
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			
			string expectedCommand = @"C:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe";
			Assert.AreEqual(expectedCommand, context.MockProcessRunner.CommandPassedToStartMethod);
		}
		
		[Test]
		public void NUnitConsoleExeProcessIsStartedWithArgumentsToTestSingleMethod()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			
			string expectedArgs = 
				"\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\" /noxml " +
				"/results=\"c:\\temp\\tmp66.tmp\" " +
				"/run=\"MyTests.MyTestClass.MyTestMethod\"";
			Assert.AreEqual(expectedArgs, context.MockProcessRunner.CommandArgumentsPassedToStartMethod);
		}
		
		[Test]
		public void NUnitConsoleWorkingDirectoryIsUsedByProcessRunner()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			
			string expectedDirectory = @"C:\SharpDevelop\bin\Tools\NUnit";
			Assert.AreEqual(expectedDirectory, context.MockProcessRunner.WorkingDirectory);
		}
		
		[Test]
		public void ProcessRunnerLogStandardOutputAndErrorIsFalse()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			
			Assert.IsFalse(context.MockProcessRunner.LogStandardOutputAndError);
		}
		
		[Test]
		public void FiringProcessRunnerOutputLineReceivedEventFiresTestRunnerMessageReceivedEvent()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			
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
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			
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
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			
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
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			
			string message = null;
			testRunner.MessageReceived += delegate (object o, MessageReceivedEventArgs e) {
				message = e.Message;
			};
			
			testRunner.Dispose();
			context.MockProcessRunner.FireErrorLineReceivedEvent(new LineReceivedEventArgs("Test"));
			
			Assert.IsNull(message);
		}
		
		[Test]
		public void NUnitConsoleExeFileCheckedThatItExists()
		{
			CreateNUnitTestRunner();
			StartNUnitTestRunner();
			
			string fileName = context.MockFileService.FileNamePassedToFileExists;
			string expectedFileName = @"C:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void NUnitConsoleExeNotStartedIfFileDoesNotExist()
		{
			CreateNUnitTestRunner();
			context.MockFileService.FileExistsReturnValue = false;
			StartNUnitTestRunner();
			
			Assert.IsNull(context.MockProcessRunner.CommandPassedToStartMethod);
		}
		
		[Test]
		public void MessageShowedToUserIfNUnitConsoleExeDoesNotExist()
		{
			CreateNUnitTestRunner();
			context.MockFileService.FileExistsReturnValue = false;
			StartNUnitTestRunner();
			
			string expectedFormat = "${res:ICSharpCode.UnitTesting.TestRunnerNotFoundMessageFormat}";
			string expectedItem = @"C:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe";
			
			Assert.AreEqual(expectedFormat, context.MockMessageService.FormatPassedToShowFormattedErrorMessage);
			Assert.AreEqual(expectedItem, context.MockMessageService.ItemPassedToShowFormattedErrorMessage);
		}
	}
}
