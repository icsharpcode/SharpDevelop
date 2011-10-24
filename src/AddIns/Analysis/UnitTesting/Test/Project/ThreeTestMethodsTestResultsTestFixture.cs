// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Creates a TestMethodCollection with three TestMethods.
	/// </summary>
	[TestFixture]
	public class ThreeTestMethodsTestResultsTestFixture
	{
		TestMember testMethod1;
		TestMember testMethod2;
		TestMember testMethod3;
		TestMemberCollection testMethods;
		bool testMethodsResultChanged;
		
		[SetUp]
		public void Init()
		{
			testMethodsResultChanged = false;
			testMethods = new TestMemberCollection();
			
			// TestMethod1.
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			MockMethod mockMethod = new MockMethod(c, "TestMethod1");
			testMethod1 = new TestMember(mockMethod);
			testMethods.Add(testMethod1);
			
			// TestMethod2.
			mockMethod = new MockMethod(c, "TestMethod2");
			testMethod2 = new TestMember(mockMethod);
			testMethods.Add(testMethod2);
		
			// TestMethod3.
			mockMethod = new MockMethod(c, "TestMethod3");
			testMethod3 = new TestMember(mockMethod);
			testMethods.Add(testMethod3);
			
			testMethods.ResultChanged += TestMethodsResultChanged;
		}
		
		[Test]
		public void InitialTestResult()
		{
			Assert.AreEqual(TestResultType.None, testMethods.Result);
		}
		
		[Test]
		public void TestMethod1Fails()
		{
			testMethod1.Result = TestResultType.Failure;
			Assert.AreEqual(TestResultType.Failure, testMethods.Result);
			Assert.IsTrue(testMethodsResultChanged);
		}
		
		[Test]
		public void ResetAfterTestMethod1Failed()
		{
			TestMethod1Fails();
			testMethods.ResetTestResults();
			InitialTestResult();
			
			Assert.AreEqual(TestResultType.None, testMethod1.Result);
		}
		
		[Test]
		public void AllTestMethodsPass()
		{
			testMethod1.Result = TestResultType.Success;
			testMethod2.Result = TestResultType.Success;
			testMethod3.Result = TestResultType.Success;
			Assert.AreEqual(TestResultType.Success, testMethods.Result);
			Assert.IsTrue(testMethodsResultChanged);
		}
		
		[Test]
		public void ResetAfterAllPassed()
		{
			AllTestMethodsPass();
			testMethods.ResetTestResults();
			InitialTestResult();
			
			Assert.AreEqual(TestResultType.None, testMethod1.Result);
			Assert.IsTrue(testMethodsResultChanged);
		}
		
		[Test]
		public void AllTestMethodsIgnored()
		{
			testMethod1.Result = TestResultType.Ignored;
			testMethod2.Result = TestResultType.Ignored;
			testMethod3.Result = TestResultType.Ignored;
			Assert.AreEqual(TestResultType.Ignored, testMethods.Result);
			Assert.IsTrue(testMethodsResultChanged);
		}
		
		[Test]
		public void ResetAfterAllTestMethodsIgnored()
		{
			AllTestMethodsIgnored();
			
			testMethodsResultChanged = false;
			testMethods.ResetTestResults();
			
			InitialTestResult();
			
			Assert.AreEqual(TestResultType.None, testMethod1.Result);
			Assert.IsTrue(testMethodsResultChanged);
		}
		
		[Test]
		public void TestMethod1Removed()
		{
			testMethods.Remove(testMethod1);
			testMethod1.Result = TestResultType.Failure;
			
			InitialTestResult();
			
			Assert.IsFalse(testMethodsResultChanged);
		}
		
		[Test]
		public void TestMethodsResultSetToNoneAfterAllTestClasssIgnored()
		{
			AllTestMethodsIgnored();
			
			testMethodsResultChanged = false;
			testMethod1.Result = TestResultType.None;
			testMethod2.Result = TestResultType.None;
			testMethod3.Result = TestResultType.None;
			
			InitialTestResult();
			
			Assert.IsTrue(testMethodsResultChanged);
		}
		
		[Test]
		public void TestMethod1RemovedAfterSetToIgnored()
		{
			testMethod1.Result = TestResultType.Ignored;
			
			testMethods.Remove(testMethod1);
			InitialTestResult();
		}
		
		[Test]
		public void AddTestFailureAfterAllTestsPassed()
		{
			AllTestMethodsPass();
			
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			MockMethod mockMethod = new MockMethod(c, "TestMethod4");
			TestMember testMethod4 = new TestMember(mockMethod);
			testMethod4.Result = TestResultType.Failure;
			testMethods.Add(testMethod4);

			Assert.AreEqual(TestResultType.Failure, testMethods.Result);
		}
		
		void TestMethodsResultChanged(object source, EventArgs e)
		{
			testMethodsResultChanged = true;
		}
	}
}
