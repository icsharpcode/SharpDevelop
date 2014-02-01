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
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class InheritanceHierarchyTestFixture : NUnitTestProjectFixtureBase
	{
		NUnitTestClass derived;
		
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFileInNamespace("base1.cs", "class Base1 { [Test] public void Base1Test() {} }");
			AddCodeFileInNamespace("base2.cs", "class Base2 { [Test] public void Base2Test() {} }");
			AddCodeFileInNamespace("middle.cs", "class Middle : Base1 { }");
			AddCodeFileInNamespace("derived.cs", "[TestFixture] class Derived : Middle { }");
			testProject.EnsureNestedTestsInitialized();
			derived = testProject.NestedTests.OfType<NUnitTestClass>().Single(c => c.ClassName == "Derived");
			derived.EnsureNestedTestsInitialized();
		}
		
		[Test]
		public void Base1TestExistsInDerivedClass()
		{
			Assert.AreEqual(new[] { "Base1.Base1Test" }, GetTestMethodNames(derived));
		}
		
		[Test]
		public void AddMethodToBase1()
		{
			UpdateCodeFileInNamespace("base1.cs", "class Base1 {" +
			                          "[Test] public void Base1Test() {}" +
			                          "[Test] public void NewTest() {} }");
			Assert.AreEqual(new[] { "Base1.Base1Test", "Base1.NewTest" }, GetTestMethodNames(derived));
		}
		
		[Test]
		public void ChangeBaseTypeOfMiddle()
		{
			UpdateCodeFileInNamespace("middle.cs", "class Middle : Base2 { }");
			Assert.AreEqual(new[] { "Base2.Base2Test" }, GetTestMethodNames(derived));
		}
		
		[Test]
		public void CauseCyclicInheritance()
		{
			UpdateCodeFileInNamespace("middle.cs", "class Middle : Derived { }");
			Assert.IsEmpty(GetTestMethodNames(derived));
		}
		
		[Test]
		public void ChangeBaseTypeOfMiddleToNonExistingClass()
		{
			UpdateCodeFileInNamespace("middle.cs", "class Middle : MissingClass { }");
			Assert.IsEmpty(GetTestMethodNames(derived));
		}
	}
}
