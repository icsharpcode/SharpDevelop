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
