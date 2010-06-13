// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			Assert.AreEqual(2, testClass.TestMethods.Count);
		}
		
		[Test]
		public void FirstTestMethodNameIsFoo()
		{
			Assert.AreEqual("Foo", testClass.TestMethods[0].Name);
		}
		
		[Test]
		public void SecondTestMethodNameIsBar()
		{
			Assert.AreEqual("Bar", testClass.TestMethods[1].Name);
		}
	}
}
