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
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RunTestsWithoutBuildingProjectTestFixture : RunTestCommandTestFixtureBase
	{
		MockTestFramework testFramework;
		MockBuildProjectBeforeTestRun buildProjectBeforeTestRun;
		
		[SetUp]
		public void Init()
		{
			InitBase();
			
			MockCSharpProject project1 = new MockCSharpProject();
			MockCSharpProject project2 = new MockCSharpProject();
			testFramework = new MockTestFramework();
			testFramework.IsBuildNeededBeforeTestRun = false;
			context.MockRegisteredTestFrameworks.AddTestFrameworkForProject(project1, testFramework);
			context.MockRegisteredTestFrameworks.AddTestFrameworkForProject(project2, testFramework);
			
			buildProjectBeforeTestRun = new MockBuildProjectBeforeTestRun();
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProjectBeforeTestRun);
			
			context.MockUnitTestsPad.AddProject(project1);
			context.MockUnitTestsPad.AddProject(project2);
			
			runTestCommand.Run();
		}
		
		[Test]
		public void TestRunnerIsStarted()
		{
			Assert.IsTrue(runTestCommand.TestRunnersCreated[0].IsStartCalled);
		}
		
		[Test]
		public void ProjectIsNotBuiltBeforeTestRun()
		{
			Assert.IsFalse(buildProjectBeforeTestRun.IsRunMethodCalled);
		}
		
		[Test]
		public void SaveAllFilesCommandIsRun()
		{
			Assert.IsTrue(context.MockSaveAllFilesCommand.IsSaveAllFilesMethodCalled);
		}
		
		[Test]
		public void WhenTestRunCompletedTheSecondProjectIsNotBuilt()
		{
			runTestCommand.CallTestsCompleted();
			Assert.IsFalse(buildProjectBeforeTestRun.IsRunMethodCalled);
		}
		
		[Test]
		public void WhenTestRunCompletedTheSecondTestRunnerIsStarted()
		{
			runTestCommand.CallTestsCompleted();
			Assert.IsTrue(runTestCommand.TestRunnersCreated[1].IsStartCalled);
		}
		
		[Test]
		public void WhenTestRunCompletedAllFilesAreNotSavedAgain()
		{
			// feature change 2010/07/05: we save all files only once
			// during the build, not whenever a new test run starts
			context.MockSaveAllFilesCommand.IsSaveAllFilesMethodCalled = false;
			runTestCommand.CallTestsCompleted();
			Assert.IsFalse(context.MockSaveAllFilesCommand.IsSaveAllFilesMethodCalled);
		}
	}
}
