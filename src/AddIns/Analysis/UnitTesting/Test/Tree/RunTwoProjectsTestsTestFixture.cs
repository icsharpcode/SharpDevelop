// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
