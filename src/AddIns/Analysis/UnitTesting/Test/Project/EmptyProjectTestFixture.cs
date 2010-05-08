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
	/// Creates a TestProject object with no test classes.
	/// </summary>
	[TestFixture]
	public class EmptyProjectTestFixture
	{
		TestProject testProject;
		MockProjectContent projectContent;

		[SetUp]
		public void Init()
		{
			// Create a project to display.
			IProject project = new MockCSharpProject();
			project.Name = "TestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);
			
			projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			
			testProject = new TestProject(project, projectContent);
		}
		
		/// <summary>
		/// Tests that a new class is added to the TestProject
		/// from the parse info when the old compilation unit is null.
		/// </summary>
		[Test]
		public void NewClassInParserInfo()
		{			
			// Create new compilation unit with extra class.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			MockClass newClass = new MockClass("RootNamespace.MyNewTestFixture");
			newClass.Attributes.Add(new MockAttribute("TestFixture"));
			newClass.ProjectContent = projectContent;
			newClass.SetCompoundClass(newClass);
			newUnit.Classes.Add(newClass);
			
			// Update TestProject's parse info.
			testProject.UpdateParseInfo(null, newUnit);
			
			Assert.IsTrue(testProject.TestClasses.Contains("RootNamespace.MyNewTestFixture"));
		}
		
		/// <summary>
		/// The class exists in both the old compilation unit and the
		/// new compilation unit, but in the new compilation unit 
		/// it has an added [TestFixture] attribute.
		/// </summary>
		[Test]
		public void TestFixtureAttributeAdded()
		{
			// Create an old compilation unit with the test class
			// but without a [TestFixture] attribute.
			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			MockClass newClass = new MockClass("RootNamespace.MyNewTestFixture");
			newClass.ProjectContent = projectContent;
			newClass.SetCompoundClass(newClass);
			oldUnit.Classes.Add(newClass);

			// Create a new compilation unit with the test class
			// having a [TestFixture] attribute.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			newClass = new MockClass("RootNamespace.MyNewTestFixture");
			newClass.Attributes.Add(new MockAttribute("TestFixture"));
			newClass.ProjectContent = projectContent;
			newClass.SetCompoundClass(newClass);
			newUnit.Classes.Add(newClass);

			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			Assert.IsTrue(testProject.TestClasses.Contains("RootNamespace.MyNewTestFixture"),
				"New class should have been added to the set of TestClasses.");
		}
		
		/// <summary>
		/// The class exists in both the old compilation unit and the
		/// new compilation unit, but in the new compilation unit 
		/// the [TestFixture] attribute has been removed.
		/// </summary>
		[Test]
		public void TestFixtureAttributeRemoved()
		{
			// Add the test class first.
			TestFixtureAttributeAdded();
			
			// Create an old compilation unit with the test class
			// having a [TestFixture] attribute.
			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			MockClass newClass = new MockClass("RootNamespace.MyNewTestFixture");
			newClass.Attributes.Add(new MockAttribute("TestFixture"));
			newClass.ProjectContent = projectContent;
			newClass.SetCompoundClass(newClass);
			oldUnit.Classes.Add(newClass);

			// Create a new compilation unit with the test class
			// but without a [TestFixture] attribute.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			newClass = new MockClass("RootNamespace.MyNewTestFixture");
			newClass.ProjectContent = projectContent;
			newClass.SetCompoundClass(newClass);
			newUnit.Classes.Add(newClass);

			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			Assert.IsFalse(testProject.TestClasses.Contains("RootNamespace.MyNewTestFixture"),
				"Class should have been removed.");
		}
	}
}
