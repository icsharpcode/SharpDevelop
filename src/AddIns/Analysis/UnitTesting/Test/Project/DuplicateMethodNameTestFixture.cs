// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that the TestProject can handle a test class that 
	/// has two test methods with the same name.
	/// </summary>
	[TestFixture]
	public class DuplicateMethodNameTestFixture
	{
		TestClass testClass;
		
		[SetUp]
		public void Init()
		{	
			// Add a test class.
			MockProjectContent projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			MockClass c = new MockClass("RootNamespace.MyTestFixture");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			c.ProjectContent = projectContent;
			projectContent.Classes.Add(c);
			
			// Add first method.
			MockMethod method = new MockMethod("MyTest");
			method.Attributes.Add(new MockAttribute("Test"));
			method.DeclaringType = c;
			c.Methods.Add(method);
			
			// Add duplicate method.
			c.Methods.Add(method);
			
			// Add a base class that has duplicate methods.
			MockClass baseClass = new MockClass("RootNamespace.MyTestFixtureBase");
			baseClass.Attributes.Add(new MockAttribute("TestFixture"));
			baseClass.ProjectContent = projectContent;
			projectContent.Classes.Add(baseClass);
			c.BaseClass = baseClass;
			baseClass.Methods.Add(method);
			baseClass.Methods.Add(method);
			
			// Create test class.
			testClass = new TestClass(c);
		}
		
		/// <summary>
		/// Two test methods.
		/// </summary>
		[Test]
		public void TwoTestMethods()
		{
			Assert.AreEqual(2, testClass.TestMethods.Count);
		}
		
		[Test]
		public void TestMethodName()
		{
			Assert.AreEqual("MyTest", testClass.TestMethods[0].Name);
		}
		
		[Test]
		public void BaseClassTestMethodName()
		{
			Assert.AreEqual("MyTestFixtureBase.MyTest", testClass.TestMethods[1].Name);
		}
	}
}
