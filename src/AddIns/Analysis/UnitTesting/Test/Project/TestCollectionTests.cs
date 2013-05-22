// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Creates a TestClassCollection with three test classes.
	/// </summary>
	[TestFixture]
	public class TestCollectionTests
	{
		class FakeTest : TestBase
		{
			public override ITestProject ParentProject {
				get { throw new NotImplementedException(); }
			}
			
			public override string DisplayName {
				get { throw new NotImplementedException(); }
			}
			
			public new TestResultType Result {
				get { return base.Result; }
				// expose the setter
				set { base.Result = value; }
			}
		}
		
		FakeTest testClass1;
		FakeTest testClass2;
		FakeTest testClass3;
		TestCollection testCollection;
		bool testClassesResultChanged;
		
		[SetUp]
		public void Init()
		{
			testClassesResultChanged = false;
			testCollection = new TestCollection();
			
			// TestClass1.
			testClass1 = new FakeTest();
			testCollection.Add(testClass1);
			
			// TestClass2.
			testClass2 = new FakeTest();
			testCollection.Add(testClass2);
			
			// TestClass3.
			testClass3 = new FakeTest();
			testCollection.Add(testClass3);
			
			testCollection.PropertyChanged += testCollection_PropertyChanged;
		}
		
		[Test]
		public void InitialTestResult()
		{
			Assert.AreEqual(TestResultType.None, testCollection.CompositeResult);
		}
		
		[Test]
		public void TestClass1Fails()
		{
			testClass1.Result = TestResultType.Failure;
			Assert.AreEqual(TestResultType.Failure, testCollection.CompositeResult);
			Assert.IsTrue(testClassesResultChanged);
		}
		
		[Test]
		public void ResetAfterTestClass1Failed()
		{
			TestClass1Fails();
			foreach (var test in testCollection)
				test.ResetTestResults();
			InitialTestResult();
			
			Assert.AreEqual(TestResultType.None, testClass1.Result);
		}
		
		[Test]
		public void AllTestClassesPass()
		{
			testClass1.Result = TestResultType.Success;
			testClass2.Result = TestResultType.Success;
			testClass3.Result = TestResultType.Success;
			Assert.AreEqual(TestResultType.Success, testCollection.CompositeResult);
			Assert.IsTrue(testClassesResultChanged);
		}
		
		[Test]
		public void ResetAfterAllPassed()
		{
			AllTestClassesPass();
				foreach (var test in testCollection)
				test.ResetTestResults();
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
			Assert.AreEqual(TestResultType.Ignored, testCollection.CompositeResult);
			Assert.IsTrue(testClassesResultChanged);
		}
		
		[Test]
		public void ResetAfterAllTestClasssIgnored()
		{
			AllTestClassesIgnored();
			
			testClassesResultChanged = false;
				foreach (var test in testCollection)
				test.ResetTestResults();
			
			InitialTestResult();
			
			Assert.AreEqual(TestResultType.None, testClass1.Result);
			Assert.IsTrue(testClassesResultChanged);
		}
		
		[Test]
		public void TestClass1Removed()
		{
			testCollection.Remove(testClass1);
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
			
			FakeTest testClass4 = new FakeTest();
			testClass4.Result = TestResultType.Failure;
			testCollection.Add(testClass4);

			Assert.AreEqual(TestResultType.Failure, testCollection.CompositeResult);
		}
		
		[Test]
		public void TestClass1RemovedAfterSetToIgnored()
		{
			testClass1.Result = TestResultType.Ignored;
			
			testCollection.Remove(testClass1);
			InitialTestResult();
		}
		
		void testCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CompositeResult") {
				testClassesResultChanged = true;
			}
		}
	}
}
