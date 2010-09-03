// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Creates a TestClassCollection with three test classes.
	/// </summary>
	[TestFixture]
	public class ThreeTestClassesTestResultsTestFixture
	{
		TestClass testClass1;
		TestClass testClass2;
		TestClass testClass3;
		TestClassCollection testClasses;
		bool testClassesResultChanged;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void Init()
		{
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			
			testClassesResultChanged = false;
			testClasses = new TestClassCollection();
			
			// TestClass1.
			MockClass mockClass = new MockClass("TestClass1");
			testClass1 = new TestClass(mockClass, testFrameworks);
			testClasses.Add(testClass1);
			
			// TestClass2.
			mockClass = new MockClass("TestClass2");
			testClass2 = new TestClass(mockClass, testFrameworks);
			testClasses.Add(testClass2);
		
			// TestClass3.
			mockClass = new MockClass("TestClass3");
			testClass3 = new TestClass(mockClass, testFrameworks);
			testClasses.Add(testClass3);
			
			testClasses.ResultChanged += TestClassesResultChanged;
		}
		
		[Test]
		public void InitialTestResult()
		{
			Assert.AreEqual(TestResultType.None, testClasses.Result);
		}
		
		[Test]
		public void TestClass1Fails()
		{
			testClass1.Result = TestResultType.Failure;
			Assert.AreEqual(TestResultType.Failure, testClasses.Result);
			Assert.IsTrue(testClassesResultChanged);
		}
		
		[Test]
		public void ResetAfterTestClass1Failed()
		{
			TestClass1Fails();
			testClasses.ResetTestResults();
			InitialTestResult();
			
			Assert.AreEqual(TestResultType.None, testClass1.Result);
		}
		
		[Test]
		public void AllTestClassesPass()
		{
			testClass1.Result = TestResultType.Success;
			testClass2.Result = TestResultType.Success;
			testClass3.Result = TestResultType.Success;
			Assert.AreEqual(TestResultType.Success, testClasses.Result);
			Assert.IsTrue(testClassesResultChanged);
		}
		
		[Test]
		public void ResetAfterAllPassed()
		{
			AllTestClassesPass();
			testClasses.ResetTestResults();
			InitialTestResult();
			
			Assert.AreEqual(TestResultType.None, testClass1.Result);
			Assert.IsTrue(testClassesResultChanged);
		}
		
		[Test]
		public void AllTestClassesIgnored()
		{
			testClass1.Result = TestResultType.Ignored;
			testClass2.Result = TestResultType.Ignored;
			testClass3.Result = TestResultType.Ignored;
			Assert.AreEqual(TestResultType.Ignored, testClasses.Result);
			Assert.IsTrue(testClassesResultChanged);
		}
		
		[Test]
		public void ResetAfterAllTestClasssIgnored()
		{
			AllTestClassesIgnored();
			
			testClassesResultChanged = false;
			testClasses.ResetTestResults();
			
			InitialTestResult();
			
			Assert.AreEqual(TestResultType.None, testClass1.Result);
			Assert.IsTrue(testClassesResultChanged);
		}
		
		[Test]
		public void TestClass1Removed()
		{
			testClasses.Remove(testClass1);
			testClass1.Result = TestResultType.Failure;
			
			InitialTestResult();
			
			Assert.IsFalse(testClassesResultChanged);
		}
		
		[Test]
		public void TestClassesResultSetToNoneAfterAllTestClassesIgnored()
		{
			AllTestClassesIgnored();
			
			testClassesResultChanged = false;
			testClass1.Result = TestResultType.None;
			testClass2.Result = TestResultType.None;
			testClass3.Result = TestResultType.None;
			
			InitialTestResult();
			
			Assert.IsTrue(testClassesResultChanged);
		}
		
		[Test]
		public void AddTestFailureClassAfterAllTestsPassed()
		{
			AllTestClassesPass();
			
			MockClass mockClass = new MockClass("TestClass4");
			TestClass testClass4 = new TestClass(mockClass, testFrameworks);
			testClass4.Result = TestResultType.Failure;
			testClasses.Add(testClass4);

			Assert.AreEqual(TestResultType.Failure, testClasses.Result);
		}
		
		[Test]
		public void TestClass1RemovedAfterSetToIgnored()
		{
			testClass1.Result = TestResultType.Ignored;
			
			testClasses.Remove(testClass1);
			InitialTestResult();
		}
		
		void TestClassesResultChanged(object source, EventArgs e)
		{
			testClassesResultChanged = true;
		}
	}
}
