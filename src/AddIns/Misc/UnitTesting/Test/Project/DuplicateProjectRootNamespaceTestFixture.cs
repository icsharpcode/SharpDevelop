// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that only one root namespace is returned when two
	/// classes contain the same root namespace.
	/// </summary>
	[TestFixture]
	public class DuplicateProjectRootNamespaceTestFixture
	{
		TestProject testProject;

		[SetUp]
		public void Init()
		{
			// Create a project.
			IProject project = new MockCSharpProject();
			project.Name = "TestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);
			
			// Add a test class with a TestFixture attributes.
			MockProjectContent projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			MockClass c = new MockClass("RootNamespace.MyTestFixture");
			c.Namespace = "RootNamespace";
			c.Attributes.Add(new MockAttribute("TestFixture"));
			c.ProjectContent = projectContent;
			projectContent.Classes.Add(c);
			
			// Add a second class the same root namespace.
			c = new MockClass("RootNamespace.MyTestFixture2");
			c.Namespace = "RootNamespace";
			c.Attributes.Add(new MockAttribute("TestFixture"));
			c.ProjectContent = projectContent;
			projectContent.Classes.Add(c);

			testProject = new TestProject(project, projectContent);
		}
		
		[Test]
		public void OneRootNamespace()
		{
			Assert.AreEqual(1, testProject.RootNamespaces.Count);
		}
		
		[Test]
		public void RootNamespace()
		{
			Assert.AreEqual("RootNamespace", testProject.RootNamespaces[0]);
		}
		
		[Test]
		public void OneChildNamespacesForEmptyNamespace()
		{
			string[] namespaces = testProject.GetChildNamespaces(String.Empty);
			Assert.AreEqual(1, namespaces.Length);
			Assert.AreEqual("RootNamespace", namespaces[0]);
		}
	}
}
