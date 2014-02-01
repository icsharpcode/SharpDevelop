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
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RunNUnitTestsWithDebuggerTestFixture
	{
		MockTestResultsMonitor testResultsMonitor;
		SelectedTests selectedTests;
		NUnitTestDebugger testDebugger;
		MockDebuggerService debuggerService;
		MockDebugger debugger;
		MockMessageService messageService;
		UnitTestingOptions options;
		
		[SetUp]
		public void Init()
		{
			selectedTests = SelectedTestsHelper.CreateSelectedTestMember();
			FileUtility.ApplicationRootPath = @"C:\SharpDevelop";
			
			messageService = new MockMessageService();
			debuggerService = new MockDebuggerService();
			debugger = debuggerService.MockDebugger;
			testResultsMonitor = new MockTestResultsMonitor();
			testResultsMonitor.FileName = @"c:\temp\tmp66.tmp";
			options = new UnitTestingOptions(new Properties());
			options.NoShadow = true;
			testDebugger = new NUnitTestDebugger(debuggerService, messageService, testResultsMonitor, options);
		}
		
		[Test]
		public void DebuggerProcessStartInfoFileNameIsNUnitConsoleApp()
		{
			StartTestDebugger();
			string expectedCommandLine =
				@"C:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe";
			
			Assert.AreEqual(expectedCommandLine, debugger.ProcessStartInfo.FileName);
		}
		
		void StartTestDebugger()
		{
			testDebugger.Start(selectedTests);
		}
		
		[Test]
		public void DebuggerProcessStartInfoArgumentsIsNUnitConsoleCommandLineArguments()
		{
			StartTestDebugger();
			string expectedArguments = 
				"\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\" " +
				"/noshadow /noxml " +
				"/results=\"c:\\temp\\tmp66.tmp\" " +
				"/run=\"MyTests.MyTestClass.MyTestMethod\"";
			
			Assert.AreEqual(expectedArguments, debugger.ProcessStartInfo.Arguments);
		}
		
		[Test]
		public void DebuggerProcessStartInfoWorkingDirectoryIsNUnitConsoleAppDirectory()
		{
			StartTestDebugger();
			string expectedDirectory = @"C:\SharpDevelop\bin\Tools\NUnit";
			
			Assert.AreEqual(expectedDirectory, debugger.ProcessStartInfo.WorkingDirectory);
		}
		
		[Test]
		public void StopCallsDebuggerStopIfDebuggerIsDebugging()
		{
			StartTestDebugger();
			debugger.IsDebugging = true;
			testDebugger.Stop();
			
			Assert.IsTrue(debugger.IsStopCalled);
		}
		
		[Test]
		public void StopDoesNotCallDebuggerStopIfDebuggerIsNotDebugging()
		{
			StartTestDebugger();
			debugger.IsDebugging = false;
			testDebugger.Stop();
			
			Assert.IsFalse(debugger.IsStopCalled);
		}
		
		[Test]
		public void FiringDebuggerDebugStopEventFiresAllTestsFinishedEvent()
		{
			StartTestDebugger();
			
			bool fired = false;
			testDebugger.AllTestsFinished += delegate (object o, EventArgs e) {
				fired = true;
			};
			debugger.FireDebugStoppedEvent();
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void FiringDebuggerDebugStopEventTwiceFiresAllTestsFinishedEventOnlyOnce()
		{
			StartTestDebugger();
			
			int eventFiredCount = 0;
			testDebugger.AllTestsFinished += delegate (object o, EventArgs e) {
				eventFiredCount++;
			};
			debugger.FireDebugStoppedEvent();
			debugger.FireDebugStoppedEvent();
			
			Assert.AreEqual(1, eventFiredCount);
		}
		
		[Test]
		public void FiringDebugStopEventAfterDebuggerThrowsExceptionOnStartDoesNotFireAllTestsFinishedEvent()
		{
			debugger.ThrowExceptionOnStart = new ApplicationException();
			try {
				StartTestDebugger();
			} catch { 
				// Do nothing.
			}
			
			bool fired = false;
			testDebugger.AllTestsFinished += delegate (object o, EventArgs e) {
				fired = true;
			};
			debugger.FireDebugStoppedEvent();
			
			Assert.IsFalse(fired);
		}
		
		[Test]
		public void FiringTestResultsMonitorEventFiresTestFinishedEventWithNUnitTestResult()
		{
			TestResult actualTestResult = null;
			testDebugger.TestFinished += delegate (object o, TestFinishedEventArgs e) {
				actualTestResult = e.Result;
			};
			TestResult testResult = new TestResult("test");
			testResultsMonitor.FireTestFinishedEvent(testResult);
			
			TestResult nunitTestResult = actualTestResult as NUnitTestResult;
			
			Assert.AreEqual("test", nunitTestResult.Name);
		}
		
		[Test]
		public void MessageReceivedEventFiredShowingCommandLineWhenDebuggerStarted()
		{
			string message = null;
			testDebugger.MessageReceived += delegate(object sender, MessageReceivedEventArgs e) { 
				message = e.Message;
			};
			StartTestDebugger();
			
			string commandLine = "\"C:\\SharpDevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" " + 
				"\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\" " +
				"/noshadow /noxml " +
				"/results=\"c:\\temp\\tmp66.tmp\" " +
				"/run=\"MyTests.MyTestClass.MyTestMethod\"";
			Assert.AreEqual(commandLine, message);
		}
		
		[Test]
		public void IfDebuggerIsRunningWhenStartMethodCalledUserIsPromptedToStopDebugger()
		{
			SetDebuggerIsLoadedToTrueAndDebuggingToTrue();
			StartTestDebugger();
			string expectedQuestion = "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingQuestion}";
			
			Assert.AreEqual(expectedQuestion, messageService.QuestionPassedToAskQuestion);
		}
		
		void SetDebuggerIsLoadedToTrueAndDebuggingToTrue()
		{
			debuggerService.IsDebuggerLoaded = true;
			debuggerService.MockDebugger.IsDebugging = true;
		}
		
		[Test]
		public void UserNotPromptedToStopDebuggerWhenDebuggerIsLoadedAndDebuggerIsNotDebugging()
		{
			debuggerService.IsDebuggerLoaded = true;
			debuggerService.MockDebugger.IsDebugging = false;
			StartTestDebugger();
			
			Assert.IsNull(messageService.QuestionPassedToAskQuestion);
		}
		
		[Test]
		public void UserNotPromptedToStopDebuggerWhenDebuggerIsNotLoadedAndDebuggerIsDebugging()
		{
			debuggerService.IsDebuggerLoaded = false;
			debuggerService.MockDebugger.IsDebugging = true;
			StartTestDebugger();
			
			Assert.IsNull(messageService.QuestionPassedToAskQuestion);
		}
		
		[Test]
		public void IfDebuggerIsRunningWhenStartMethodCalledUserIsPromptedToStopDebuggerWithCaption()
		{
			SetDebuggerIsLoadedToTrueAndDebuggingToTrue();
			StartTestDebugger();
			string expectedCaption = "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingTitle}";
			
			Assert.AreEqual(expectedCaption, messageService.CaptionPassedToAskQuestion);
		}
		
		[Test]
		public void DebuggerStoppedIfUserClicksYesToStopDebuggerPrompt()
		{
			messageService.AskQuestionReturnValue = true;
			SetDebuggerIsLoadedToTrueAndDebuggingToTrue();
			StartTestDebugger();
			
			Assert.IsTrue(debugger.IsStopCalled);
		}
		
		[Test]
		public void DebuggerNotStoppedIfUserClicksNoToStopDebuggerPrompt()
		{
			UserClicksNoToStopDebuggerPrompt();
			
			Assert.IsFalse(debugger.IsStopCalled);
		}
		
		void UserClicksNoToStopDebuggerPrompt()
		{
			messageService.AskQuestionReturnValue = false;
			SetDebuggerIsLoadedToTrueAndDebuggingToTrue();
			StartTestDebugger();
		}
		
		[Test]
		public void DebuggerNotStartedIfUserClicksNoToStopDebuggerPrompt()
		{
			UserClicksNoToStopDebuggerPrompt();
			
			Assert.IsNull(debugger.ProcessStartInfo);
		}
		
		[Test]
		public void IfUserSaysYesToStopDebuggingPromptThenDebuggerIsStarted()
		{
			SetDebuggerIsLoadedToTrueAndDebuggingToTrue();
			messageService.AskQuestionReturnValue = true;
			StartTestDebugger();
			
			Assert.IsNotNull(debugger.ProcessStartInfo);
		}
		
		[Test]
		public void TestResultsMonitorIsStartedWhenDebuggerIsStarted()
		{
			StartTestDebugger();
			Assert.IsTrue(testResultsMonitor.IsStartMethodCalled);
		}
		
		[Test]
		public void StoppingDebuggerAlsoStopsTestResultsMonitor()
		{
			StartTestDebugger();
			testDebugger.Stop();
			
			Assert.IsTrue(testResultsMonitor.IsStopMethodCalled);
		}
		
		[Test]
		public void StoppingDebuggerCausesTestResultsMonitorReadMethodToBeCalled()
		{
			StartTestDebugger();
			testDebugger.Stop();
			
			Assert.IsTrue(testResultsMonitor.IsReadMethodCalled);
		}
		
		[Test]
		public void DisposingDebuggerDisposesTestResultsMonitor()
		{
			StartTestDebugger();
			testDebugger.Dispose();
			
			Assert.IsTrue(testResultsMonitor.IsDisposeMethodCalled);
		}
	}
}
