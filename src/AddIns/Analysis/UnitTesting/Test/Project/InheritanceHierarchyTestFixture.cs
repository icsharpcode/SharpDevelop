// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
