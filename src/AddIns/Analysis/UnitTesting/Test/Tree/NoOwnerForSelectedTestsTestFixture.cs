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
	public class NoOwnerForSelectedTestsTestFixture
	{
		SelectedTests selectedTests;
		List<IProject> projects;
		
		[SetUp]
		public void Init()
		{
			projects = new List<IProject>();
			projects.Add(new MockCSharpProject());
			projects.Add(new MockCSharpProject());
			projects.Add(new MockCSharpProject());
			
			selectedTests = new SelectedTests(null, projects.ToArray());
		}
		
		[Test]
		public void SelectedTestsHaveThreeProjects()
		{
			Assert.AreEqual(3, selectedTests.Projects.Count);
		}
		
		[Test]
		public void SelectedProjectsMatchProjectsPassedToConstructor()
		{
			List<IProject> actualProjects = new List<IProject>(selectedTests.Projects);
			Assert.AreEqual(projects.ToArray(), actualProjects.ToArray());
		}
		
		[Test]
		public void ProjectPropertyReturnsFirstProject()
		{
			Assert.AreEqual(selectedTests.Project, projects[0]);
		}
	}
}
