// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
