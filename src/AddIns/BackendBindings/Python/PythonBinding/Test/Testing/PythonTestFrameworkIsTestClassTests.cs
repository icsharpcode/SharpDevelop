// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Testing
{
	[TestFixture]
	public class PythonTestFrameworkIsTestClassTests
	{
		PythonTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new PythonTestFramework();
		}
		
		[Test]
		public void CreateClassWithUnitTestTestCaseBaseTypeReturnsClassWithFirstBaseTypeEqualToTestCase()
		{
			IClass c = MockClass.CreateClassWithBaseType("unittest.TestCase");
			string name = c.BaseTypes[0].FullyQualifiedName;
			string expectedName = "unittest.TestCase";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void IsTestClassReturnsTrueWhenClassFirstBaseTypeIsUnitTestTestCase()
		{
			MockClass c = MockClass.CreateClassWithBaseType("unittest.TestCase");
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
			MockClass baseClass = MockClass.CreateClassWithBaseType("unittest.TestCase");
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			DefaultReturnType returnType = new DefaultReturnType(baseClass);
			c.BaseTypes.Add(returnType);
			
			Assert.IsTrue(testFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueWhenClassFirstBaseTypeIsUnitTest2TestCase()
		{
			MockClass c = MockClass.CreateClassWithBaseType("unittest2.TestCase");
			Assert.IsTrue(testFramework.IsTestClass(c));
		}
	}
}
