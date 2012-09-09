// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
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
	public class InnerClassNameChangesTestFixture : ProjectTestFixtureBase
	{
		TestClass originalA;
		
		[SetUp]
		public void Init()
		{
			CreateNUnitProject(Parse(@"
using NUnit.Framework;
namespace MyTests {
	class A {
		class InnerTest {
			[Test]
			public void M() {}
		}
	}
}
"));
			
			originalA = testProject.GetTestClass("MyTests.A");
			
			UpdateCodeFile(@"
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
			Assert.AreSame(originalA, testProject.GetTestClass("MyTests.A"));
		}
		
		[Test]
		public void InnerClassRenamed()
		{
			Assert.AreEqual("InnerTestMod", originalA.NestedClasses.Single().Name);
		}
	}
}
