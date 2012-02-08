// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class AbstractRunTestCommandTests
	{
		DerivedRunTestCommand runTestCommand;
		MockRunTestCommandContext runTestCommandContext;
		
		void CreateRunTestCommand()
		{
			runTestCommandContext = new MockRunTestCommandContext();
			runTestCommand = new DerivedRunTestCommand(runTestCommandContext);
		}
		
		void UnitTestsPadNotCreated()
		{
			runTestCommandContext.MockUnitTestsPad = null;
		}
				
		void NoSolutionOpen()
		{
			runTestCommandContext.Solution = null;
		}
		
		IProject SolutionWithOneProjectOpen()
		{
			var solution = new Solution(new MockProjectChangeWatcher());
			var project = new MockCSharpProject(solution, "MyProject");
			solution.AddFolder(project);
			
			runTestCommandContext.Solution = solution;
			
			return project;
		}
		
		IProject GetFirstProjectTestedByTestRunner()
		{
			return runTestCommand
				.TestRunnersCreated
				.First()
				.SelectedTestsPassedToStartMethod
				.Project;
		}
		
		void BuildIsNotNeededBeforeTestRunForProject(IProject expectedProject)
		{
			MockTestFramework testFramework = RegisterTestFrameworkForProject(expectedProject);
			testFramework.IsBuildNeededBeforeTestRun = false;			
		}
		
		MockTestFramework RegisterTestFrameworkForProject(IProject project)
		{
			var testFramework = new MockTestFramework();
			runTestCommandContext
				.MockRegisteredTestFrameworks
					.AddTestFrameworkForProject(project, testFramework);
			
			return testFramework;
		}
		
		[Test]
		public void Run_UnitTestsPadNotCreatedAndNoSolutionOpen_NullReferenceExceptionNotThrown()
		{
			CreateRunTestCommand();
			NoSolutionOpen();
			
			Assert.DoesNotThrow(() => runTestCommand.Run());
		}
		
		[Test]
		public void Run_UnitTestsPadNotCreatedAndNoSolutionOpen_OnBeforeRunIsNotCalled()
		{
			CreateRunTestCommand();
			UnitTestsPadNotCreated();
			NoSolutionOpen();
			
			runTestCommand.Run();
			
			Assert.IsFalse(runTestCommand.IsOnBeforeBuildMethodCalled);
		}
		
		[Test]
		public void Run_UnitTestsPadNotCreatedAndSolutionWithOneProjectOpen_TestsRunForProject()
		{
			CreateRunTestCommand();
			UnitTestsPadNotCreated();
			IProject expectedProject = SolutionWithOneProjectOpen();
			BuildIsNotNeededBeforeTestRunForProject(expectedProject);
			
			runTestCommand.Run();
			
			IProject project = GetFirstProjectTestedByTestRunner();
			
			Assert.AreEqual(expectedProject, project);
		}
	}
}
