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
