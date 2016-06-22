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
