// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests what happens when a test method is renamed inside an inner class.
	/// </summary>
	[TestFixture]
	public class InnerClassMethodRemovedTestFixture : ProjectTestFixtureBase
	{
		TestClass innerTestClass;
		
		[SetUp]
		public void Init()
		{
			CreateNUnitProject(Parse(@"
using NUnit.Framework;
namespace MyTests {
	class A {
		class InnerATest {
			[Test]
			public void FooBar() {}
		}
	}
}
"));

			// The members should be changed on the existing TestClass instance,
			// so grab the reference in before updating.
			innerTestClass = testProject.GetTestClass("MyTests.A+InnerATest");
			
			UpdateCodeFile(@"
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
			TestMember method = innerTestClass.Members[0];
			Assert.AreEqual("FooBarRenamed", method.Name);
		}
		
		[Test]
		public void OldTestMethodRemoved()
		{
			Assert.AreEqual(1, innerTestClass.Members.Count);
		}
		
		[Test]
		public void NewTestClassExists() 
		{
			CollectionAssert.Contains(innerTestClass.NestedClasses.Select(x => x.QualifiedName).ToList(), "MyTests.A+InnerATest+InnerInnerTest");
		}
	}
}
