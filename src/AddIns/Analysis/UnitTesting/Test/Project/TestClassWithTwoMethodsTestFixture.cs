// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class TestClassWithTwoMethodsTestFixture : ProjectTestFixtureBase
	{
		TestClass testClass;
		TestMember testMethod1;
		TestMember testMethod2;
		
		[SetUp]
		public void Init()
		{
			CreateNUnitProject(Parse(@"using NUnit.Framework;
namespace RootNamespace.Tests {
	[TestFixture]
	class MyTestFixture {
		[Test] public void TestMethod1() {}
		[Test] public void TestMethod2() {}
	}
}"));
			testClass = testProject.TestClasses.Single();
			testMethod1 = testClass.Members[0];
			testMethod2 = testClass.Members[1];
		}
		
		[Test]
		public void TwoMethods()
		{
			Assert.AreEqual(2, testClass.Members.Count);
		}
		
		[Test]
		public void TestMethod1Failed()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod1");
			result.ResultType = TestResultType.Failure;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Failure, testMethod1.TestResult);
		}
		
		[Test]
		public void TestMethod1Ignored()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod1");
			result.ResultType = TestResultType.Ignored;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Ignored, testMethod1.TestResult);
		}
		
		[Test]
		public void TestClassResultAfterTestMethod1Failed()
		{
			TestMethod1Failed();
			
			Assert.AreEqual(TestResultType.Failure, testClass.TestResult);
		}
		
		[Test]
		public void TestMethod1Passes()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod1");
			result.ResultType = TestResultType.Success;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Success, testMethod1.TestResult);
		}
		
		[Test]
		public void TestClassResultAfterTestMethod1Passes()
		{
			TestMethod1Passes();
			
			Assert.AreEqual(TestResultType.None, testClass.TestResult);
		}
		
		[Test]
		public void TestMethod2Passes()
		{
			TestResult result = new TestResult("RootNamespace.Tests.MyTestFixture.TestMethod2");
			result.ResultType = TestResultType.Success;
			
			testProject.UpdateTestResult(result);
			
			Assert.AreEqual(TestResultType.Success, testMethod2.TestResult);
		}
		
		[Test]
		public void TestClassResultAfterTestMethod2Passes()
		{
			TestMethod2Passes();
			
			Assert.AreEqual(TestResultType.None, testClass.TestResult);
		}
		
		[Test]
		public void TestClassResultAfterBothMethodsPass()
		{
			TestMethod1Passes();
			TestMethod2Passes();
			
			Assert.AreEqual(TestResultType.Success, testClass.TestResult);
		}
		
		[Test]
		public void TestClassResultAfterOneIgnoredAndOnePassed()
		{
			TestMethod1Ignored();
			TestMethod2Passes();
			
			Assert.AreEqual(TestResultType.Ignored, testClass.TestResult);
		}
		
		[Test]
		public void TestClassResultAfterOneFailedAndOnePassed()
		{
			TestMethod1Failed();
			TestMethod2Passes();
			
			Assert.AreEqual(TestResultType.Failure, testClass.TestResult);
		}
		
		[Test]
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
		}
		
		/// <summary>
		/// SD2-1278. Tests that the method is updated in the TestClass.
		/// This ensures that the method's location is up to date.
		/// </summary>
		[Test]
		public void TestMethodShouldBeUpdatedInClass()
		{
			Assert.AreEqual(5, testMethod1.Member.Region.BeginLine);
			
			UpdateCodeFile(@"using NUnit.Framework;
namespace RootNamespace.Tests {
	[TestFixture]
	class MyTestFixture {
		// New line
		[Test] public void TestMethod1() {}
		[Test] public void TestMethod2() {}
	}
}");
			
			Assert.AreSame(testClass, testProject.TestClasses.Single());
			Assert.AreSame(testMethod1, testClass.Members[0]);
			Assert.AreEqual(6, testMethod1.Member.Region.BeginLine);
		}
	}
}
