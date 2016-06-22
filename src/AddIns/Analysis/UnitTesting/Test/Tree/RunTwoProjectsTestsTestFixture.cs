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
	public class RunTwoProjectsTestsTestFixture : RunTestCommandTestFixtureBase
	{
		TestProject testProject1;
		MockCSharpProject project1;
		TestProject testProject2;
		MockCSharpProject project2;
		MockBuildProjectBeforeTestRun buildProject1;
		
		[SetUp]
		public void Init()
		{
			base.InitBase();
			
			project1 = new MockCSharpProject();
			testProject1 = 
				TestProjectHelper.CreateTestProjectWithTestClassAndSingleTestMethod(project1, "testClass1", "testMethod1");
			
			project2 = new MockCSharpProject();
			testProject2 = 
				TestProjectHelper.CreateTestProjectWithTestClassAndSingleTestMethod(project1, "testClass2", "testMethod2");

			context.MockUnitTestsPad.AddProject(project1);
			context.MockUnitTestsPad.AddProject(project2);
			context.MockUnitTestsPad.AddTestProject(testProject1);
			context.MockUnitTestsPad.AddTestProject(testProject2);
			
			buildProject1 = new MockBuildProjectBeforeTestRun();
			
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProject1);
			
			runTestCommand.Run();
			
			buildProject1.FireBuildCompleteEvent();
			runTestCommand.CallTestsCompleted();
		}
		
		[Test]
		public void IsRunningTestReturnsTrueSinceOneTestProject()
		{
			Assert.IsTrue(AbstractRunTestCommand.IsRunningTest);
		}
		
		[Test]
		public void SecondProjectIsBuilt()
		{
			Assert.IsTrue(buildProject1.IsRunMethodCalled);
			Assert.AreEqual(new[] { project1, project2 }, buildProject1.Projects);
		}
		
		[Test]
		public void IsRunningTestReturnsFalseAfterSecondProjectTestsHaveAllRun()
		{
			runTestCommand.CallTestsCompleted();
			Assert.IsFalse(AbstractRunTestCommand.IsRunningTest);
		}
		
		[Test]
		public void OnBeforeTestRunIsCalledOnlyOnce()
		{
			int count = runTestCommand.OnBeforeRunTestsMethodCallCount;
			Assert.AreEqual(1, count);
		}
	}
}
