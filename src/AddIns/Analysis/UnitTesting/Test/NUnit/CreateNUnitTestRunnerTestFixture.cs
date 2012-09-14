// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using Rhino.Mocks;
using ICSharpCode.SharpDevelop.Project;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class CreateNUnitTestRunnerTestFixture
	{
		IProject project;
		NUnitTestProject testProject;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			project = MockRepository.GenerateStub<IProject>();
			testProject = new NUnitTestProject(project);
		}
		
		[Test]
		public void NUnitTestProjectCreateTestRunnerReturnsNUnitTestRunner()
		{
			Assert.IsInstanceOf(typeof(NUnitTestRunner), testProject.CreateTestRunner(new TestExecutionOptions()));
		}
		
		[Test]
		public void NUnitTestProjectCreateTestRunnerReturnsNUnitTestDebugger()
		{
			Assert.IsInstanceOf(typeof(NUnitTestDebugger), testProject.CreateTestRunner(new TestExecutionOptions { UseDebugger = true }));
		}
		
		[Test]
		public void NUnitTestProjectBuildsTheProject()
		{
			Assert.AreSame(project, testProject.GetBuildableForTesting());
		}
	}
}
