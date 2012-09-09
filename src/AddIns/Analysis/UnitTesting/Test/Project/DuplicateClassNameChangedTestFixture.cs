// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
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
	public class DuplicateClassNameChangedTestFixture : ProjectTestFixtureBase
	{
		[SetUp]
		public void Init()
		{
			// Create a project to display.
			CreateNUnitProject();
			project = new MockCSharpProject();
			project.Name = "TestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);
			
			// Add a test class.
			const string program = @"
using NUnit.Framework;
namespace RootNamespace {
	class MyTestFixture {
		[Test]
		public void Foo() {}
	}
}
";
			UpdateCodeFile(program, "file1.cs");
			UpdateCodeFile(program, "file2.cs");
			
			Assert.AreEqual(2, testProject.TestClasses.Single().Members.Count);
			
			// Change the name of the second test class.
			UpdateCodeFile(program.Replace("MyTestFixture", "MyNewTestFixture"), "file2.cs");
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
		/// the test methods. So any methods from the duplicate test class are removed.
		/// </summary>
		[Test]
		public void OldTestClassHasOneMethod()
		{
			Assert.AreEqual(1, GetTestClass("RootNamespace.MyTestFixture").Members.Count);
		}
		
		[Test]
		public void OldTestClassHasOneMethodCalledTest1()
		{
			Assert.AreEqual("Test1", GetTestClass("RootNamespace.MyTestFixture").Members[0].Name);
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
