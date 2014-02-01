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

using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class TwoRootNamespacesTestFixture : NUnitTestProjectFixtureBase
	{
		public override void SetUp()
		{
			base.SetUp();
			
			AddCodeFile("ns1.cs", @"using NUnit.Framework;
namespace RootNamespace {
	[TestFixture] class MyTestFixture1 {}
}");
			AddCodeFile("ns2.cs", @"using NUnit.Framework;
namespace RootNamespace2 {
	[TestFixture] class MyTestFixture2 {}
}");
			testProject.EnsureNestedTestsInitialized();
		}
		
		[Test]
		public void TwoRootNamespaces()
		{
			Assert.AreEqual(2, testProject.NestedTests.Count);
		}
		
		[Test]
		public void RootNamespace1()
		{
			// first root namespace is the project root namespace, so the
			// class is added directly
			var c = (NUnitTestClass)testProject.NestedTests.First();
			Assert.AreEqual("MyTestFixture1", c.ClassName);
		}
		
		[Test]
		public void RootNamespace2()
		{
			var ns = (TestNamespace)testProject.NestedTests.ElementAt(1);
			Assert.AreEqual("RootNamespace2", ns.NamespaceName);
		}
		
		[Test]
		public void OneClassInNamespace2()
		{
			var ns = (TestNamespace)testProject.NestedTests.ElementAt(1);
			Assert.AreEqual(1, ns.NestedTests.Count);
		}
		
		[Test]
		public void ClassNameInNamespace2()
		{
			var ns = (TestNamespace)testProject.NestedTests.ElementAt(1);
			Assert.AreEqual("MyTestFixture2", ns.NestedTests.Single().DisplayName);
		}
	}
}
