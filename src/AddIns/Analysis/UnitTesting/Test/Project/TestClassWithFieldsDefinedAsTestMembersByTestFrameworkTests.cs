// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class TestClassWithFieldsDefinedAsTestMembersByTestFrameworkTests
	{
		TestClass testClass;
		MockClass fakeClass;
		MockTestFramework fakeTestFramework;
		MockRegisteredTestFrameworks fakeRegisteredTestFrameworks;
		
		void CreateTestClass()
		{
			fakeClass = MockClass.CreateMockClassWithoutAnyAttributes();
			fakeTestFramework = new MockTestFramework();
			fakeRegisteredTestFrameworks = new MockRegisteredTestFrameworks();
			fakeRegisteredTestFrameworks.AddTestFrameworkForProject(fakeClass.Project, fakeTestFramework);
			
			testClass = new TestClass(fakeClass, fakeRegisteredTestFrameworks);
		}
		
		DefaultField AddTestFieldDefinedAsTestMemberToClass(string name)
		{
			var field = new DefaultField(fakeClass, name);
			fakeClass.Fields.Add(field);
			fakeRegisteredTestFrameworks.AddTestMethod(field);
			
			return field;
		}
		
		[Test]
		public void TestMethods_ClassHasOneFieldDefinedAsTestMemberByTestFramework_FirstItemHasSameNameAsField()
		{
			CreateTestClass();
			AddTestFieldDefinedAsTestMemberToClass("MyField");
			
			TestMethod testField = testClass.TestMethods[0];
			string testFieldName = testField.Name;
			
			Assert.AreEqual("MyField", testFieldName);
		}
	}
}
