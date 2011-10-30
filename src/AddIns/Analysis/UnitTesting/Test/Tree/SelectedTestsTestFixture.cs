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
	public class SelectedTestsTestFixture
	{
		SelectedTests selectedTests;
		MockCSharpProject project;
		string namespaceFilter = "MyNamespace.Tests";
		MockClass c;
		MockMethod method;
		
		[SetUp]
		public void Init()
		{
			project = new MockCSharpProject();
			c = new MockClass();
			method = new MockMethod(c);
			selectedTests = new SelectedTests(project, namespaceFilter, c, method);
		}
		
		[Test]
		public void ProjectPropertyMatchesProjectPassedToConstructor()
		{
			Assert.AreEqual(project, selectedTests.Project);
		}
		
		[Test]
		public void ClassPropertyMatchesClassPassedToConstructor()
		{
			Assert.AreEqual(c, selectedTests.Class);
		}
		
		[Test]
		public void MethodPropertyMatchesMethodPassedToConstructor()
		{
			Assert.AreEqual(method, selectedTests.Member);
		}
		
		[Test]
		public void ProjectsReturnsSingleItemContainingProjectPassedToConstructor()
		{
			var projects = selectedTests.Projects.ToArray();
			IProject[] expectedProjects = new IProject[] { project };
			
			Assert.AreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void HasProjectsReturnsTrue()
		{
			Assert.IsTrue(selectedTests.HasProjects);
		}
		
		[Test]
		public void NamespaceFilterPropertyMatchesNamespaceFilterPassedToConstructor()
		{
			Assert.AreEqual(namespaceFilter, selectedTests.NamespaceFilter);
		}
		
		[Test]
		public void RemoveFirstProjectLeavesNoProjects()
		{
			selectedTests.RemoveFirstProject();
			Assert.AreEqual(0, selectedTests.ProjectsCount);
		}
		
		[Test]
		public void HasProjectReturnsFalseAfterRemoveFirstProjectCalled()
		{
			selectedTests.RemoveFirstProject();
			Assert.IsFalse(selectedTests.HasProjects);
		}
		
		[Test]
		public void RemovingFirstProjectTwiceDoesNotThrowException()
		{
			selectedTests.RemoveFirstProject();
			Assert.DoesNotThrow(delegate { selectedTests.RemoveFirstProject(); });
		}
	}
}
