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
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class OverriddenBaseTestMethodTestFixture : NUnitTestProjectFixtureBase
	{
		NUnitTestClass baseClass;
		NUnitTestClass derivedClass;
		List<string> derivedClassMethodNames;
		
		public override void SetUp()
		{
			base.SetUp();
			
			AddCodeFileInNamespace("base.cs", @"
class BaseClass {
	[Test] public virtual void VirtualTestMethod() {}
	[Test] public virtual void VirtualNonOverriddenTestMethod() {}
}");
			AddCodeFileInNamespace("derived.cs", @"
class DerivedClass : BaseClass {
	[Test] public override void VirtualTestMethod() {}
}");
			baseClass = testProject.NestedTests.Cast<NUnitTestClass>().Single(c => c.ClassName == "BaseClass");
			derivedClass = testProject.NestedTests.Cast<NUnitTestClass>().Single(c => c.ClassName == "DerivedClass");
			derivedClassMethodNames = derivedClass.NestedTests.Cast<NUnitTestMethod>().Select(m => m.MethodNameWithDeclaringTypeForInheritedTests).ToList();
		}

		[Test]
		public void NonOverriddenVirtualBaseMethodExists()
		{
			Assert.IsTrue(derivedClassMethodNames.Contains("BaseClass.VirtualNonOverriddenTestMethod"));
		}

		[Test]
		public void VirtualBaseMethodDoesNotExistSinceItIsOverriddenInDerivedClass()
		{
			Assert.IsFalse(derivedClassMethodNames.Contains("BaseClass.VirtualTestMethod"));
		}

		[Test]
		public void DerivedClassTestMethodExists()
		{
			Assert.IsTrue(derivedClassMethodNames.Contains("VirtualTestMethod"));
		}
	}
}
