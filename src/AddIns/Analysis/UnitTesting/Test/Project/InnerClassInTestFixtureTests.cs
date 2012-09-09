// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
	public class InnerClassInTestFixtureTests : ProjectTestFixtureBase
	{
		TestClass outerTestClass;
		
		[SetUp]
		public void Init()
		{
			CreateNUnitProject(
				Parse(@"using NUnit.Framework;
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
}
"));
		}
		
		[Test]
		public void OuterTestClassFound()
		{
			Assert.IsNotNull(testProject.GetTestClass("A"));
		}

		[Test]
		public void InnerNonTestClassWithoutWasNotMarkedAsTestClass()
		{
			Assert.IsNull(testProject.GetTestClass("A+InnerBClass"));
		}
		
		[Test]
		public void InnerClassInInnerClassFound()
		{
			Assert.IsNotNull(testProject.GetTestClass("A+InnerATest+InnerTestLevel2"));
		}
		
		[Test]
		public void TestClassNameShouldBeDotNetNameOfTheDoubleNestedClass()
		{
			Assert.AreEqual("MyTests.A+InnerATest+InnerTestLevel2", testProject.GetTestClass("A+InnerATest+InnerTestLevel2").QualifiedName);
		}
		
		[Test]
		public void TestClassNamespaceShouldBeValid()
		{
			Assert.AreEqual("MyTests", testProject.GetTestClass("A+InnerATest+InnerTestLevel2").Namespace);
		}
	}
}
