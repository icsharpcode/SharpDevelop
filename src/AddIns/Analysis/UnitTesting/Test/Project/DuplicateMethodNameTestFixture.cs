// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void Init()
		{
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			
			// Add a test class.
			MockProjectContent projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			MockClass c = new MockClass(projectContent, "RootNamespace.MyTestFixture");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(c);
			
			// Add first method.
			var method = new MockMethod(c, "MyTest");
			method.Attributes.Add(new MockAttribute("Test"));
			c.Methods.Add(method);
			
			// Add duplicate method.
			c.Methods.Add(method);
			
			// Add a base class that has duplicate methods.
			MockClass baseClass = new MockClass(projectContent, "RootNamespace.MyTestFixtureBase");
			baseClass.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(baseClass);
			c.AddBaseClass(baseClass);
			var baseClassMethod = new MockMethod(baseClass, "MyTest");
			baseClassMethod.Attributes.Add(new MockAttribute("Test"));
			baseClass.Methods.Add(baseClassMethod);
			baseClass.Methods.Add(baseClassMethod);
			
			// Create test class.
			testClass = new TestClass(c, testFrameworks);
		}
		
		/// <summary>
		/// Two test methods.
		/// </summary>
		[Test]
		public void TwoTestMethods()
		{
			Assert.AreEqual(2, testClass.TestMembers.Count);
		}
		
		[Test]
		public void TestMethodName()
		{
			Assert.AreEqual("MyTest", testClass.TestMembers[0].Name);
		}
		
		[Test]
		public void BaseClassTestMethodName()
		{
			Assert.AreEqual("MyTestFixtureBase.MyTest", testClass.TestMembers[1].Name);
		}
	}
}
