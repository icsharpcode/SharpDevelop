// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		TestMethod testMethod1;
		TestMethod testMethod2;
		TestMethod testMethod3;
		TestMethodCollection testMethods;
		bool testMethodsResultChanged;
		
		[SetUp]
		public void Init()
		{
			testMethodsResultChanged = false;
			testMethods = new TestMethodCollection();
			
			// TestMethod1.
			MockMethod mockMethod = new MockMethod("TestMethod1");
			testMethod1 = new TestMethod(mockMethod);
			testMethods.Add(testMethod1);
			
			// TestMethod2.
			mockMethod = new MockMethod("TestMethod2");
			testMethod2 = new TestMethod(mockMethod);
			testMethods.Add(testMethod2);
		
			// TestMethod3.
			mockMethod = new MockMethod("TestMethod3");
			testMethod3 = new TestMethod(mockMethod);
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
			
			MockMethod mockMethod = new MockMethod("TestMethod4");
			TestMethod testMethod4 = new TestMethod(mockMethod);
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
