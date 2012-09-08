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
	/// Tests that the inner test class is removed when its TestFixture attribute is removed.
	/// </summary>
	[TestFixture]
	public class InnerClassTestFixtureAttributeRemovedTestFixture : ProjectTestFixtureBase
	{
		[SetUp]
		public void Init()
		{
			CreateNUnitProject(Parse(@"
using NUnit.Framework;
namespace MyTests {
	class A {
		[TestFixture]
		class Inner {}
	}
}"));
		}
		
		[Test]
		public void RemoveAttribute_Leaves_No_TestClasses()
		{
			UpdateCodeFile(@"
using NUnit.Framework;
namespace MyTests {
	class A {
		class Inner {}
	}
}");
			Assert.AreEqual(0, testProject.TestClasses.Count);
		}
		
		[Test]
		public void MoveAttributeToOuterClass()
		{
			UpdateCodeFile(@"
using NUnit.Framework;
namespace MyTests {
	[TestFixture]
	class A {
		class Inner {}
	}
}");
			Assert.IsEmpty(testProject.TestClasses.Single().NestedClasses);
		}
	}
}
