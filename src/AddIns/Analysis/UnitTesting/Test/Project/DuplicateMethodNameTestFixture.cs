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
