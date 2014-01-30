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
