// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public static class TestProjectHelper
	{
		public static TestProject CreateTestProjectWithTestClassAndSingleTestMethod(IProject project, 
			string className, 
			string methodName)
		{
			string[] methodNames = new string[] { methodName };
			return CreateTestProjectWithTestClassTestMethods(project, className, methodNames);
		}
		
		public static TestProject CreateTestProjectWithTestClassAndSingleTestMethod(string className, string methodName)
		{
			MockCSharpProject project = new MockCSharpProject();
			return CreateTestProjectWithTestClassAndSingleTestMethod(project, className, methodName);
		}
		
		public static TestProject CreateTestProjectWithTestClassTestMethods(string className, string[] methodNames)
		{
			MockCSharpProject project = new MockCSharpProject();
			return CreateTestProjectWithTestClassTestMethods(project, className, methodNames);
		}
		
		public static TestProject CreateTestProjectWithTestClassTestMethods(IProject project, string className, string[] methodNames)
		{
			MockRegisteredTestFrameworks testFrameworks = new MockRegisteredTestFrameworks();
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			c.MockProjectContent.Project = project;
			c.SetDotNetName(className);
			c.CompilationUnit.FileName = @"c:\projects\tests\MyTests.cs";
			TestClass testClass = new TestClass(c, testFrameworks);
			
			foreach (string methodName in methodNames) {
				MockMethod method = new MockMethod(c, methodName);
				method.Region = new DomRegion(4, 20);
				c.Methods.Add(method);
				
				TestMember testMember = new TestMember(method);
				testClass.TestMembers.Add(testMember);
			}
			
			c.Project.Name = "TestProject";
			TestProject testProject = new TestProject(c.Project, c.ProjectContent, testFrameworks);
			testProject.TestClasses.Add(testClass);
			
			return testProject;
		}
	}
}
