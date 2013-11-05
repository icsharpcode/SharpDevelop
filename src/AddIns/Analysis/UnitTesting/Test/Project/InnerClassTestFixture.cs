// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
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
	public class InnerClassTestFixture : NUnitTestProjectFixtureBase
	{
		NUnitTestClass outerClass;
		NUnitTestClass innerClass;
		
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFile("test.cs", @"
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
}");
			outerClass = testProject.GetTestClass(new FullTypeName("MyTests.A"));
			innerClass = testProject.GetTestClass(new FullTypeName("MyTests.A+InnerATest"));
		}
		
		[Test]
		public void OneTestClassFound()
		{
			Assert.AreEqual(1, testProject.NestedTests.Count);
		}
		
		[Test]
		public void TestClassQualifiedName()
		{
			Assert.AreEqual("MyTests.A+InnerATest", innerClass.ReflectionName);
		}
		
		[Test]
		public void TestClassName()
		{
			Assert.AreEqual("InnerATest", innerClass.ClassName);
		}
		
		[Test]
		public void NamespaceForInnerClassIsDeclaringTypesNamespace()
		{
			Assert.AreEqual("MyTests", innerClass.FullTypeName.TopLevelTypeName.Namespace);
		}
	}
}
