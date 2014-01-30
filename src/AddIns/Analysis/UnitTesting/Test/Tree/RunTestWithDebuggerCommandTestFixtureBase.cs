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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	public class RunTestWithDebuggerCommandTestFixtureBase
	{
		string oldRootPath = String.Empty;
		protected DerivedRunTestWithDebuggerCommand runCommand;
		protected MockDebuggerService debuggerService;
		protected MockRunTestCommandContext context;
		protected MockCSharpProject project;
		protected MockBuildProjectBeforeTestRun buildProject;
		protected MockNUnitTestFramework testFramework;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			oldRootPath = FileUtility.ApplicationRootPath;
			FileUtility.ApplicationRootPath = @"D:\SharpDevelop";
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			FileUtility.ApplicationRootPath = oldRootPath;
		}
		
		public void InitBase()
		{
			project = new MockCSharpProject();
			buildProject = new MockBuildProjectBeforeTestRun();
			
			context = new MockRunTestCommandContext();
			context.MockUnitTestsPad.AddProject(project);
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProject);
			
			debuggerService = new MockDebuggerService();
			testFramework = 
				new MockNUnitTestFramework(debuggerService,
					context.MockTestResultsMonitor,
					context.UnitTestingOptions,
					context.MessageService);
			context.MockRegisteredTestFrameworks.AddTestFrameworkForProject(project, testFramework);
			
			runCommand = new DerivedRunTestWithDebuggerCommand(context);
		}
		
		[Test]
		public void ExpectedBuildProjectReturnedFromBuildFactory()
		{
			InitBase();
			Assert.AreEqual(buildProject, context.MockBuildProjectFactory.CreateBuildProjectBeforeTestRun(new IProject[0]));
		}
		
		[Test]
		public void ExpectedTestFrameworkReturnedFromRegisteredFrameworks()
		{
			InitBase();
			Assert.AreEqual(testFramework, context.MockRegisteredTestFrameworks.GetTestFrameworkForProject(project));
		}
	}
}
