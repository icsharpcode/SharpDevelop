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
	public class MethodTestSelectedTestFixture
	{
		SelectedTests selectedTests;
		List<IProject> projects;
		MockCSharpProject project;
		MockMethod method;
		MockClass c;
		
		[SetUp]
		public void Init()
		{
			projects = new List<IProject>();
			projects.Add(new MockCSharpProject());
			projects.Add(new MockCSharpProject());
			
			project = new MockCSharpProject();
			
			c = new MockClass();
			method = new MockMethod(c);
			
			MockTestTreeView treeView = new MockTestTreeView();
			treeView.SelectedProject = project;
			treeView.SelectedNamespace = "MyNamespace";
			treeView.SelectedMember = method;
			
			selectedTests = new SelectedTests(treeView, projects.ToArray());
		}
		
		[Test]
		public void SelectedTestsHasOneProject()
		{
			IProject[] expectedProjects = new IProject[] { project };
			var actualProjects = selectedTests.Projects.ToArray();
			
			Assert.AreEqual(expectedProjects, actualProjects);
		}
		
		[Test]
		public void ProjectPropertyReturnsSelectedProject()
		{
			Assert.AreEqual(project, selectedTests.Project);
		}
		
		[Test]
		public void NamespaceFilterIsMyNamespace()
		{
			Assert.AreEqual("MyNamespace", selectedTests.NamespaceFilter);
		}
		
		[Test]
		public void MethodPropertyReturnsSelectedMethod()
		{
			Assert.AreEqual(method, selectedTests.Member);
		}
		
		[Test]
		public void ClassPropertyReturnsMethodDeclaringType()
		{
			Assert.AreEqual(c, selectedTests.Class);
		}
	}
}
