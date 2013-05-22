// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
