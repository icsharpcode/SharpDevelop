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
	public class TestClassWithTwoMethodsTestFixture
	{
		TestProject testProject;
		TestClass testClass;
		TestMember testMethod1;
		TestMember testMethod2;
		MockClass mockClass;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void Init()
		{
			IProject project = new MockCSharpProject();
			project.Name = "TestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, nunitFrameworkReferenceItem);

			MockProjectContent projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			
			mockClass = new MockClass(projectContent, "RootNamespace.Tests.MyTestFixture");
			mockClass.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(mockClass);
			
			// Add a method to the test class
			MockMethod mockMethod = new MockMethod(mockClass, "TestMethod1");
			mockMethod.Attributes.Add(new MockAttribute("Test"));
			mockClass.Methods.Add(mockMethod);
			
			mockMethod = new MockMethod(mockClass, "TestMethod2");
			mockMethod.Attributes.Add(new MockAttribute("Test"));
			mockClass.Methods.Add(mockMethod);
			
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			testProject = new TestProject(project, projectContent, testFrameworks);
			testClass = testProject.TestClasses[0];
			testMethod1 = testClass.TestMembers[0];
			testMethod2 = testClass.TestMembers[1];
		}
		
		[Test]
		public void TwoMethods()
		{
			Assert.AreEqual(2, testClass.TestMembers.Count);
		}
			
		[Test]
		public void TestMethod1Failed()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod1");
			result.ResultType = TestResultType.Failure;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Failure, testMethod1.Result);
		}
		
		[Test]
		public void TestMethod1Ignored()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod1");
			result.ResultType = TestResultType.Ignored;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Ignored, testMethod1.Result);
		}
		
		[Test]
		public void TestClassResultAfterTestMethod1Failed()
		{
			TestMethod1Failed();
			
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}
		
		[Test]
		public void TestMethod1Passes()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod1");
			result.ResultType = TestResultType.Success;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Success, testMethod1.Result);
		}
		
		[Test]
		public void TestClassResultAfterTestMethod1Passes()
		{
			TestMethod1Passes();
			
			Assert.AreEqual(TestResultType.None, testClass.Result);
		}
		
		[Test]
		public void TestMethod2Passes()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod2");
			result.ResultType = TestResultType.Success;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Success, testMethod2.Result);
		}
		
		[Test]
		public void TestClassResultAfterTestMethod2Passes()
		{
			TestMethod2Passes();
			
			Assert.AreEqual(TestResultType.None, testClass.Result);
		}
		
		[Test]
		public void TestClassResultAfterBothMethodsPass()
		{
			TestMethod1Passes();
			TestMethod2Passes();
			
			Assert.AreEqual(TestResultType.Success, testClass.Result);
		}
		
		[Test]
		public void TestClassResultAfterOneIgnoredAndOnePassed()
		{
			TestMethod1Ignored();
			TestMethod2Passes();
			
			Assert.AreEqual(TestResultType.Ignored, testClass.Result);
		}
		
		[Test]
		public void TestClassResultAfterOneFailedAndOnePassed()
		{
			TestMethod1Failed();
			TestMethod2Passes();
			
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}
		
		[Test]
		public void FindTestMethod()
		{
			TestMember method = testProject.TestClasses.GetTestMember("RootNamespace.Tests.MyTestFixture.TestMethod1");
			Assert.AreSame(testMethod1, method);
		}
		
		[Test]
		public void FindTestMethodFromUnknownTestMethod()
		{
			Assert.IsNull(testProject.TestClasses.GetTestMember("RootNamespace.Tests.MyTestFixture.UnknownTestMethod"));
		}
		
		[Test]
		public void FindTestMethodFromUnknownTestClass()
		{
			Assert.IsNull(testProject.TestClasses.GetTestMember("RootNamespace.Tests.UnknownTestFixture.TestMethod1"));
		}
		
		[Test]
		public void FindTestMethodFromInvalidTestMethodName()
		{
			Assert.IsNull(testProject.TestClasses.GetTestMember(String.Empty));
		}
		
		/// <summary>
		/// SD2-1278. Tests that the method is updated in the TestClass. 
		/// This ensures that the method's location is up to date.
		/// </summary>
		[Test]
		public void TestMethodShouldBeUpdatedInClass()
		{
			MockMethod mockMethod = new MockMethod(mockClass, "TestMethod1");
			mockMethod.Attributes.Add(new MockAttribute("Test"));
			
			// Remove the existing TestMethod1 in the class.
			mockClass.Methods.RemoveAt(0);

			// Add our newly created test method object.
			mockClass.Methods.Insert(0, mockMethod);
			
			TestClass testClass = testProject.TestClasses["RootNamespace.Tests.MyTestFixture"];
			testClass.UpdateClass(mockClass);
			
			// Ensure that the TestClass now uses the new method object.
			TestMember method = testClass.GetTestMember("TestMethod1");
			Assert.AreSame(mockMethod, method.Member);
		}
	}
}
