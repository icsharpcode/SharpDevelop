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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class TestClassWithTwoMethodsTestFixture : NUnitTestProjectFixtureBase
	{
		NUnitTestClass testClass;
		NUnitTestMethod testMethod1;
		NUnitTestMethod testMethod2;
		
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFile("test.cs", @"using NUnit.Framework;
namespace RootNamespace.Tests {
	[TestFixture]
	class MyTestFixture {
		[Test] public void TestMethod1() {}
		[Test] public void TestMethod2() {}
	}
}");
			testClass = testProject.GetTestClass(new FullTypeName("RootNamespace.Tests.MyTestFixture"));
			testMethod1 = testClass.FindTestMethod("TestMethod1");
			testMethod2 = testClass.FindTestMethod("TestMethod2");
		}
		
		[Test]
		public void TwoMethods()
		{
			Assert.AreEqual(2, testClass.NestedTests.Count);
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
		
		/*[Test]
		public void FindTestMethod()
		{
			TestMember method = testProject.GetTestMember("RootNamespace.Tests.MyTestFixture.TestMethod1");
			Assert.AreSame(testMethod1, method);
		}
		
		[Test]
		public void FindTestMethodFromUnknownTestMethod()
		{
			Assert.IsNull(testProject.GetTestMember("RootNamespace.Tests.MyTestFixture.UnknownTestMethod"));
		}
		
		[Test]
		public void FindTestMethodFromUnknownTestClass()
		{
			Assert.IsNull(testProject.GetTestMember("RootNamespace.Tests.UnknownTestFixture.TestMethod1"));
		}
		
		[Test]
		public void FindTestMethodFromInvalidTestMethodName()
		{
			Assert.IsNull(testProject.GetTestMember(String.Empty));
		}*/
		
		/// <summary>
		/// SD2-1278. Tests that the method is updated in the TestClass.
		/// This ensures that the method's location is up to date.
		/// </summary>
		[Test]
		public void TestMethodShouldBeUpdatedInClass()
		{
			Assert.AreEqual(5, testMethod1.Region.BeginLine);
			
			UpdateCodeFile("test.cs", @"using NUnit.Framework;
namespace RootNamespace.Tests {
	[TestFixture]
	class MyTestFixture {
		// New line
		[Test] public void TestMethod1() {}
		[Test] public void TestMethod2() {}
	}
}");
			
			Assert.AreSame(testClass, testProject.GetTestClass(new FullTypeName("RootNamespace.Tests.MyTestFixture")));
			Assert.AreSame(testMethod1, testClass.NestedTests.Single(m => m.DisplayName == "TestMethod1"));
			Assert.AreEqual(6, testMethod1.Region.BeginLine);
		}
	}
}
