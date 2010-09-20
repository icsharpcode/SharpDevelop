// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseTestClassTestFixture
	{
		ICompilationUnit compilationUnit;
		IClass c;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string python = 
				"import unittest\r\n" +
				"\r\n" +
				"class simpleTest(unittest.TestCase):\r\n" +
				"    def testSuccess(self):\r\n" +
				"        assert True\r\n" +
				"\r\n" +
				"    def testFailure(self):\r\n" +
				"        assert False\r\n" +
				"\r\n";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);
			if (compilationUnit.Classes.Count > 0) {
				c = compilationUnit.Classes[0];
			}
		}
		
		[Test]
		public void SimpleTestFirstBaseTypeIsUnitTestTestCase()
		{
			IReturnType baseType = c.BaseTypes[0];
			string actualBaseTypeName = baseType.FullyQualifiedName;
			string expectedBaseTypeName = "unittest.TestCase";
			Assert.AreEqual(expectedBaseTypeName, actualBaseTypeName);
		}
	}
}
