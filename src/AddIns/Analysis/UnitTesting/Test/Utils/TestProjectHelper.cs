// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
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
				testClass.Members.Add(testMember);
			}
			
			c.Project.Name = "TestProject";
			TestProject testProject = new TestProject(c.Project, c.ProjectContent, testFrameworks);
			testProject.TestClasses.Add(testClass);
			
			return testProject;
		}
	}
}
