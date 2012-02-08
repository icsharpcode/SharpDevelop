// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RunTestsForMethodTestFixture : RunTestCommandTestFixtureBase
	{
		MockTestTreeView treeView;
		MockCSharpProject project;
		MockClass classToTest;
		MockMethod methodToTest;
		
		[SetUp]
		public void Init()
		{
			base.InitBase();
			
			project = new MockCSharpProject();
			MockBuildProjectBeforeTestRun buildProject = new MockBuildProjectBeforeTestRun();
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProject);
			
			methodToTest = MockMethod.CreateMockMethodWithoutAnyAttributes();
			methodToTest.FullyQualifiedName = "MyTests.MyTestClass.MyTestMethod";
			
			classToTest = methodToTest.DeclaringType as MockClass;
			classToTest.SetDotNetName("MyTests.MyTestClass");
			
			treeView = new MockTestTreeView();
			treeView.SelectedProject = project;
			treeView.SelectedMember = methodToTest;
			
			runTestCommand.Owner = treeView;
			
			runTestCommand.Run();
			
			buildProject.FireBuildCompleteEvent();
			
			context.MockUnitTestWorkbench.MakeSafeThreadAsyncMethodCallsWithArguments = true;
			context.MockBuildOptions.ShowErrorListAfterBuild = false;
			
			TestResult result = new TestResult("MyTests.MyTestClass.MyTestMethod");
			result.ResultType = TestResultType.Failure;
			context.MockTestResultsMonitor.FireTestFinishedEvent(result);
			
			runTestCommand.CallTestsCompleted();
		}
		
		[Test]
		public void SelectedTestsHasTestMethodConfigured()
		{
			SelectedTests tests = runTestCommand.TestRunnersCreated[0].SelectedTestsPassedToStartMethod;
			Assert.AreEqual(methodToTest, tests.Member);
		}
		
		[Test]
		public void TestableConditionGetMemberReturnsTestMethod()
		{
			Assert.AreEqual(methodToTest, TestableCondition.GetMember(treeView));
		}
		
		[Test]
		public void SelectedTestsHasClassConfigured()
		{
			SelectedTests tests = runTestCommand.TestRunnersCreated[0].SelectedTestsPassedToStartMethod;
			Assert.AreEqual(classToTest, tests.Class);
		}
		
		[Test]
		public void ErrorListPadNotDisplayedWhenShowErrorListPadAfterBuildIsFalse()
		{
			Assert.IsFalse(context.MockUnitTestWorkbench.TypesPassedToGetPadMethod.Contains(typeof(ErrorListPad)));
		}
	}
}
