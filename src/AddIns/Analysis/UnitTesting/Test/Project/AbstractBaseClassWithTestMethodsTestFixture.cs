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

using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using System.Collections.Generic;
using NUnit.Framework;
using System;
using Rhino.Mocks;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// The Unit Tests window was not updating the tree when a test was run if the test
	/// was in an abstract base class that did not use the [TestFixture] attribute.
	/// 
	/// This is the case with the CecilLayerTests class:
	/// 
	/// [TestFixture]
	/// public class CecilLayerTests : ReflectionOrCecilLayerTests
	/// { ... }
	/// 
	/// public abstract class ReflectionOrCecilLayerTests
	/// {
	///
	///     [Test]
	///     public void InheritanceTest()
	///     { ... }
	/// }
	/// 
	/// Note that the test result for the test method is written to the file with the name
	/// Namespace.CecilLayerTests.InheritanceTests but the unit tests window displays it with the
	/// base class name prefixed to it to be consistent with NUnit GUI.
	/// </summary>
	[TestFixture]
	public class AbstractBaseClassWithTestMethodsTestFixture : NUnitTestProjectFixtureBase
	{
		NUnitTestClass cecilLayer;
		List<string> testMembers;
		
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFileInNamespace("derived.cs", @"
[TestFixture]
public class CecilLayerTests : ReflectionOrCecilLayerTests
{ }");
			AddCodeFileInNamespace("base.cs", @"
public abstract class ReflectionOrCecilLayerTests {
	[Test]
	public void InheritanceTests() {}
	
	public void NonTestMethod() {}
}
");
			testProject.EnsureNestedTestsInitialized();
			cecilLayer = testProject.NestedTests.Cast<NUnitTestClass>().Single(c => c.ClassName == "CecilLayerTests");
			testMembers = cecilLayer.NestedTests.Cast<NUnitTestMethod>().Select(m => m.MethodNameWithDeclaringTypeForInheritedTests).ToList();
		}

		[Test]
		public void BaseMethodExists()
		{
			Assert.IsTrue(testMembers.Contains("ReflectionOrCecilLayerTests.InheritanceTests"));
		}

		[Test]
		public void NonTestBaseMethodDoesNotExist()
		{
			Assert.IsFalse(testMembers.Contains("ReflectionOrCecilLayerTests.NonTestMethod"));
			Assert.AreEqual(1, cecilLayer.NestedTests.Count);
		}
		
		[Test]
		public void BaseMethodFixtureReflectionNameIsDerivedClass()
		{
			var method = (NUnitTestMethod)cecilLayer.NestedTests.Single();
			Assert.AreEqual("RootNamespace.CecilLayerTests", method.FixtureReflectionName);
		}

		[Test]
		public void UpdateTestResult()
		{
			TestResult testResult = new TestResult("RootNamespace.CecilLayerTests.ReflectionOrCecilLayerTests.InheritanceTests");
			testResult.ResultType = TestResultType.Failure;
			testProject.UpdateTestResult(testResult);
			
			Assert.AreEqual(TestResultType.Failure, cecilLayer.NestedTests.Single().Result);
			Assert.AreEqual(TestResultType.Failure, cecilLayer.Result);
		}
		
		[Test]
		public void AddTestMethodToBaseClass()
		{
			Assert.AreEqual(1, cecilLayer.NestedTests.Count);
			
			UpdateCodeFileInNamespace("base.cs", @"
public abstract class ReflectionOrCecilLayerTests {
	[Test]
	public void InheritanceTests() {}
	
	[Test]
	public void NewTestMethod() {}
}
");
			Assert.AreEqual(2, cecilLayer.NestedTests.Count);
		}
	}
}
