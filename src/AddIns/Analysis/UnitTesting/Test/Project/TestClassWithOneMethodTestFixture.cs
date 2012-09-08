// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class TestClassWithOneMethodTestFixture : ProjectTestFixtureBase
	{
		TestClass testClass;
		TestMember testMethod;
		bool resultChangedCalled;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void Init()
		{
			resultChangedCalled = false;
			CreateNUnitProject(Parse(@"
using NUnit.Framework;
namespace RootNamespace.Tests {
	[TestFixture]
	class MyTestFixture {
		[Test]
		public void TestMethod() { }
	}
}"));
			testClass = testProject.TestClasses.Single();
			testMethod = testClass.Members.Single();
		}
		
		[Test]
		public void TestMethodResult()
		{
			Assert.AreEqual(TestResultType.None, testMethod.Result);
		}
		
		[Test]
		public void TestFailed()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod");
			result.ResultType = TestResultType.Failure;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Failure, testMethod.Result);
		}
		
		[Test]
		public void TestClassFailed()
		{
			TestFailed();
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}
		
		[Test]
		public void TestClassIgnored()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod");
			result.ResultType = TestResultType.Ignored;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Ignored, testClass.Result);
		}
		
		[Test]
		public void TestResultChanged()
		{
			try {
				testMethod.ResultChanged += ResultChanged;
				TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod");
				result.ResultType = TestResultType.Failure;
				testProject.UpdateTestResult(result);
			} finally {
				testMethod.ResultChanged -= ResultChanged;
			}
			
			Assert.IsTrue(resultChangedCalled);
		}
		
		[Test]
		public void TestResultChangedOnClass()
		{
			try {
				testClass.ResultChanged += ResultChanged;
				TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod");
				result.ResultType = TestResultType.Failure;
				testProject.UpdateTestResult(result);
			} finally {
				testClass.ResultChanged -= ResultChanged;
			}
			
			Assert.IsTrue(resultChangedCalled);
		}
		
		[Test]
		public void UpdateProjectWithTestResultHavingClassNameOnly()
		{
			TestResult result = new TestResult("ClassNameOnly");
			result.ResultType = TestResultType.Failure;
			
			testProject.UpdateTestResult(result);
		}
		
		[Test]
		public void UpdateProjectWithTestResultWithUnknownClassName()
		{
			TestResult result = new TestResult("RootNamespace.Tests.UnknownClassName.TestMethod");
			result.ResultType = TestResultType.Failure;
			
			testProject.UpdateTestResult(result);
		}
		
		[Test]
		public void UpdateProjectWithTestResultWithUnknownMethodName()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.UnknownMethodName");
			result.ResultType = TestResultType.Failure;
			
			testProject.UpdateTestResult(result);
		}
		
		[Test]
		public void FindTestMethod()
		{
			Assert.AreSame(testMethod, testClass.Members["TestMethod"]);
		}
		
		[Test]
		public void AddNewClassNodeWhenTestClassPassed()
		{
			testClass.Result = TestResultType.Success;
			TestClassTreeNode node = new TestClassTreeNode(testProject, testClass);
			Assert.AreEqual(TestTreeViewImageListIndex.TestPassed, (TestTreeViewImageListIndex)node.ImageIndex);
		}
		
		[Test]
		public void AddNewMethodNodeWhenTestPassed()
		{
			testMethod.Result = TestResultType.Success;
			TestMemberTreeNode node = new TestMemberTreeNode(testProject, testMethod);
			Assert.AreEqual(TestTreeViewImageListIndex.TestPassed, (TestTreeViewImageListIndex)node.ImageIndex);
		}
		
		/// <summary>
		/// Tests that a method is removed from the TestClass
		/// based on the parse info.
		/// </summary>
		[Test]
		public void MethodRemovedInParserInfo()
		{
			UpdateCodeFile(@"
using NUnit.Framework;
namespace RootNamespace.Tests {
	[TestFixture]
	class MyTestFixture {
		public void TestMethod() { }
	}
}");
			
			Assert.AreEqual(0, testClass.Members.Count);
		}
		
		void ResultChanged(object source, EventArgs e)
		{
			resultChangedCalled = true;
		}
	}
}
