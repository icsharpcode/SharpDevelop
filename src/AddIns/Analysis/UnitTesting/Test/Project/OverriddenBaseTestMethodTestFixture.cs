// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
