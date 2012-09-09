// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Creates a TestMethodCollection with three TestMethods.
	/// </summary>
	[TestFixture]
	public class ThreeTestMethodsTestResultsTestFixture : ProjectTestFixtureBase
	{
		TestClass testClass;
		TestMember testMethod1;
		TestMember testMethod2;
		TestMember testMethod3;
		bool testMethodsResultChanged;
		
		[SetUp]
		public void Init()
		{
			testMethodsResultChanged = false;
			
			testClass = new TestClass(MockClass.CreateMockClassWithoutAnyAttributes(), new NUnitTestFramework());
			testClass.Members.Add(new TestMember(MockMethod.CreateUnresolvedMethod("TestMethod1")));
			testClass.Members.Add(new TestMember(MockMethod.CreateUnresolvedMethod("TestMethod2")));
			testClass.Members.Add(new TestMember(MockMethod.CreateUnresolvedMethod("TestMethod3")));
			
			testClass.TestResultChanged += TestMethodsResultChanged;
		}
		
		[Test]
		public void InitialTestResult()
		{
			Assert.AreEqual(TestResultType.None, testClass.TestResult);
		}
		
		[Test]
		public void TestMethod1Fails()
		{
			testMethod1.TestResult = TestResultType.Failure;
			Assert.AreEqual(TestResultType.Failure, testClass.TestResult);
			Assert.IsTrue(testMethodsResultChanged);
		}
		
		[Test]
		public void ResetAfterTestMethod1Failed()
		{
			TestMethod1Fails();
			testClass.ResetTestResults();
			InitialTestResult();
			
			Assert.AreEqual(TestResultType.None, testMethod1.TestResult);
		}
		
		[Test]
		public void AllTestMethodsPass()
		{
			testMethod1.TestResult = TestResultType.Success;
			testMethod2.TestResult = TestResultType.Success;
			testMethod3.TestResult = TestResultType.Success;
			Assert.AreEqual(TestResultType.Success, testClass.TestResult);
			Assert.IsTrue(testMethodsResultChanged);
		}
		
		[Test]
		public void ResetAfterAllPassed()
		{
			AllTestMethodsPass();
			testClass.ResetTestResults();
			InitialTestResult();
			
			Assert.AreEqual(TestResultType.None, testMethod1.TestResult);
			Assert.IsTrue(testMethodsResultChanged);
		}
		
		[Test]
		public void AllTestMethodsIgnored()
		{
			testMethod1.TestResult = TestResultType.Ignored;
			testMethod2.TestResult = TestResultType.Ignored;
			testMethod3.TestResult = TestResultType.Ignored;
			Assert.AreEqual(TestResultType.Ignored, testClass.TestResult);
			Assert.IsTrue(testMethodsResultChanged);
		}
		
		[Test]
		public void ResetAfterAllTestMethodsIgnored()
		{
			AllTestMethodsIgnored();
			
			testMethodsResultChanged = false;
			testClass.ResetTestResults();
			
			InitialTestResult();
			
			Assert.AreEqual(TestResultType.None, testMethod1.TestResult);
			Assert.IsTrue(testMethodsResultChanged);
		}
		
		[Test]
		public void TestMethod1Removed()
		{
			testClass.Members.Remove(testMethod1);
			testMethod1.TestResult = TestResultType.Failure;
			
			InitialTestResult();
			
			Assert.IsFalse(testMethodsResultChanged);
		}
		
		[Test]
		public void TestMethodsResultSetToNoneAfterAllTestClasssIgnored()
		{
			AllTestMethodsIgnored();
			
			testMethodsResultChanged = false;
			testMethod1.TestResult = TestResultType.None;
			testMethod2.TestResult = TestResultType.None;
			testMethod3.TestResult = TestResultType.None;
			
			InitialTestResult();
			
			Assert.IsTrue(testMethodsResultChanged);
		}
		
		[Test]
		public void TestMethod1RemovedAfterSetToIgnored()
		{
			testMethod1.TestResult = TestResultType.Ignored;
			
			testClass.Members.Remove(testMethod1);
			InitialTestResult();
		}
		
		[Test]
		public void AddTestFailureAfterAllTestsPassed()
		{
			AllTestMethodsPass();
			
			testClass.Members.Add(new TestMember(MockMethod.CreateUnresolvedMethod("TestMethod4")));

			Assert.AreEqual(TestResultType.Failure, testClass.TestResult);
		}
		
		void TestMethodsResultChanged(object source, EventArgs e)
		{
			testMethodsResultChanged = true;
		}
	}
}
