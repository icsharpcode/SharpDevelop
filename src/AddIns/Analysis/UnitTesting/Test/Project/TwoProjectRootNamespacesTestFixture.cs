// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
