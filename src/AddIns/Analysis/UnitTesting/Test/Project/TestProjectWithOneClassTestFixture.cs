// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	/// Creates a TestProject that has one test class.
	/// </summary>
	[TestFixture]
	public class TestProjectWithOneClassTestFixture
	{
		TestProject testProject;
		TestClass testClass;
		MSBuildBasedProject project;
		bool resultChangedCalled;
		MockProjectContent projectContent;
		List<TestClass> classesAdded;
		List<TestClass> classesRemoved;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void Init()
		{
			resultChangedCalled = false;
			classesAdded = new List<TestClass>();
			classesRemoved = new List<TestClass>();
			
			// Create a project.
			project = new MockCSharpProject();
			project.Name = "TestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);
			
			// Add a test class with a TestFixture attributes.
			projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			MockClass c = new MockClass(projectContent, "RootNamespace.MyTestFixture");
			c.SetCompoundClass(c);
			c.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(c);
			
			// Add a second class that has no test fixture attribute.
			MockClass nonTestClass = new MockClass(projectContent);
			projectContent.Classes.Add(nonTestClass);
			
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			testProject = new TestProject(project, projectContent, testFrameworks);
			testProject.TestClasses.TestClassAdded += TestClassAdded;
			testProject.TestClasses.TestClassRemoved += TestClassRemoved;
			
			testClass = testProject.TestClasses[0];
		}
		
		[Test]
		public void OneTestClass()
		{
			Assert.AreEqual(1, testProject.TestClasses.Count);
		}
		
		[Test]
		public void TestProjectName()
		{
			Assert.AreEqual("TestProject", testProject.Name);
		}
		
		[Test]
		public void TestClassName()
		{
			Assert.AreEqual("MyTestFixture", testClass.Name);
		}
		
		[Test]
		public void TestClassQualifiedName()
		{
			Assert.AreEqual("RootNamespace.MyTestFixture", testClass.QualifiedName);
		}
		
		[Test]
		public void OneRootNamespace()
		{
			Assert.AreEqual(1, testProject.RootNamespaces.Count);
		}
		
		[Test]
		public void ProjectProperty()
		{
			Assert.AreSame(project, testProject.Project);
		}
		
		[Test]
		public void FindTestClass()
		{
			Assert.AreSame(testClass, testProject.TestClasses["RootNamespace.MyTestFixture"]);
		}
		
		[Test]
		public void NoMatchingTestClass()
		{
			Assert.IsFalse(testProject.TestClasses.Contains("NoSuchClass.MyTestFixture"));
		}
		
		[Test]
		public void TestClassResultChanged()
		{
			try {
				testClass.ResultChanged += ResultChanged;
				testClass.Result = TestResultType.Success;
			} finally {
				testClass.ResultChanged -= ResultChanged;
			}
			
			Assert.IsTrue(resultChangedCalled);
		}
		
		/// <summary>
		/// Tests that a new class is added to the TestProject
		/// from the parse info.
		/// </summary>
		[Test]
		public void NewClassInParserInfo()
		{
			// Create old compilation unit.
			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			MockClass mockClass = (MockClass)testClass.Class;
			mockClass.SetCompoundClass(mockClass);
			oldUnit.Classes.Add(testClass.Class);
			
			// Create new compilation unit with extra class.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			newUnit.Classes.Add(testClass.Class);
			MockClass newClass = new MockClass(projectContent, "RootNamespace.MyNewTestFixture");
			newClass.Attributes.Add(new MockAttribute("TestFixture"));
			newClass.SetCompoundClass(newClass);
			newUnit.Classes.Add(newClass);
			
			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			Assert.IsTrue(testProject.TestClasses.Contains("RootNamespace.MyNewTestFixture"));
			Assert.AreEqual(1, classesAdded.Count);
			Assert.AreSame(newClass, classesAdded[0].Class);
		}
		
		/// <summary>
		/// Tests that the TestProject.UpdateParseInfo handles the
		/// case when the old compilation unit does not have a
		/// test class, the new compilation unit does have the
		/// test class, but the test class has already been
		/// added to our TestProject.
		/// </summary>
		[Test]
		public void TestClassInNewCompilationUnitOnly()
		{
			// Create old compilation unit.
			projectContent.Classes.Clear();
			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			
			// Create new compilation unit with class that 
			// already exists in the project.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			newUnit.Classes.Add(testClass.Class);
			MockClass c = new MockClass(projectContent, "RootNamespace.MyTestFixture");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			c.SetCompoundClass(c);
			newUnit.Classes.Add(c);
			
			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			Assert.IsTrue(testProject.TestClasses.Contains("RootNamespace.MyTestFixture"));
			Assert.AreEqual(0, classesAdded.Count);
		}
		
		/// <summary>
		/// New class without a test fixture attribute should not
		/// be added to the list of test classes.
		/// </summary>
		[Test]
		public void NewClassInParserInfoWithoutTestFixtureAttribute()
		{
			// Create old compilation unit.
			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			MockClass mockClass = (MockClass)testClass.Class;
			mockClass.SetCompoundClass(mockClass);
			oldUnit.Classes.Add(testClass.Class);
			
			// Create new compilation unit with extra class.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			newUnit.Classes.Add(testClass.Class);
			MockClass newClass = new MockClass(projectContent, "RootNamespace.MyNewTestFixture");
			newClass.SetCompoundClass(newClass);
			newUnit.Classes.Add(newClass);
			
			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			Assert.IsFalse(testProject.TestClasses.Contains("RootNamespace.MyNewTestFixture"));
		}
		
		[Test]
		public void TestClassRemovedInParserInfo()
		{
			// Create old compilation unit.
			projectContent.Classes.Clear();
			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			MockClass mockClass = (MockClass)testClass.Class;
			mockClass.SetCompoundClass(mockClass);
			oldUnit.Classes.Add(testClass.Class);
			
			// Create new compilation unit with the original test class
			// removed.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			
			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			Assert.AreEqual(0, testProject.TestClasses.Count);
			Assert.AreEqual(1, classesRemoved.Count);
			Assert.AreSame(testClass, classesRemoved[0]);
		}
		
		[Test]
		public void NewCompilationUnitNull()
		{
			// Create old compilation unit.
			projectContent.Classes.Clear();
			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			MockClass mockClass = (MockClass)testClass.Class;
			mockClass.SetCompoundClass(mockClass);
			oldUnit.Classes.Add(testClass.Class);
			
			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, null);
			
			Assert.AreEqual(0, testProject.TestClasses.Count);
			Assert.AreEqual(1, classesRemoved.Count);
			Assert.AreSame(testClass, classesRemoved[0]);
		}
		
		/// <summary>
		/// Tests that a new method is added to the TestClass
		/// from the parse info. Also checks that the test method is 
		/// taken from the CompoundClass via IClass.GetCompoundClass. 
		/// A CompoundClass combines partial classes into one class so
		/// we do not get any duplicate classes with the same name.
		/// </summary>
		[Test]
		public void NewMethodInParserInfo()
		{
			// Create old compilation unit.
			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(projectContent);
			oldUnit.Classes.Add(testClass.Class);
			
			// Create new compilation unit.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			newUnit.Classes.Add(testClass.Class);
			
			// Add a new method to a new compound class.
			MockClass compoundClass = new MockClass(projectContent, "RootNamespace.MyTestFixture");
			compoundClass.Attributes.Add(new MockAttribute("TestFixture"));
			MockMethod method = new MockMethod(testClass.Class, "NewMethod");
			method.Attributes.Add(new MockAttribute("Test"));
			compoundClass.Methods.Add(method);
			MockClass mockClass = (MockClass)testClass.Class;
			mockClass.SetCompoundClass(compoundClass);
			
			// Monitor test methods added.
			List<TestMember> methodsAdded = new List<TestMember>();
			testClass.TestMembers.TestMemberAdded += delegate(Object source, TestMemberEventArgs e)
				{ methodsAdded.Add(e.TestMember); };

			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);
	
			Assert.IsTrue(testClass.TestMembers.Contains("NewMethod"));
			Assert.AreEqual(1, methodsAdded.Count);
			Assert.AreSame(method, methodsAdded[0].Member);
		}
		
		[Test]
		public void ParserInfoForDifferentProject()
		{
			// Create old compilation unit.
			MockProjectContent differentProjectContent = new MockProjectContent();
			DefaultCompilationUnit oldUnit = new DefaultCompilationUnit(differentProjectContent);
			MockClass mockClass = (MockClass)testClass.Class;
			mockClass.SetCompoundClass(mockClass);
			oldUnit.Classes.Add(testClass.Class);
			
			// Create new compilation unit with the original test class
			// removed.
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(differentProjectContent);
			
			// Update TestProject's parse info.
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			Assert.AreEqual(1, testProject.TestClasses.Count);
			Assert.AreEqual(0, classesRemoved.Count);
			Assert.AreEqual(0, classesAdded.Count);
		}
		
		[Test]
		public void ParserInfoForThisProjectOldCompilationUnitNull()
		{
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(projectContent);
			Assert.IsTrue(testProject.IsParseInfoForThisProject(null, newUnit));
		}
		
		[Test]
		public void ParserInfoForDifferentProjectOldCompilationUnitNull()
		{
			MockProjectContent differentProjectContent = new MockProjectContent();
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(differentProjectContent);
			
			Assert.IsFalse(testProject.IsParseInfoForThisProject(null, newUnit));
		}
		
		[Test]
		public void ParseInfoForThisProjectWhenBothCompilationUnitsNull()
		{
			Assert.IsFalse(testProject.IsParseInfoForThisProject(null, null));
		}
		
		void ResultChanged(object source, EventArgs e)
		{
			resultChangedCalled = true;
		}
		
		void TestClassAdded(object source, TestClassEventArgs e)
		{
			classesAdded.Add(e.TestClass);
		}
		
		void TestClassRemoved(object source, TestClassEventArgs e)
		{
			classesRemoved.Add(e.TestClass);
		}
	}
}
