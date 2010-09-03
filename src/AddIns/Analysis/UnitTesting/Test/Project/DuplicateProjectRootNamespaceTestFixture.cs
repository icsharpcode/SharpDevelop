// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
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
			MockClass c = new MockClass(projectContent, "RootNamespace.MyTestFixture");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(c);
			
			// Add a second class the same root namespace.
			c = new MockClass(projectContent, "RootNamespace.MyTestFixture2");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(c);
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			testProject = new TestProject(project, projectContent, testFrameworks);
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
