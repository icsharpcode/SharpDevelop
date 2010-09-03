// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
