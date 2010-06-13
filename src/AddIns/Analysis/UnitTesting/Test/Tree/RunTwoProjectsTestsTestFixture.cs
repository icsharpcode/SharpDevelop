// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		MockBuildProjectBeforeTestRun buildProject2;
		
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
			buildProject2 = new MockBuildProjectBeforeTestRun();
			
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProject1);
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProject2);
			
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
			Assert.IsTrue(buildProject2.IsRunMethodCalled);
		}
		
		[Test]
		public void IsRunningTestReturnsFalseAfterSecondProjectTestsHaveAllRun()
		{
			runTestCommand.CallTestsCompleted();
			Assert.IsFalse(AbstractRunTestCommand.IsRunningTest);
		}
	}
}
