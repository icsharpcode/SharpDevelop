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
