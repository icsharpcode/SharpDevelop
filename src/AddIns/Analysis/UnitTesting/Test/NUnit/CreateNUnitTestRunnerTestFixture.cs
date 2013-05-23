// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.UnitTesting;
using UnitTesting.Tests.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop;

namespace UnitTesting.Tests.NUnit
{
	[TestFixture]
	public class CreateNUnitTestRunnerTestFixture : SDTestFixtureBase
	{
		IProject project;
		NUnitTestProject testProject;
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			SD.Services.AddService(typeof(IBookmarkManager), MockRepository.GenerateStub<IBookmarkManager>());
			SD.Services.AddService(typeof(IProjectService), MockRepository.GenerateStub<IProjectService>());
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
			Assert.IsTrue(testProject.IsBuildNeededBeforeTestRun);
		}
	}
}
