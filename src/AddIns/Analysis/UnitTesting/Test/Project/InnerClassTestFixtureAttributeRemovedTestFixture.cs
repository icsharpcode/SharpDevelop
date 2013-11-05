// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that the inner test class is removed when its TestFixture attribute is removed.
	/// </summary>
	[TestFixture]
	public class InnerClassTestFixtureAttributeRemovedTestFixture : NUnitTestProjectFixtureBase
	{
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFile("test.cs", @"
using NUnit.Framework;
namespace MyTests {
	class A {
		[TestFixture]
		class Inner {}
	}
}");
			testProject.EnsureNestedTestsInitialized();
		}
		
		[Test]
		public void RemoveAttribute_Leaves_No_TestClasses()
		{
			UpdateCodeFile("test.cs", @"
using NUnit.Framework;
namespace MyTests {
	class A {
		class Inner {}
	}
}");
			Assert.AreEqual(0, testProject.NestedTests.Count);
		}
		
		[Test]
		public void MoveAttributeToOuterClass()
		{
			UpdateCodeFile("test.cs", @"
using NUnit.Framework;
namespace MyTests {
	[TestFixture]
	class A {
		class Inner {}
	}
}");
			Assert.IsEmpty(testProject.GetTestClass(new FullTypeName("MyTests.A")).NestedTests);
		}
	}
}
