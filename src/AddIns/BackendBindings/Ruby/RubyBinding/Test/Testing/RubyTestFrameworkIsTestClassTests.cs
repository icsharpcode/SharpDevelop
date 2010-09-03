// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
