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
			Assert.AreEqual(method, selectedTests.Method);
		}
		
		[Test]
		public void ProjectsReturnsSingleItemContainingProjectPassedToConstructor()
		{
			List<IProject> projects = new List<IProject>(selectedTests.Projects);
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
			Assert.AreEqual(0, selectedTests.Projects.Count);
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
