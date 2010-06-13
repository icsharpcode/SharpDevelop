// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			treeView.SelectedMethod = methodToTest;
			
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
			Assert.AreEqual(methodToTest, tests.Method);
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
