// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that the TestProject is correctly updated after the inner class name changes.
	/// </summary>
	[TestFixture]
	public class InnerClassNameChangesTestFixture : NUnitTestProjectFixtureBase
	{
		NUnitTestClass originalA;
		
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFile("test.cs", @"
using NUnit.Framework;
namespace MyTests {
	class A {
		class InnerTest {
			[Test]
			public void M() {}
		}
	}
}
");
			
			originalA = testProject.GetTestClass(new FullTypeName("MyTests.A"));
			
			UpdateCodeFile("test.cs", @"
using NUnit.Framework;
namespace MyTests {
	class A {
		class InnerTestMod {
			[Test]
			public void M() {}
		}
	}
}
");
		}
		
		[Test]
		public void OuterClassNotChanged()
		{
			Assert.IsNotNull(originalA);
			Assert.AreSame(originalA, testProject.GetTestClass(new FullTypeName("MyTests.A")));
		}
		
		[Test]
		public void InnerClassRenamed()
		{
			Assert.AreEqual("InnerTestMod", originalA.NestedTests.Single().DisplayName);
		}
	}
}
