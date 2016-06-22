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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockUnitTestsPadTestFixture
	{
		MockUnitTestsPad pad;
		
		[SetUp]
		public void Init()
		{
			pad = new MockUnitTestsPad();
		}
		
		[Test]
		public void IsUpdateToolbarMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(pad.IsUpdateToolbarMethodCalled);
		}
		
		[Test]
		public void IsUpdateToolbarMethodCalledReturnsTrueAfterUpdateToolbarMethodIsCalled()
		{
			pad.UpdateToolbar();
			Assert.IsTrue(pad.IsUpdateToolbarMethodCalled);
		}
		
		[Test]
		public void CanResetIsUpdateToolbarMethodAfterUpdateToolbarMethodIsCalled()
		{
			pad.UpdateToolbar();
			pad.IsUpdateToolbarMethodCalled = false;
			Assert.IsFalse(pad.IsUpdateToolbarMethodCalled);
		}
		
		[Test]
		public void IsBringToFrontMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(pad.IsBringToFrontMethodCalled);
		}
		
		[Test]
		public void IsBringToFrontMethodCalledReturnsTrueAfterBringToFrontMethodIsCalled()
		{
			pad.BringToFront();
			Assert.IsTrue(pad.IsBringToFrontMethodCalled);
		}
		
		[Test]
		public void IsResetTestResultsMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(pad.IsResetTestResultsMethodCalled);
		}
		
		[Test]
		public void IsResetTestResultsMethodCalledReturnsTrueAfterResetTestResultsMethodIsCalled()
		{
			pad.ResetTestResults();
			Assert.IsTrue(pad.IsResetTestResultsMethodCalled);
		}
		
		[Test]
		public void GetProjectsReturnsNoProjectsByDefault()
		{
			Assert.AreEqual(0, pad.GetProjects().Length);
		}
		
		[Test]
		public void GetProjectsReturnsProjectsAddedToMockUnitTestsPad()
		{
			MockCSharpProject project = new MockCSharpProject();
			pad.AddProject(project);
			
			IProject[] expectedProjects = new IProject[] { project };
			
			Assert.AreEqual(expectedProjects, pad.GetProjects());
		}
		
		[Test]
		public void GetTestProjectReturnsNullForUnknownProject()
		{
			Assert.IsNull(pad.GetTestProject(new MockCSharpProject()));
		}
		
		[Test]
		public void GetTestProjectReturnsTestProjectForProject()
		{
			MockCSharpProject project = new MockCSharpProject();
			TestProject testProject = new TestProject(project, new MockProjectContent(), new MockRegisteredTestFrameworks());
			pad.AddTestProject(testProject);
			
			Assert.AreEqual(testProject, pad.GetTestProject(project));
		}
	}
}
