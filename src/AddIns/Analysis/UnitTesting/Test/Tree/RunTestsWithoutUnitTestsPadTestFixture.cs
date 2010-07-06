// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RunTestsWithoutUnitTestsPadTestFixture : RunTestCommandTestFixtureBase
	{
		MockTestTreeView treeView;
		MockCSharpProject project;
		
		[SetUp]
		public void Init()
		{
			base.InitBase();
			
			project = new MockCSharpProject();
			MockBuildProjectBeforeTestRun buildProject = new MockBuildProjectBeforeTestRun();
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProject);
			
			treeView = new MockTestTreeView();
			treeView.SelectedProject = project;
			
			runTestCommand.Owner = treeView;
			
			context.MockUnitTestsPad = null;
			runTestCommand.Run();
			
			context.MockUnitTestWorkbench.MakeSafeThreadAsyncMethodCallsWithArguments = true;
			TestResult testResult = new TestResult("MyTests.MyTestClass.UnknownTestMethod");
			testResult.ResultType = TestResultType.Failure;
			context.MockTestResultsMonitor.FireTestFinishedEvent(testResult);
		}
		
		[Test]
		public void TestableConditionGetProjectReturnsSelectedProjectFromTreeView()
		{
			Assert.AreEqual(project, TestableCondition.GetProject(treeView));
		}
		
		[Test]
		public void OnBeforeTestsRunMethodIsCalled()
		{
			Assert.IsTrue(runTestCommand.IsOnBeforeBuildMethodCalled);
		}
	}
}
