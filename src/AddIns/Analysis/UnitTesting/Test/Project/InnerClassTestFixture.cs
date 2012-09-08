// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that a class with an inner test fixture is recognised, for example:
	/// 
	/// public class A
	/// {
	/// 	[TestFixture]
	/// 	public class InnerATest
	/// 	{
	/// 		[Test]
	/// 		public void FooBar()
	/// 		{
	/// 		}
	/// 	}
	/// }
	/// 
	/// In this case the FooBar test is identified via: "A+InnerATest.FooBar".
	/// </summary>
	[TestFixture]
	public class InnerClassTestFixture : ProjectTestFixtureBase
	{
		TestClass outerClass;
		TestClass innerClass;
		
		[SetUp]
		public void Init()
		{
			CreateNUnitProject(Parse(@"
using NUnit.Framework;
namespace MyTests {
	public class A
	{
		public class InnerATest
		{
			[Test]
			public void FooBar()
			{
			}
		}
	}
}"));
			outerClass = testProject.GetTestClass("A");
			innerClass = testProject.GetTestClass("A+InnerATest");
		}
		
		[Test]
		public void OneTestClassFound()
		{
			Assert.AreEqual(1, testProject.TestClasses.Count);
		}
		
		[Test]
		public void TestClassQualifiedName()
		{
			Assert.AreEqual("MyTests.A+InnerATest", innerClass.QualifiedName);
		}
		
		[Test]
		public void TestClassName()
		{
			Assert.AreEqual("InnerATest", innerClass.Name);
		}
		
		[Test]
		public void NamespaceForInnerClassIsDeclaringTypesNamespace()
		{
			Assert.AreEqual("MyTests", innerClass.Namespace);
		}
	}
}
