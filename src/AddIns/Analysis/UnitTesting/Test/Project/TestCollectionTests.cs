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
