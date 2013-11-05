// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests what happens when a test method is renamed inside an inner class.
	/// </summary>
	[TestFixture]
	public class InnerClassMethodRemovedTestFixture : NUnitTestProjectFixtureBase
	{
		NUnitTestClass innerTestClass;
		
		public override void SetUp()
		{
			base.SetUp();
			
			AddCodeFile("test.cs", @"
using NUnit.Framework;
namespace MyTests {
	class A {
		class InnerATest {
			[Test]
			public void FooBar() {}
		}
	}
}
");

			// The members should be changed on the existing TestClass instance,
			// so grab the reference in before updating.
			innerTestClass = testProject.GetTestClass(new FullTypeName("MyTests.A+InnerATest"));
			
			UpdateCodeFile("test.cs", @"
using NUnit.Framework;
namespace MyTests {
	class A {
		class InnerATest {
			[Test]
			public void FooBarRenamed() {}
			
			[TestFixture]
			class InnerInnerTest {}
		}
	}
}
");
		}
		
		[Test]
		public void NewTestMethodExists()
		{
			Assert.Contains("FooBarRenamed", GetTestMethodNames(innerTestClass));
		}
		
		[Test]
		public void OldTestMethodRemoved()
		{
			Assert.IsFalse(GetTestMethodNames(innerTestClass).Contains("FooBar"));
		}
		
		[Test]
		public void NewTestClassExists()
		{
			CollectionAssert.Contains(innerTestClass.NestedTests.OfType<NUnitTestClass>().Select(x => x.ReflectionName),
			                          "MyTests.A+InnerATest+InnerInnerTest");
		}
	}
}
