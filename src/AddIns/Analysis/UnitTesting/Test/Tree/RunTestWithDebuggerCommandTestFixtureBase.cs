// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
