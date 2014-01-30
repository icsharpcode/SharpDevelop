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
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.Testing
{
	[TestFixture]
	public class RubyTestFrameworkIsTestClassTests
	{
		RubyTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new RubyTestFramework();
		}
		
		[Test]
		public void CreateClassWithTestUnitTestCaseBaseTypeReturnsClassWithFirstBaseTypeEqualToTestCase()
		{
			IClass c = MockClass.CreateClassWithBaseType("Test.Unit.TestCase");
			string name = c.BaseTypes[0].FullyQualifiedName;
			string expectedName = "Test.Unit.TestCase";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void IsTestClassReturnsTrueWhenClassFirstBaseTypeIsUnitTestTestCase()
		{
			MockClass c = MockClass.CreateClassWithBaseType("Test.Unit.TestCase");
			Assert.IsTrue(testFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenClassHasNoBaseTypes()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			Assert.IsFalse(testFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsFalseForNull()
		{
			Assert.IsFalse(testFramework.IsTestClass(null));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenFirstBaseTypeIsSystemWindowsFormsForm()
		{
			MockClass c = MockClass.CreateClassWithBaseType("System.Windows.Forms.Form");
			Assert.IsFalse(testFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueWhenDerivedClassHasBaseClassDerivedFromTestCase()
		{
			MockClass baseClass = MockClass.CreateClassWithBaseType("Test.Unit.TestCase");
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			DefaultReturnType returnType = new DefaultReturnType(baseClass);
			c.BaseTypes.Add(returnType);
			
			Assert.IsTrue(testFramework.IsTestClass(c));
		}
	}
}
