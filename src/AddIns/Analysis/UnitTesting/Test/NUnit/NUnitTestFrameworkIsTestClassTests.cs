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
	public class NUnitTestFrameworkIsTestClassTests
	{
		NUnitTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new NUnitTestFramework();
		}
		
		[Test]
		public void IsTestClassReturnsFalseHasClassHasNoAttributes()
		{
			var c = NRefactoryHelper.CreateTypeDefinition("class EmptyClass {}");
			Assert.IsFalse(NUnitTestFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueHasClassHasTestFixtureAttributeMissingAttributePart()
		{
			var c = NRefactoryHelper.CreateTypeDefinition("using NUnit.Framework; [TestFixture] class EmptyClass {}");
			Assert.IsTrue(NUnitTestFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenClassHasTestFixtureAttributeAndIsAbstract() {
			var c = NRefactoryHelper.CreateTypeDefinition("using NUnit.Framework; [TestFixture] abstract class EmptyClass {}");
			Assert.IsFalse(NUnitTestFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueHasClassHasTestFixtureAttribute()
		{
			var c = NRefactoryHelper.CreateTypeDefinition("using NUnit.Framework; [TestFixtureAttribute] class EmptyClass {}");
			Assert.IsTrue(NUnitTestFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueHasClassHasFullyQualifiedNUnitTestFixtureAttribute()
		{
			var c = NRefactoryHelper.CreateTypeDefinition("[NUnit.Framework.TestFixtureAttribute] class EmptyClass {}");
			Assert.IsTrue(NUnitTestFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenClassIsNull()
		{
			Assert.IsFalse(NUnitTestFramework.IsTestClass(null));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenClassHasTestAttributeWithoutUsingNUnit()
		{
			var c = NRefactoryHelper.CreateTypeDefinition("class C { [Test] public void M() {} }");
			Assert.IsFalse(NUnitTestFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueWhenContainingTestMethod()
		{
			var c = NRefactoryHelper.CreateTypeDefinition("using NUnit.Framework; class C { [Test] public void M() {} }");
			Assert.IsTrue(NUnitTestFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueWhenContainingTestCase()
		{
			var c = NRefactoryHelper.CreateTypeDefinition("using NUnit.Framework; class C { [TestCase(1)] public void M(int i) {} }");
			Assert.IsTrue(NUnitTestFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenAbstractClassContainsTestMethod()
		{
			var c = NRefactoryHelper.CreateTypeDefinition("using NUnit.Framework; abstract class C { [Test] public void M() {} }");
			Assert.IsFalse(NUnitTestFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueWhenContainingInnerClassWithTestMethod()
		{
			var c = NRefactoryHelper.CreateTypeDefinition("using NUnit.Framework; class C { class Inner { [Test] public void M() {} } }");
			Assert.IsTrue(NUnitTestFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueWhenAbstractClassContainsInnerClassWithTestMethod()
		{
			var c = NRefactoryHelper.CreateTypeDefinition("using NUnit.Framework; abstract class C { class Inner { [Test] public void M() {} } }");
			Assert.IsTrue(NUnitTestFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueWhenStaticClassWithTestMethod()
		{
			var c = NRefactoryHelper.CreateTypeDefinition("using NUnit.Framework; static class C { [Test] public static void M() {} }");
			Assert.IsTrue(NUnitTestFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueWhenStaticClassWithAttribute()
		{
			var c = NRefactoryHelper.CreateTypeDefinition("using NUnit.Framework; [TestFixture] static class C { }");
			Assert.IsTrue(NUnitTestFramework.IsTestClass(c));
		}
	}
}
