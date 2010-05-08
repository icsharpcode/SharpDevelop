// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that the TestProject can handle the user
	/// opening a project with two test classes with the same
	/// fully qualified name.
	/// </summary>
	[TestFixture]
	public class DuplicateClassNameTestFixture
	{
		TestProject testProject;
		IProject project;
		MockProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			// Create a project to display.
			project = new MockCSharpProject();
			project.Name = "TestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);
			
			// Add a test class.
			projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			MockClass c = new MockClass("RootNamespace.MyTestFixture");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			c.ProjectContent = projectContent;
			projectContent.Classes.Add(c);
			
			// Add a second class with the same name.
			MockClass secondTestClass = new MockClass("RootNamespace.MyTestFixture");
			secondTestClass.ProjectContent = projectContent;
			secondTestClass.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(secondTestClass);
			
			testProject = new TestProject(project, projectContent);
		}
		
		/// <summary>
		/// If one or more classes exist with the same fully qualified
		/// name only one should be added to the test project. The
		/// project will not compile anyway due to the duplicate class 
		/// name so only having one test class is probably an OK
		/// workaround.
		/// </summary>
		[Test]
		public void OneTestClass()
		{
			Assert.AreEqual(1, testProject.TestClasses.Count);
		}
		
		[Test]
		public void TestClassName()
		{
			Assert.AreEqual("RootNamespace.MyTestFixture", testProject.TestClasses[0].QualifiedName);
		}
	}
}
