// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
