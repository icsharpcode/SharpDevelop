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
		
		[SetUp]
		public void Init()
		{
			MSBuildProject project = new MSBuildProject();
			project.Name = "TestProject";
			ReferenceProjectItem nunitFrameworkReferenceItem = new ReferenceProjectItem(project);
			nunitFrameworkReferenceItem.Include = "NUnit.Framework";
			project.Items.Add(nunitFrameworkReferenceItem);

			MockProjectContent projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			
			MockClass mockClass = new MockClass("RootNamespace.Tests.MyTestFixture");
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
	}
}
