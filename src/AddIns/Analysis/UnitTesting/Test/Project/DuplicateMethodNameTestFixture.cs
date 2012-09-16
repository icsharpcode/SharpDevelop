// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that the TestProject can handle a test class that
	/// has two test methods with the same name.
	/// </summary>
	[TestFixture]
	public class DuplicateMethodNameTestFixture : NUnitTestProjectFixtureBase
	{
		NUnitTestClass testClass;
		NUnitTestMethod baseMethod;
		NUnitTestMethod derivedMethod;
		
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFileInNamespace("base.cs", @"
abstract class MyTestFixtureBase {
	[Test] public void MyTest() {}
	[Test] public void MyTest() {}
}");
			AddCodeFileInNamespace("derived.cs", @"
class MyTestFixture : MyTestFixtureBase {
	[Test] public void MyTest() {}
	[Test] public void MyTest() {}
}");
			testClass = testProject.NestedTests.Cast<NUnitTestClass>().Single(c => c.ClassName == "MyTestFixture");
			baseMethod = testClass.FindTestMethod("MyTestFixtureBase.MyTest");
			derivedMethod = testClass.FindTestMethod("MyTest");
		}
		
		[Test]
		public void TwoTestMethods()
		{
			Assert.AreEqual(2, testClass.NestedTests.Count);
		}
		
		[Test]
		public void TestMethodNames()
		{
			Assert.AreEqual("MyTestFixtureBase.MyTest", baseMethod.MethodNameWithDeclaringTypeForInheritedTests);
			Assert.AreEqual("MyTest", derivedMethod.MethodNameWithDeclaringTypeForInheritedTests);
		}
		
		[Test]
		public void UpdateTestResultMethodInDerivedClass()
		{
			testProject.UpdateTestResult(new TestResult("RootNamespace.MyTestFixture.MyTest") { ResultType = TestResultType.Failure });
			Assert.AreEqual(TestResultType.None, baseMethod.Result);
			Assert.AreEqual(TestResultType.Failure, derivedMethod.Result);
		}
		
		[Test]
		public void UpdateTestResultMethodInBaseClass()
		{
			testProject.UpdateTestResult(new TestResult("RootNamespace.MyTestFixture.MyTestFixtureBase.MyTest") { ResultType = TestResultType.Failure });
			Assert.AreEqual(TestResultType.Failure, baseMethod.Result);
			Assert.AreEqual(TestResultType.None, derivedMethod.Result);
		}
	}
}
