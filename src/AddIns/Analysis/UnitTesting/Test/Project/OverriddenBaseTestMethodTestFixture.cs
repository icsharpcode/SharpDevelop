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
	public class OverriddenBaseTestMethodTestFixture
	{
		TestClass testClass;
		MockClass c;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void SetUp()
		{
			MockProjectContent projectContent = new MockProjectContent();
			projectContent.Language = LanguageProperties.None;
			
			// Create the base test class.
			MockClass baseClass = new MockClass(projectContent, "ICSharpCode.Tests.BaseClass");
			
			// Add a virtual method to base class.
			MockMethod baseMethod = new MockMethod(baseClass, "VirtualTestMethod");
			baseMethod.Attributes.Add(new MockAttribute("Test"));
			baseMethod.Modifiers = ModifierEnum.Virtual;
			baseClass.Methods.Add(baseMethod);
			
			// Add a second method that is virtual but will not be overriden.
			baseMethod = new MockMethod(baseClass, "VirtualNonOverriddenTestMethod");
			baseMethod.Attributes.Add(new MockAttribute("Test"));
			baseMethod.Modifiers = ModifierEnum.Virtual;
			baseClass.Methods.Add(baseMethod);
			
			// Create the derived test class.
			c = new MockClass(projectContent, "ICSharpCode.Tests.DerivedClass");
			c.Attributes.Add(new MockAttribute("TestFixture"));
			projectContent.Classes.Add(c);
			
			// Create a new test method that overrides one of the base class methods.		
			MockMethod method = new MockMethod(c, "VirtualTestMethod");
			method.Attributes.Add(new MockAttribute("Test"));
			method.Modifiers = ModifierEnum.Override;
			c.Methods.Add(method);
			
			// Set derived class's base class.
			c.AddBaseClass(baseClass);
			
			// Create TestClass.
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			testClass = new TestClass(c, testFrameworks);
		}

		[Test]
		public void NonOverriddenVirtualBaseMethodExists()
		{
			Assert.IsTrue(testClass.TestMembers.Contains("BaseClass.VirtualNonOverriddenTestMethod"));
		}

		[Test]
		public void VirtualBaseMethodDoesNotExistSinceItIsOverriddenInDerivedClass()
		{
			Assert.IsFalse(testClass.TestMembers.Contains("BaseClass.VirtualTestMethod"));
		}

		[Test]
		public void DerivedClassTestMethodExists()
		{
			Assert.IsTrue(testClass.TestMembers.Contains("VirtualTestMethod"));
		}
	}
}
