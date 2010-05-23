// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class ClassTestFixtureSelectedTestFixture
	{
		SelectedTests selectedTests;
		List<IProject> projects;
		MockCSharpProject project;
		MockClass c;
		
		[SetUp]
		public void Init()
		{
			projects = new List<IProject>();
			projects.Add(new MockCSharpProject());
			projects.Add(new MockCSharpProject());
			
			project = new MockCSharpProject();
			
			c = new MockClass();
			
			MockTestTreeView treeView = new MockTestTreeView();
			treeView.SelectedProject = project;
			treeView.SelectedClass = c;
			
			selectedTests = new SelectedTests(treeView, projects.ToArray());
		}
		
		[Test]
		public void SelectedTestsHasOneProject()
		{
			IProject[] expectedProjects = new IProject[] { project };
			List<IProject> actualProjects = new List<IProject>(selectedTests.Projects);
			
			Assert.AreEqual(expectedProjects, actualProjects.ToArray());
		}
		
		[Test]
		public void ProjectPropertyReturnsSelectedProject()
		{
			Assert.AreEqual(project, selectedTests.Project);
		}
		
		[Test]
		public void ClassPropertyReturnsSelectedClass()
		{
			Assert.AreEqual(c, selectedTests.Class);
		}
	}
}
