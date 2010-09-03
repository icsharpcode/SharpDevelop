// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockNUnitTestFrameworkTestFixture
	{
		MockRunTestCommandContext context;
		MockProcessRunner processRunner;
		MockNUnitTestFramework testFramework;
		NUnitTestRunner testRunner;
		NUnitTestDebugger testDebugger;
		MockDebuggerService debuggerService;
		
		[SetUp]
		public void Init()
		{
			context = new MockRunTestCommandContext();
			processRunner = new MockProcessRunner();
			debuggerService = new MockDebuggerService();
			
			testFramework = new MockNUnitTestFramework(debuggerService,
				processRunner,
				context.MockTestResultsMonitor,
				context.UnitTestingOptions,
				context.MessageService);
			
			testRunner = testFramework.CreateTestRunner() as NUnitTestRunner;
			testDebugger = testFramework.CreateTestDebugger() as NUnitTestDebugger;
		}
		
		[Test]
		public void CreateTestRunnerCreatesNUnitTestRunner()
		{
			Assert.IsNotNull(testRunner);
		}
		
		[Test]
		public void NUnitTestRunnerAddedToTestRunnersCreatedList()
		{
			List<NUnitTestRunner> expectedRunners = new List<NUnitTestRunner>();
			expectedRunners.Add(testRunner);
			
			Assert.AreEqual(expectedRunners.ToArray(), testFramework.NUnitTestRunnersCreated.ToArray());
		}
		
		[Test]
		public void NUnitTestRunnerCreatedWithMockProcessRunnerAndUnitTestingOptions()
		{
			MockCSharpProject project = new MockCSharpProject();
			SelectedTests tests = new SelectedTests(project);
			testRunner.Start(tests);
			
			NUnitConsoleApplication app = new NUnitConsoleApplication(tests, context.UnitTestingOptions);
			string expectedArguments = app.GetArguments();
			Assert.AreEqual(expectedArguments, processRunner.CommandArgumentsPassedToStartMethod);
		}
		
		[Test]
		public void NUnitTestRunnerCreatedWithMockTestResultsMonitor()
		{
			MockCSharpProject project = new MockCSharpProject();
			SelectedTests tests = new SelectedTests(project);
			testRunner.Start(tests);
			
			Assert.IsTrue(context.MockTestResultsMonitor.IsStartMethodCalled);
		}
		
		[Test]
		public void NUnitTestDebuggerCreated()
		{
			Assert.IsNotNull(testDebugger);
		}
		
		[Test]
		public void NUnitTestDebuggerAddedToTestDebuggersCreatedList()
		{
			List<NUnitTestDebugger> expectedDebuggers = new List<NUnitTestDebugger>();
			expectedDebuggers.Add(testDebugger);
			
			Assert.AreEqual(expectedDebuggers.ToArray(), testFramework.NUnitTestDebuggersCreated.ToArray());
		}
		
		[Test]
		public void NUnitTestDebuggerCreatedWithMockTestResultsMonitor()
		{
			MockCSharpProject project = new MockCSharpProject();
			SelectedTests tests = new SelectedTests(project);
			testDebugger.Start(tests);
			
			Assert.IsTrue(context.MockTestResultsMonitor.IsStartMethodCalled);
		}
		
		[Test]
		public void NUnitTestDebuggerCreatedWithDebuggerService()
		{
			context.UnitTestingOptions.NoShadow = true;
			
			MockCSharpProject project = new MockCSharpProject();
			SelectedTests tests = new SelectedTests(project);
			testDebugger.Start(tests);
			
			NUnitConsoleApplication app = new NUnitConsoleApplication(tests, context.UnitTestingOptions);
			string expectedArguments = app.GetArguments();
			Assert.AreEqual(expectedArguments, debuggerService.MockDebugger.ProcessStartInfo.Arguments);
		}
		
		[Test]
		public void NUnitTestDebuggerCreatedWithMessageService()
		{
			context.MockMessageService.AskQuestionReturnValue = true;
			debuggerService.IsDebuggerLoaded = true;
			debuggerService.MockDebugger.IsDebugging = true;
			
			MockCSharpProject project = new MockCSharpProject();
			SelectedTests tests = new SelectedTests(project);
			testDebugger.Start(tests);
			
			Assert.IsNotNull(context.MockMessageService.QuestionPassedToAskQuestion);
		}
		
		[Test]
		public void IsBuildNeededBeforeTestRunReturnsTrue()
		{
			Assert.IsTrue(testFramework.IsBuildNeededBeforeTestRun);
		}
	}
}
