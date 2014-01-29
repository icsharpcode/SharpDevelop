// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.MSTest;
using NUnit.Framework;

namespace MSTest.SharpDevelop.Tests
{
	[TestFixture]
	public class MSTestClassTests : MSTestBaseTests
	{
		bool IsTestClass()
		{
			return MSTestClass.IsTestClass(GetFirstTypeDefinition());
		}
		
		[Test]
		public void IsTestClass_ClassHasNoAttributes_ReturnsFalse()
		{
			AddCodeFile("myclass.cs", "class MyTest {}");
			
			bool result = IsTestClass();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestClass_ClassHasTestFixtureAttributeMissingAttributePart_ReturnsTrue()
		{
			AddCodeFile("myclass.cs", "[TestClass]class MyTest {}");
			
			bool result = IsTestClass();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestClass_ClassHasTestClassAttributeAndIsAbstract_ReturnsFalse()
		{
			AddCodeFile("myclass.cs", "[TestClass] abstract class MyTest {}");
			
			bool result = IsTestClass();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestClass_ClassHasTestClassAttributeIncludingAttributePart_ReturnsTrue()
		{
			AddCodeFile("myclass.cs", "[TestClassAttribute]class MyTest {}");
			
			bool result = IsTestClass();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestClass_ClassHasFullyQualifiedMSTestClassAttribute_ReturnsTrue()
		{
			AddCodeFile("myclass.cs", "[Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute]class MyTest {}");
			
			bool result = IsTestClass();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestClass_ClassIsNull_ReturnsFalse()
		{
			bool result = MSTestClass.IsTestClass(null);
			
			Assert.IsFalse(result);
		}
	}
}
