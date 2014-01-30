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
