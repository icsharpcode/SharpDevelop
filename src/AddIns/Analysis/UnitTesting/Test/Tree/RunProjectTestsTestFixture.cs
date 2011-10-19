// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RunProjectTestsTestFixture : RunTestCommandTestFixtureBase
	{
		MockCSharpProject project;
		TestProject testProject;
		TestResult errorTestResult;
		TestMember firstTestMethod;
		TestResult warningTestResult;
		TestMember secondTestMethod;
		TestResult successTestResult;
		TestMember thirdTestMethod;
		MockTestFramework testFramework;
		bool runningTestsBeforeTestsFinishedCalled;
		
		[SetUp]
		public void Init()
		{
			base.InitBase();
			
			project = new MockCSharpProject();
			context.MockUnitTestsPad.AddProject(project);
			
			string[] methodNames = new string[] { "FirstTest", "SecondTest", "ThirdTest" };
			
			testProject = 
				TestProjectHelper.CreateTestProjectWithTestClassTestMethods(project,
					"MyTests.MyTestClass",
					methodNames);
			
			TestClass testClass = testProject.TestClasses[0];
			firstTestMethod = testClass.TestMembers[0];
			secondTestMethod = testClass.TestMembers[1];
			thirdTestMethod = testClass.TestMembers[2];
			
			context.MockUnitTestsPad.AddTestProject(testProject);
			
			MockBuildProjectBeforeTestRun buildProjectBeforeTestRun = new MockBuildProjectBeforeTestRun();
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProjectBeforeTestRun);
			
			context.UnitTestingOptions.NoThread = true;
			context.UnitTestingOptions.NoShadow = true;
			context.UnitTestingOptions.NoLogo = true;
			context.UnitTestingOptions.NoDots = true;
			context.UnitTestingOptions.Labels = true;
			context.UnitTestingOptions.CreateXmlOutputFile = true;
			
			testFramework = new MockTestFramework();
			context.MockRegisteredTestFrameworks.AddTestFrameworkForProject(project, testFramework);
			
			runTestCommand.Run();
			
			buildProjectBeforeTestRun.FireBuildCompleteEvent();
			
			errorTestResult = new TestResult("MyTests.MyTestClass.FirstTest");
			errorTestResult.ResultType = TestResultType.Failure;
			
			warningTestResult = new TestResult("MyTests.MyTestClass.SecondTest");
			warningTestResult.ResultType = TestResultType.Ignored;
			
			successTestResult = new TestResult("MyTests.MyTestClass.ThirdTest");
			successTestResult.ResultType = TestResultType.Success;
			
			context.MockUnitTestWorkbench.MakeSafeThreadAsyncMethodCallsWithArguments = true;
			MockTestRunner testRunner = runTestCommand.TestRunnersCreated[0];
			testRunner.FireTestFinishedEvent(errorTestResult);
			testRunner.FireTestFinishedEvent(warningTestResult);
			testRunner.FireTestFinishedEvent(successTestResult);
			
			context.MockUnitTestsPad.IsUpdateToolbarMethodCalled = false;
			runningTestsBeforeTestsFinishedCalled = AbstractRunTestCommand.IsRunningTest;
			runTestCommand.CallTestsCompleted();
		}
		
		[Test]
		public void RegisteredTestFrameworksReturnsTestFrameworkForProject()
		{
			Assert.AreEqual(testFramework, context.MockRegisteredTestFrameworks.GetTestFrameworkForProject(project));
		}
		
		[Test]
		public void FirstSafeAsyncMethodCallWithArgsIsMadeOnRunTestCommandShowResultsMethod()
		{
			ActionArguments<TestResult> actionArgs = new ActionArguments<TestResult>();
			actionArgs.Action = runTestCommand.ShowResultAction;
			actionArgs.Arg = errorTestResult;
			
			Assert.AreEqual(actionArgs, context.MockUnitTestWorkbench.SafeThreadAsyncMethodCallsWithArguments[0]);
		}
		
		[Test]
		public void FirstTaskAddedToTaskServiceIsErrorTask()
		{
			Task expectedTask = TestResultTask.Create(errorTestResult, testProject);
			TaskComparison taskComparison = new TaskComparison(expectedTask, context.MockTaskService.Tasks[0]);
			Assert.IsTrue(taskComparison.IsMatch, taskComparison.MismatchReason);
		}
		
		[Test]
		public void FirstTaskMethodRegionIsTakenFromTestProject()
		{
			DomRegion expectedRegion = new DomRegion(4, 19);
			Task task = context.MockTaskService.Tasks[0];
			DomRegion region = new DomRegion(task.Line, task.Column);
			Assert.AreEqual(expectedRegion, region);
		}
		
		[Test]
		public void UnitTestsPadGetProjectReturnsTestProject()
		{
			Assert.AreEqual(testProject, context.MockUnitTestsPad.GetTestProject(testProject.Project));
		}
		
		[Test]
		public void FirstTestMethodResultTypeIsFailure()
		{
			Assert.AreEqual(TestResultType.Failure, firstTestMethod.Result);
		}
		
		[Test]
		public void SecondTaskAddedToTaskServiceIsWarningTask()
		{
			Task expectedTask = TestResultTask.Create(warningTestResult, testProject);
			TaskComparison taskComparison = new TaskComparison(expectedTask, context.MockTaskService.Tasks[1]);
			Assert.IsTrue(taskComparison.IsMatch, taskComparison.MismatchReason);
		}
		
		[Test]
		public void SecondTestMethodResultTypeIsIgnored()
		{
			Assert.AreEqual(TestResultType.Ignored, secondTestMethod.Result);
		}
		
		[Test]
		public void TaskServiceOnlyHasTwoTasksSinceSuccessTestResultsDoNotCreateTasks()
		{
			Assert.AreEqual(2, context.MockTaskService.Tasks.Count);
		}
		
		[Test]
		public void ThirdTestMethodResultTypeIsSuccess()
		{
			Assert.AreEqual(TestResultType.Success, thirdTestMethod.Result);
		}
		
		[Test]
		public void IsRunningTestsReturnsTrueBeforeTestsFinishedMethodIsCalled()
		{
			Assert.IsTrue(runningTestsBeforeTestsFinishedCalled);
		}
		
		[Test]
		public void IsRunningTestsReturnsTrueAfterTestsFinishedMethodCalled()
		{
			Assert.IsFalse(AbstractRunTestCommand.IsRunningTest);
		}
		
		[Test]
		public void IsUnitTestsPadToolbarUpdatedAfterTestFinishedMethodCalled()
		{
			Assert.IsTrue(context.MockUnitTestsPad.IsUpdateToolbarMethodCalled);
		}
		
		[Test]
		public void OnAfterRunTestsCalledAfterTestFinishedMethodCalled()
		{
			Assert.IsTrue(runTestCommand.IsOnAfterRunTestsMethodCalled);
		}
		
		[Test]
		public void ErrorListPadDescriptorExistsInWorkbench()
		{
			Assert.IsNotNull(context.MockUnitTestWorkbench.GetPad(typeof(ErrorListPad)));
		}
		
		[Test]
		public void ErrorListPadBroughtToFrontAfterTestsFinishedCalled()
		{
			Action expectedAction = context.MockUnitTestWorkbench.ErrorListPadDescriptor.BringPadToFront;
			Assert.AreEqual(expectedAction, context.MockUnitTestWorkbench.SafeThreadAsyncMethodCalls[1]);
		}
	}
}
