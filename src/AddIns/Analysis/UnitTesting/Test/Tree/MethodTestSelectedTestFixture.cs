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
