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
	public class RunTestsForClassTestFixture : RunTestCommandTestFixtureBase
	{
		MockTestTreeView treeView;
		MockCSharpProject project;
		MockClass classToTest;
		
		[SetUp]
		public void Init()
		{
			base.InitBase();
			
			project = new MockCSharpProject();
			MockBuildProjectBeforeTestRun buildProject = new MockBuildProjectBeforeTestRun();
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProject);
			
			classToTest = MockClass.CreateMockClassWithoutAnyAttributes();
			classToTest.SetDotNetName("MyTestClass");
			
			treeView = new MockTestTreeView();
			treeView.SelectedProject = project;
			treeView.SelectedClass = classToTest;
			
			runTestCommand.Owner = treeView;
			
			runTestCommand.Run();
			
			buildProject.FireBuildCompleteEvent();
			
			TestResult result = new TestResult("MyTestClass");
			result.ResultType = TestResultType.Success;
			context.MockTestResultsMonitor.FireTestFinishedEvent(result);
			
			runTestCommand.CallTestsCompleted();
		}
		
		[Test]
		public void TestableConditionGetClassReturnsClassToTest()
		{
			Assert.AreEqual(classToTest, TestableCondition.GetClass(treeView));
		}
		
		[Test]
		public void SelectedTestsHasClassConfigured()
		{
			MockTestRunner testRunner = runTestCommand.TestRunnersCreated[0];
			SelectedTests tests = testRunner.SelectedTestsPassedToStartMethod;
			Assert.AreEqual(classToTest, tests.Class);
		}
		
		[Test]
		public void ErrorListPadIsNotShownWhenAllTestsPassed()
		{
			Assert.IsFalse(context.MockUnitTestWorkbench.TypesPassedToGetPadMethod.Contains(typeof(ErrorListPad)));
		}
	}
}
