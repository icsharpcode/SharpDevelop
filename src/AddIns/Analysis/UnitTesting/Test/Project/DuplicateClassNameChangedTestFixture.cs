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
	/// SD2-1213 - Creating a unit test with the same name as an existing test
	///   
	/// Two files exist in a project each having the same unit test class. In one file the
	/// name of the class is changed. This should result in both test classes being displayed in the unit
	/// test tree.
	/// </summary>
	[TestFixture]
	public class DuplicateClassNameChangedTestFixture
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
			c.SetCompoundClass(c);
			MockMethod test1Method = new MockMethod("Test1");
			test1Method .DeclaringType = c;
			test1Method .Attributes.Add(new MockAttribute("Test"));
			c.Methods.Add(test1Method);
			
			// Test 2 method is from a duplicate test class.
			MockMethod test2Method = new MockMethod("Test2"); 
			test2Method.DeclaringType = c;
			test2Method.Attributes.Add(new MockAttribute("Test"));
			c.Methods.Add(test2Method);			
			projectContent.Classes.Add(c);
						
			testProject = new TestProject(project, projectContent);
			
			// Make sure test methods are created, otherwise
			// the Test2 method will never be looked at due to lazy evaluation
			// of test method.s
			int count = testProject.TestClasses[0].TestMethods.Count;
			
			// Change the name of the second test class.
			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			oldUnit.Classes.Add(c);
			c.Methods.Remove(test2Method); // Remove duplicate test class method.

			
			// Create new compilation unit with inner class that has its method renamed.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			MockClass newTestClass = new MockClass("RootNamespace.MyNewTestFixture");
			newTestClass.ProjectContent = projectContent;
			newTestClass.Attributes.Add(new MockAttribute("TestFixture"));
			newTestClass.SetCompoundClass(newTestClass);
			projectContent.Classes.Add(newTestClass);
			newTestClass.Methods.Add(test2Method);
			newUnit.Classes.Add(newTestClass);

			testProject.UpdateParseInfo(oldUnit, newUnit);
		}
		
		[Test]
		public void TwoTestClassesFound()
		{
			Assert.AreEqual(2, testProject.TestClasses.Count);
		}
		
		[Test]
		public void NewTestClassFound()
		{
			AssertTestClassFound("RootNamespace.MyNewTestFixture");
		}
		
		[Test]
		public void OldTestClassFound()
		{
			AssertTestClassFound("RootNamespace.MyTestFixture");
		}
		
		/// <summary>
		/// This test ensures that the existing class in the project content is used to update
		/// the test methods. So the any methods from the duplicate test class are removed.
		/// </summary>
		[Test]
		public void OldTestClassHasOneMethod()
		{
			Assert.AreEqual(1, GetTestClass("RootNamespace.MyTestFixture").TestMethods.Count);
		}
		
		[Test]
		public void OldTestClassHasOneMethodCalledTest1()
		{
			Assert.AreEqual("Test1", GetTestClass("RootNamespace.MyTestFixture").TestMethods[0].Name);
		}		
		
		void AssertTestClassFound(string name)
		{
			TestClass c = GetTestClass(name);
			Assert.IsTrue(c != null);
		}		
		
		TestClass GetTestClass(string name)
		{
			foreach (TestClass c in testProject.TestClasses) {
				if (c.QualifiedName == name) {
					return c;
				}
			}
			return null;
		}
	}
}
