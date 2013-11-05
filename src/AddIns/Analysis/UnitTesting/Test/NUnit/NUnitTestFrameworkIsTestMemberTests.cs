// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.NUnit
{
	[TestFixture]
	public class NUnitTestFrameworkIsTestMemberTests
	{
		IMethod CreateMethod(string code)
		{
			var c = NRefactoryHelper.CreateTypeDefinition("using NUnit.Framework; class C { " + code + " } ");
			return c.Methods.First();
		}
		
		[Test]
		public void IsTestMember_MethodHasNoAttributes_ReturnsFalse()
		{
			var m = CreateMethod("public void M() {}");
			Assert.IsFalse(NUnitTestFramework.IsTestMethod(m));
		}
		
		[Test]
		public void IsTestMember_MethodHasTestAttributeWithoutAttributePart_ReturnsTrue()
		{
			var m = CreateMethod("[Test] public void M() {}");
			Assert.IsTrue(NUnitTestFramework.IsTestMethod(m));
		}
		
		[Test]
		public void IsTestMember_MethodHasTestAttributeAttribute_ReturnsTrue()
		{
			var m = CreateMethod("[TestAttribute] public void M() {}");
			Assert.IsTrue(NUnitTestFramework.IsTestMethod(m));
		}

		[Test]
		public void IsTestMember_MethodHasFullyQualifiedNUnitTestAttribute_ReturnsTrue()
		{
			var m = CreateMethod("[NUnit.Framework.TestAttribute] public void M() {}");
			Assert.IsTrue(NUnitTestFramework.IsTestMethod(m));
		}
		
		[Test]
		public void IsTestMember_MethodIsNull_ReturnsFalse()
		{
			Assert.IsFalse(NUnitTestFramework.IsTestMethod(null));
		}
		
		[Test]
		public void IsTestMember_MethodHasParameters_ReturnsTrue()
		{
			// not actually executable, but NUnit still considers it a test case
			// and marks it as ignored
			var m = CreateMethod("[Test] public void M(int a) {}");
			Assert.IsTrue(NUnitTestFramework.IsTestMethod(m));
		}
		
		[Test]
		public void IsTestMember_MethodIsStatic_ReturnsTrue()
		{
			var m = CreateMethod("[Test] public static void M() {}");
			Assert.IsTrue(NUnitTestFramework.IsTestMethod(m));
		}
		
		[Test]
		public void TestCase()
		{
			var m = CreateMethod("[TestCase] public void M() {}");
			Assert.IsTrue(NUnitTestFramework.IsTestMethod(m));
		}
		
		[Test]
		public void TestCase_WithParameter()
		{
			var m = CreateMethod("[TestCase(1), TestCase(2)] public void M(int x) {}");
			Assert.IsTrue(NUnitTestFramework.IsTestMethod(m));
		}
	}
}
