// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class CreateTestProjectWithTwoTestMethodsTestFixture
	{
		TestProject testProject;
		TestClass testClass;
		
		[SetUp]
		public void Init()
		{
			string[] methods = new string[] { "Foo", "Bar" };
			testProject = 
				TestProjectHelper.CreateTestProjectWithTestClassTestMethods("MyClass", methods);
			
			testClass = testProject.TestClasses[0];
		}
		
		[Test]
		public void TestProjectWithTwoTestMethodsHasTwoMethods()
		{
			Assert.AreEqual(2, testClass.TestMembers.Count);
		}
		
		[Test]
		public void FirstTestMethodNameIsFoo()
		{
			Assert.AreEqual("Foo", testClass.TestMembers[0].Name);
		}
		
		[Test]
		public void SecondTestMethodNameIsBar()
		{
			Assert.AreEqual("Bar", testClass.TestMembers[1].Name);
		}
	}
}
