// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
