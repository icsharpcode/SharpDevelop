// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;
using System.Linq;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that a class nested inside test fixture is recognized
	/// </summary>
	[TestFixture]
	public class InnerClassInTestFixtureTests : NUnitTestProjectFixtureBase
	{
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFile("test.cs", @"using NUnit.Framework;
namespace MyTests {
	[TestFixture]
	public class A
	{
		public class InnerATest
		{
			[Test]
			public void FooBar()
			{
			}
	
			[TestFixture]
			public class InnerTestLevel2 {
			}
		}
		
		public class InnerBClass {}
	}
}");
		}
		
		[Test]
		public void OuterTestClassFound()
		{
			Assert.IsNotNull(testProject.GetTestClass(new FullTypeName("MyTests.A")));
		}

		[Test]
		public void InnerNonTestClassWithoutWasNotMarkedAsTestClass()
		{
			Assert.IsNull(testProject.GetTestClass(new FullTypeName("MyTests.A+InnerBClass")));
		}
		
		[Test]
		public void InnerClassInInnerClassFound()
		{
			Assert.IsNotNull(testProject.GetTestClass(new FullTypeName("MyTests.A+InnerATest+InnerTestLevel2")));
		}
		
		[Test]
		public void TestClassNameShouldBeDotNetNameOfTheDoubleNestedClass()
		{
			Assert.AreEqual("MyTests.A+InnerATest+InnerTestLevel2", testProject.GetTestClass(new FullTypeName("MyTests.A+InnerATest+InnerTestLevel2")).ReflectionName);
		}
	}
}
