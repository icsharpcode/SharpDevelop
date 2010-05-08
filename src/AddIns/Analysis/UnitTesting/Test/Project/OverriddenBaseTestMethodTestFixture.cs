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

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class OverriddenBaseTestMethodTestFixture
	{
		TestClass testClass;
		MockClass c;
		
		[SetUp]
		public void SetUp()
		{			
			MockProjectContent projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			
			// Create the base test class.
			MockClass baseClass = new MockClass("ICSharpCode.Tests.BaseClass");
			baseClass.ProjectContent = projectContent;

			// Add a virtual method to base class.
			MockMethod baseMethod = new MockMethod("VirtualTestMethod");
			baseMethod.Attributes.Add(new MockAttribute("Test"));
			baseMethod.DeclaringType = baseClass;
			baseMethod.IsVirtual = true;
			baseClass.Methods.Add(baseMethod);

			// Add a second method that is virtual but will not be overriden.
			baseMethod = new MockMethod("VirtualNonOverriddenTestMethod");
			baseMethod.Attributes.Add(new MockAttribute("Test"));
			baseMethod.DeclaringType = baseClass;
			baseMethod.IsVirtual = true;
			baseClass.Methods.Add(baseMethod);
			
			// Create the derived test class.
			c = new MockClass("ICSharpCode.Tests.DerivedClass");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			c.ProjectContent = projectContent;
			projectContent.Classes.Add(c);

			// Create a new test method that overrides one of the base class methods.		
			MockMethod method = new MockMethod("VirtualTestMethod");
			method.Attributes.Add(new MockAttribute("Test"));
			method.DeclaringType = c;
			method.IsOverride = true;
			c.Methods.Add(method);

			// Set derived class's base class.
			c.BaseClass = baseClass;
			
			// Create TestClass.
			testClass = new TestClass(c);
		}

		[Test]
		public void NonOverriddenVirtualBaseMethodExists()
		{
			Assert.IsTrue(testClass.TestMethods.Contains("BaseClass.VirtualNonOverriddenTestMethod"));
		}

		[Test]
		public void VirtualBaseMethodDoesNotExistSinceItIsOverriddenInDerivedClass()
		{
			Assert.IsFalse(testClass.TestMethods.Contains("BaseClass.VirtualTestMethod"));
		}

		[Test]
		public void DerivedClassTestMethodExists()
		{
			Assert.IsTrue(testClass.TestMethods.Contains("VirtualTestMethod"));
		}
	}
}
