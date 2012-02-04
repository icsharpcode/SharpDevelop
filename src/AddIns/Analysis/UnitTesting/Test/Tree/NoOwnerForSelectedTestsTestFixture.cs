// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

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
			Assert.AreEqual(3, selectedTests.ProjectsCount);
		}
		
		[Test]
		public void SelectedProjectsMatchProjectsPassedToConstructor()
		{
			var actualProjects = selectedTests.Projects.ToArray();
			Assert.AreEqual(projects.ToArray(), actualProjects);
		}
		
		[Test]
		public void ProjectPropertyReturnsFirstProject()
		{
			Assert.AreEqual(selectedTests.Project, projects[0]);
		}
	}
}
