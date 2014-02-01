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

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// If a test fixture's base class has test attributes then
	/// NUnit includes the base class test methods by prefixing
	/// the base class name to them. In NUnit 2.2 this was true for
	/// both the GUI and the test result information. In
	/// NUnit 2.4 the GUI prefixes the base class to the method
	/// name but the test result does not prefix the base class
	/// to the name of the method.
	/// 
	/// [TestFixture]
	/// public DerivedClass : BaseClass
	/// 
	/// Test method is shown in UI as:
	/// 
	/// BaseClass.BaseClassMethod
	/// 
	/// Test method name returned by NUnit-Console:
	/// 
	/// RootNamespace.DerivedClass.BaseClassMethod
	/// </summary>
	[TestFixture]
	public class TestMethodsInBaseClassTestFixture : NUnitTestProjectFixtureBase
	{
		NUnitTestClass testClass;
		
		public override void SetUp()
		{
			base.SetUp();
			
			AddCodeFileInNamespace("base.cs", @"
[TestFixture] class TestFixtureBase {
	[Test] public void BaseMethod() {}
}");
			AddCodeFileInNamespace("derived.cs", @"
[TestFixture] class MyTestFixture : TestFixtureBase {
	[Test] public void DerivedMethod() {}
}");
			testClass = testProject.GetTestClass(new FullTypeName("RootNamespace.MyTestFixture"));
			testClass.EnsureNestedTestsInitialized();
		}
		
		[Test]
		public void TwoTestMethods()
		{
			Assert.AreEqual(2, testClass.NestedTests.Count);
		}
		
		[Test]
		public void DerivedMethod()
		{
			Assert.IsNotNull(testClass.FindTestMethod("DerivedMethod"));
		}
		
		[Test]
		public void BaseMethod()
		{
			Assert.IsNotNull(testClass.FindTestMethod("TestFixtureBase.BaseMethod"));
		}
		
		[Test]
		public void FindTestMethodDoesNotReturnBaseMethodIfUsingShortName()
		{
			Assert.IsNull(testClass.FindTestMethod("BaseMethod"));
		}
		
		[Test]
		public void FindTestMethodWithShortNameCanFindBaseMethod()
		{
			Assert.IsNotNull(testClass.FindTestMethodWithShortName("BaseMethod"));
		}
		
		[Test]
		public void UpdateTestResultUsingPrefixBaseClassName()
		{
			TestResult testResult = new TestResult("RootNamespace.MyTestFixture.TestFixtureBase.BaseMethod");
			testResult.ResultType = TestResultType.Failure;
			testProject.UpdateTestResult(testResult);
			
			Assert.AreEqual(TestResultType.Failure, testClass.Result);
		}
	}
}
