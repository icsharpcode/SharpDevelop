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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that a class with base type nested inside test fixture is recognized
	/// </summary>
	[TestFixture]
	public class DoubleNestedTestClassWithBaseType : NUnitTestProjectFixtureBase
	{
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFileInNamespace("test.cs", @"
public abstract class BaseClass {
	[Test]
	public void BaseFoo() {
	}
}

public class A
{
	public class InnerATest
	{
		[Test]
		public void FooBar()
		{
		}

		[TestFixture]
		public class InnerTestLevel2 : BaseClass {
		}
	}
}");
		}
		
		[Test]
		public void DoubleNestedClassShouldHaveTestMemberImportedFromBaseClass() 
		{
			var compilation = SD.ParserService.GetCompilation(project);
			var nestedClass = compilation.MainAssembly.GetTypeDefinition(new FullTypeName("RootNamespace.A+InnerATest+InnerTestLevel2"));
			var nestedTestClass = testProject.GetTestsForEntity(nestedClass).Single();
			var testMethod = (NUnitTestMethod)nestedTestClass.NestedTests.Single();
			Assert.AreEqual("BaseClass.BaseFoo", testMethod.MethodNameWithDeclaringTypeForInheritedTests);
		}
	}
}
