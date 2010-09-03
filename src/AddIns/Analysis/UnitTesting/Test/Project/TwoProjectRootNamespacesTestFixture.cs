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
	[TestFixture]
	public class TwoRootNamespacesTestFixture
	{
		TestProject testProject;
		MockProjectContent projectContent;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void Init()
		{
			// Create a project to display in the test tree view.
			IProject project = new MockCSharpProject();
			project.Name = "TestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);
			
			// Add a test class with a TestFixture attributes.
			projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			MockClass c = new MockClass(projectContent, "RootNamespace1.MyTestFixture1");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(c);
			
			// Add a second class with a different root namespace.
			c = new MockClass(projectContent, "RootNamespace2.MyTestFixture2");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(c);
			
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			testProject = new TestProject(project, projectContent, testFrameworks);
		}
		
		[Test]
		public void TwoRootNamespaces()
		{
			Assert.AreEqual(2, testProject.RootNamespaces.Count);
		}
		
		[Test]
		public void RootNamespace1()
		{
			Assert.AreEqual("RootNamespace1", testProject.RootNamespaces[0]);
		}
		
		[Test]
		public void RootNamespace2()
		{
			Assert.AreEqual("RootNamespace2", testProject.RootNamespaces[1]);
		}
		
		[Test]
		public void OneClassInNamespace1()
		{
			Assert.AreEqual(1, testProject.GetTestClasses("RootNamespace1").Length);
		}
		
		[Test]
		public void ClassNameInNamespace1()
		{
			TestClass[] classes = testProject.GetTestClasses("RootNamespace1");
			Assert.AreEqual("MyTestFixture1", classes[0].Name);
		}
		
		[Test]
		public void OneClassInNamespace2()
		{
			Assert.AreEqual(1, testProject.GetTestClasses("RootNamespace2").Length);
		}
		
		[Test]
		public void ClassNameInNamespace2()
		{
			TestClass[] classes = testProject.GetTestClasses("RootNamespace2");
			Assert.AreEqual("MyTestFixture2", classes[0].Name);
		}
		
		[Test]
		public void TwoClassesStartWithRootNamespace()
		{
			TestClass[] classes = testProject.GetAllTestClasses("RootNamespace");
			Assert.AreEqual(2, classes.Length);
			Assert.AreEqual("MyTestFixture1", classes[0].Name);
			Assert.AreEqual("MyTestFixture2", classes[1].Name);
		}
		
		[Test]
		public void NoChildNamespaces()
		{
			Assert.AreEqual(0, testProject.GetChildNamespaces("RootNamespace").Length);
		}
		
		[Test]
		public void TwoChildNamespacesForEmptyNamespace()
		{
			string[] namespaces = testProject.GetChildNamespaces(String.Empty);
			Assert.AreEqual(2, namespaces.Length);
			Assert.AreEqual("RootNamespace1", namespaces[0]);
			Assert.AreEqual("RootNamespace2", namespaces[1]);
		}
		
		/// <summary>
		/// Makes sure that the fully qualified class name is used
		/// when working out the overall test result in the 
		/// TestClassCollection.
		/// </summary>
		[Test]
		public void DuplicateClassName()
		{
			MockClass c = new MockClass(projectContent, "RootNamespace1.MyTestFixture2");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			
			TestClass testClass = new TestClass(c, testFrameworks);
			testProject.TestClasses.Add(testClass);
			
			testClass.Result = TestResultType.Failure;
			TestClass testClass2 = testProject.TestClasses["RootNamespace2.MyTestFixture2"];
			testClass2.Result = TestResultType.Failure;
			
			Assert.AreEqual(TestResultType.Failure, testProject.TestClasses.Result);
		}
	}
}
