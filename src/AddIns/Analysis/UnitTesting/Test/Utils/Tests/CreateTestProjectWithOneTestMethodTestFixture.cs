// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class CreateTestProjectWithOneTestMethodTestFixture
	{
		TestProject testProject;
		TestClass testClass;
		TestMember testMethod;
		
		[SetUp]
		public void Init()
		{
			testProject = 
				TestProjectHelper.CreateTestProjectWithTestClassAndSingleTestMethod("MyNamespace.MyClass", "MyTestMethod");
			
			if (testProject.TestClasses.Count > 0) {
				testClass = testProject.TestClasses[0];
				if (testClass.TestMembers.Count > 0) {
					testMethod = testClass.TestMembers[0];
				}
			}	
		}
		
		[Test]
		public void TestProjectNameIsTestProject()
		{
			string expectedName = "TestProject";
			Assert.AreEqual(expectedName, testProject.Name);
		}
		
		[Test]
		public void TestProjectNameMatchesProjectName()
		{
			Assert.AreEqual(testProject.Project.Name, testProject.Name);
		}
		
		[Test]
		public void TestProjectHasOneTestClass()
		{
			Assert.AreEqual(1, testProject.TestClasses.Count);
		}
		
		[Test]
		public void TestClassDotNetNameIsMyNamespaceMyClass()
		{
			string expectedName = "MyNamespace.MyClass";
			Assert.AreEqual(expectedName, testClass.Class.DotNetName);
		}
		
		[Test]
		public void TestClassCompilationUnitFileNameIsProjectsTestsMyTestsCs()
		{
			string fileName = @"c:\projects\tests\MyTests.cs";
			Assert.AreEqual(fileName, testClass.Class.CompilationUnit.FileName);
		}
		
		[Test]
		public void TestClassHasOneTestMethod()
		{
			Assert.AreEqual(1, testClass.TestMembers.Count);
		}
		
		[Test]
		public void TestMethodNameIsMyTestMethod()
		{
			Assert.AreEqual("MyTestMethod", testMethod.Name);
		}
		
		[Test]
		public void TestMethodDeclaringTypeIsNotNull()
		{
			Assert.IsNotNull(testMethod.Member.DeclaringType);
		}
		
		[Test]
		public void TestMethodDeclaringTypeEqualsTestClass()
		{
			Assert.AreEqual(testClass.Class, testMethod.Member.DeclaringType);
		}
		
		[Test]
		public void TestMethodRegionIsLine4Column20()
		{
			DomRegion expectedRegion = new DomRegion(4, 20);
			Assert.AreEqual(expectedRegion, testMethod.Member.Region);
		}
		
		[Test]
		public void ClassHasOneMethod()
		{
			Assert.AreEqual(1, testClass.Class.Methods.Count);
		}
		
		[Test]
		public void ClassMethodMatchesTestMethodMethod()
		{
			Assert.AreEqual(testClass.Class.Methods[0], testMethod.Member);
		}
	}
}
