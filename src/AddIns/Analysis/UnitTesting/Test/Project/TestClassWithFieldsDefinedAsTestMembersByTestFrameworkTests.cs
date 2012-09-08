// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class TestClassWithFieldsDefinedAsTestMembersByTestFrameworkTests : ProjectTestFixtureBase
	{
		[Test]
		public void TestMembers_ClassHasOneFieldDefinedAsTestMemberByTestFramework_FirstItemHasSameNameAsField()
		{
			var fakeTestFramework = new MockTestFramework();
			fakeTestFramework.AddTestClass("MyClass");
			fakeTestFramework.AddTestMember("MyClass.MyField");
			
			CreateProject(fakeTestFramework, Parse("class MyClass { int MyField; }"));
			
			TestMember testField = testProject.TestClasses.Single().Members.Single();
			Assert.AreEqual("MyField", testField.Name);
		}
	}
}
