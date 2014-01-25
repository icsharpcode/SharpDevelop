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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RunTestInPadCommandTestFixture
	{
		string oldRootPath;
		DerivedRunTestInPadCommand runCommand;
		MockProcessRunner processRunner;
		NUnitConsoleApplication helper;
		MockRunTestCommandContext context;
		MockNUnitTestFramework testFramework;
		MockBuildProjectBeforeTestRun buildProject;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			oldRootPath = FileUtility.ApplicationRootPath;
			FileUtility.ApplicationRootPath = @"D:\SharpDevelop";
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			FileUtility.ApplicationRootPath = oldRootPath;
		}
		
		[SetUp]
		public void Init()
		{
			MockCSharpProject project = new MockCSharpProject();
			SelectedTests selectedTests = new SelectedTests(project);
			helper = new NUnitConsoleApplication(selectedTests);
			
			context = new MockRunTestCommandContext();
			context.MockUnitTestsPad.AddProject(project);
			
			buildProject = new MockBuildProjectBeforeTestRun();
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProject);
			
			processRunner = new MockProcessRunner();
			
			testFramework = new MockNUnitTestFramework(processRunner, context.MockTestResultsMonitor, context.UnitTestingOptions);
			context.MockRegisteredTestFrameworks.AddTestFrameworkForProject(project, testFramework);
			
			runCommand = new DerivedRunTestInPadCommand(context);
		}
		
		[Test]
		public void LogStandardOutputAndErrorIsSetToFalse()
		{
			runCommand.Run();
			buildProject.FireBuildCompleteEvent();
			Assert.IsFalse(processRunner.LogStandardOutputAndError);
		}
		
		[Test]
		public void RunTestsStartsProcessWithCommandForUnitTestApplication()
		{
			runCommand.Run();
			buildProject.FireBuildCompleteEvent();
			string expectedCommand = @"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe";
			
			Assert.AreEqual(expectedCommand, processRunner.CommandPassedToStartMethod);
		}
		
		[Test]
		public void RunTestsStartsProcessWithCommandLineArgumentsForUnitTestApplication()
		{
			runCommand.Run();
			buildProject.FireBuildCompleteEvent();
			string expectedArgs = "\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\" /noxml";
			
			Assert.AreEqual(expectedArgs, processRunner.CommandArgumentsPassedToStartMethod);			
		}
		
		[Test]
		public void MessageCategoryDisplaysUnitTestApplicationCommandLine()
		{
			runCommand.Run();
			buildProject.FireBuildCompleteEvent();
			string fullCommandLine = helper.GetCommandLine();
			string expectedCategoryText = fullCommandLine + "\r\n";
			
			Assert.AreEqual(expectedCategoryText, context.UnitTestCategory.Text);
		}
		
		[Test]
		public void ProcessRunnerKillMethodCalledAfterStopMethodCalled()
		{
			runCommand.Run();
			buildProject.FireBuildCompleteEvent();
			runCommand.Stop();
			Assert.IsTrue(processRunner.IsKillMethodCalled);
		}
		
		[Test]
		public void ProcessRunnerKillMethodIsNotCalledAfterStopMethodCalledTheSecondTime()
		{
			runCommand.Run();
			buildProject.FireBuildCompleteEvent();
			runCommand.Stop();
			processRunner.IsKillMethodCalled = false;
			runCommand.Stop();
			Assert.IsFalse(processRunner.IsKillMethodCalled);
		}
		
		[Test]
		public void TestRunnerIsDisposedAfterRunCommandStopMethodIsCalled()
		{
			runCommand.Run();
			buildProject.FireBuildCompleteEvent();
			runCommand.Stop();
			Assert.IsTrue(context.MockTestResultsMonitor.IsDisposeMethodCalled);
		}
		
		[Test]
		public void OutputLineTextReceivedFromProcessRunnerIsAddedToMessageCategoryText()
		{
			runCommand.Run();
			buildProject.FireBuildCompleteEvent();
			context.UnitTestCategory.ClearText();
			
			LineReceivedEventArgs firstLineReceived = new LineReceivedEventArgs("first");
			processRunner.FireOutputLineReceivedEvent(firstLineReceived);
			LineReceivedEventArgs secondLineReceived = new LineReceivedEventArgs("second");
			processRunner.FireOutputLineReceivedEvent(secondLineReceived);
			
			string expectedCategoryText =
				"first\r\n" +
				"second\r\n";
			
			Assert.AreEqual(expectedCategoryText, context.UnitTestCategory.Text);
		}
		
		[Test]
		public void TestCompletedMethodIsCalledSafelyAndAsynchronouslyAfterProcessExitEventFires()
		{
			runCommand.Run();
			context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls.Clear();
			buildProject.FireBuildCompleteEvent();
			processRunner.FireProcessExitedEvent();
			
			Action callTestRunCompletedAction = runCommand.GetCallRunTestCompletedAction();
			Assert.AreEqual(callTestRunCompletedAction, context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls[0]);
		}
		
		[Test]
		public void TestRunnerIsDisposedAfterAllTestsFinished()
		{
			runCommand.Run();
			buildProject.FireBuildCompleteEvent();
			context.MockUnitTestWorkbench.MakeNonGenericSafeThreadAsyncMethodCalls = true;
			processRunner.FireProcessExitedEvent();
			
			Assert.IsTrue(context.MockTestResultsMonitor.IsDisposeMethodCalled);
		}
	}
}
