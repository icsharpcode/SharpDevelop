// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class TestProjectUsesTestFrameworksTestFixture
	{
		MockRegisteredTestFrameworks testFrameworks;
		TestProject testProject;
		MockClass myTestClass;
		DefaultCompilationUnit oldUnit;
		
		[SetUp]
		public void Init()
		{
			testFrameworks = new MockRegisteredTestFrameworks();
			myTestClass = MockClass.CreateMockClassWithoutAnyAttributes();
			myTestClass.SetDotNetName("MyTests");
			testFrameworks.AddTestClass(myTestClass);
			
			oldUnit = new DefaultCompilationUnit(myTestClass.ProjectContent);
			oldUnit.Classes.Add(myTestClass);
			
			testProject = new TestProject(myTestClass.Project, myTestClass.ProjectContent, testFrameworks);
		}
		
		[Test]
		public void TestProjectHasTestClassCalledMyTests()
		{
			Assert.AreEqual(myTestClass, testProject.TestClasses[0].Class);
		}
		
		[Test]
		public void NewTestClassInNewCompilationUnitAddedToTestProjectTestClasses()
		{
			MockClass myNewTestClass = MockClass.CreateMockClassWithoutAnyAttributes();
			myNewTestClass.SetDotNetName("MyNewTests");
			testFrameworks.AddTestClass(myNewTestClass);
			
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(myTestClass.ProjectContent);
			newUnit.Classes.Add(myTestClass);
			newUnit.Classes.Add(myNewTestClass);
			
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			Assert.AreEqual(myNewTestClass, testProject.TestClasses[1].Class);
		}
		
		[Test]
		public void NewInnerTestClassInNewCompilationUnitAddedToTestProjectTestClasses()
		{
			MockClass myNewInnerTestClass = MockClass.CreateMockClassWithoutAnyAttributes();
			myNewInnerTestClass.SetDotNetName("MyNewInnerTests");
			testFrameworks.AddTestClass(myNewInnerTestClass);
			myTestClass.InnerClasses.Add(myNewInnerTestClass);
			
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(myTestClass.ProjectContent);
			newUnit.Classes.Add(myTestClass);
			
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			Assert.AreEqual(myNewInnerTestClass, testProject.TestClasses[1].Class);
		}
		
		[Test]
		public void TestProjectRemovesTestClassWhenItIsNoLongerATestClass()
		{
			testFrameworks.RemoveTestClass(myTestClass);
			
			DefaultCompilationUnit newUnit = new DefaultCompilationUnit(myTestClass.ProjectContent);
			newUnit.Classes.Add(myTestClass);
			
			testProject.UpdateParseInfo(oldUnit, newUnit);
			
			Assert.AreEqual(0, testProject.TestClasses.Count);
		}
	}
}
