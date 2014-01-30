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
