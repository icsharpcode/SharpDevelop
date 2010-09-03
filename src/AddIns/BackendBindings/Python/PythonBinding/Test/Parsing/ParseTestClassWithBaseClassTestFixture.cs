// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseTestClassWithBaseClassTestFixture
	{
		ICompilationUnit compilationUnit;
		IClass c;
		DefaultProjectContent projectContent;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string python = 
				"import unittest\r\n" +
				"\r\n" +
				"class BaseTest(unittest.TestCase):\r\n" +
				"    def testSuccess(self):\r\n" +
				"        assert True\r\n" +
				"\r\n" +
				"class DerivedTest(BaseTest):\r\n" +
				"    pass\r\n" +
				"\r\n";
			
			projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			string fileName = @"C:\test.py";
			compilationUnit = parser.Parse(projectContent, fileName, python);
			projectContent.UpdateCompilationUnit(null, compilationUnit, fileName);
			if (compilationUnit.Classes.Count > 1) {
				c = compilationUnit.Classes[1];
			}
		}
		
		[Test]
		public void DerivedTestFirstBaseTypeIsBaseTestTestCase()
		{
			IReturnType baseType = c.BaseTypes[0];
			string actualBaseTypeName = baseType.FullyQualifiedName;
			string expectedBaseTypeName = "test.BaseTest";
			Assert.AreEqual(expectedBaseTypeName, actualBaseTypeName);
		}
		
		[Test]
		public void DerivedTestBaseClassNameIsBaseTest()
		{
			IClass baseClass = c.BaseClass;
			string actualName = baseClass.FullyQualifiedName;
			string expectedName = "test.BaseTest";
			Assert.AreEqual(expectedName, actualName);
		}
		
		[Test]
		public void ProjectContentGetClassReturnsBaseTest()
		{
			IClass c = projectContent.GetClass("test.BaseTest", 0);
			Assert.AreEqual("test.BaseTest", c.FullyQualifiedName);
		}
		
		[Test]
		public void CompilationUnitUsingScopeNamespaceNameIsNamespaceTakenFromFileName()
		{
			string namespaceName = compilationUnit.UsingScope.NamespaceName;
			string expectedNamespace = "test";
			Assert.AreEqual(expectedNamespace, namespaceName);
		}
		
		[Test]
		public void DerivedTestBaseClassHasTestCaseBaseClass()
		{
			IReturnType baseType = c.BaseTypes[0];
			IClass baseClass = baseType.GetUnderlyingClass();
			IReturnType baseBaseType = baseClass.BaseTypes[0];
			string actualBaseTypeName = baseBaseType.FullyQualifiedName;
			string expectedBaseTypeName = "unittest.TestCase";
			Assert.AreEqual(expectedBaseTypeName, actualBaseTypeName);
		}
		
		[Test]
		public void CompilationUnitUsingScopeHasParentUsingScopeWithNamespaceNameOfEmptyString()
		{
			IUsingScope parentUsingScope = compilationUnit.UsingScope.Parent;
			string namespaceName = parentUsingScope.NamespaceName;
			Assert.AreEqual(String.Empty, namespaceName);
		}
	}
}
