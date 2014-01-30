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
	public class TestClassWithOneMethodTestFixture : NUnitTestProjectFixtureBase
	{
		NUnitTestClass testClass;
		NUnitTestMethod testMethod;
		bool resultChangedCalled;
		
		public override void SetUp()
		{
			base.SetUp();
			resultChangedCalled = false;
			AddCodeFile("test.cs", @"
using NUnit.Framework;
namespace RootNamespace.Tests {
	[TestFixture]
	class MyTestFixture {
		[Test]
		public void TestMethod() { }
	}
}");
			testClass = (NUnitTestClass)testProject.NestedTests.Single().NestedTests.Single();
			testMethod = (NUnitTestMethod)testClass.NestedTests.Single();
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
			Assert.AreSame(testMethod, testClass.NestedTests.Single(m => m.DisplayName == "TestMethod"));
		}
		
		/*
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
		*/
		
		/// <summary>
		/// Tests that a method is removed from the TestClass
		/// based on the parse info.
		/// </summary>
		[Test]
		public void MethodRemovedInParserInfo()
		{
			UpdateCodeFile("test.cs", @"
using NUnit.Framework;
namespace RootNamespace.Tests {
	[TestFixture]
	class MyTestFixture {
		public void TestMethod() { }
	}
}");
			
			Assert.AreEqual(0, testClass.NestedTests.Count);
		}
		
		void ResultChanged(object source, EventArgs e)
		{
			resultChangedCalled = true;
		}
	}
}
