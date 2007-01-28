// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		TestMethod testMethod1;
		TestMethod testMethod2;
		MockClass mockClass;
		
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
			
			mockClass = new MockClass("RootNamespace.Tests.MyTestFixture");
			mockClass.Namespace = "RootNamespace.Tests";
			mockClass.ProjectContent = projectContent;
			mockClass.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(mockClass);
			
			// Add a method to the test class
			MockMethod mockMethod = new MockMethod("TestMethod1");
			mockMethod.DeclaringType = mockClass;
			mockMethod.Attributes.Add(new MockAttribute("Test"));
			mockClass.Methods.Add(mockMethod);
			
			mockMethod = new MockMethod("TestMethod2");
			mockMethod.DeclaringType = mockClass;
			mockMethod.Attributes.Add(new MockAttribute("Test"));
			mockClass.Methods.Add(mockMethod);
			
			testProject = new TestProject(project, projectContent);
			testClass = testProject.TestClasses[0];
			testMethod1 = testClass.TestMethods[0];
			testMethod2 = testClass.TestMethods[1];
		}
		
		[Test]
		public void TwoMethods()
		{
			Assert.AreEqual(2, testClass.TestMethods.Count);
		}
			
		[Test]
		public void TestMethod1Failed()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod1");
			result.IsFailure = true;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Failure, testMethod1.Result);
		}
		
		[Test]
		public void TestMethod1Ignored()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod1");
			result.IsIgnored = true;
			
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
			result.IsSuccess = true;
			
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
			result.IsSuccess = true;
			
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
			TestMethod method = testProject.TestClasses.GetTestMethod("RootNamespace.Tests.MyTestFixture.TestMethod1");
			Assert.AreSame(testMethod1, method);
		}
		
		[Test]
		public void FindTestMethodFromUnknownTestMethod()
		{
			Assert.IsNull(testProject.TestClasses.GetTestMethod("RootNamespace.Tests.MyTestFixture.UnknownTestMethod"));
		}
		
		[Test]
		public void FindTestMethodFromUnknownTestClass()
		{
			Assert.IsNull(testProject.TestClasses.GetTestMethod("RootNamespace.Tests.UnknownTestFixture.TestMethod1"));
		}
		
		[Test]
		public void FindTestMethodFromInvalidTestMethodName()
		{
			Assert.IsNull(testProject.TestClasses.GetTestMethod(String.Empty));
		}
		
		/// <summary>
		/// SD2-1278. Tests that the method is updated in the TestClass. 
		/// This ensures that the method's location is up to date.
		/// </summary>
		[Test]
		public void TestMethodShouldBeUpdatedInClass()
		{
			MockMethod mockMethod = new MockMethod("TestMethod1");
			mockMethod.DeclaringType = mockClass;
			mockMethod.Attributes.Add(new MockAttribute("Test"));
			mockClass.SetCompoundClass(mockClass);
			
			// Remove the existing TestMethod1 in the class.
			mockClass.Methods.RemoveAt(0);

			// Add our newly created test method object.
			mockClass.Methods.Insert(0, mockMethod);
			
			TestClass testClass = testProject.TestClasses["RootNamespace.Tests.MyTestFixture"];
			testClass.UpdateClass(mockClass);
			
			// Ensure that the TestClass now uses the new method object.
			TestMethod method = testClass.GetTestMethod("TestMethod1");
			Assert.AreSame(mockMethod, method.Method);
		}
	}
}
