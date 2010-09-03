// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseClassNestedInsideMethodTestFixture
	{
		ICompilationUnit compilationUnit;
		IClass c;
		
		[SetUp]
		public void SetUpFixture()
		{
			string python =
				"class MyClass:\r\n" +
				"    def firstMethod(self):\r\n" +
				"        class NestedClass:\r\n" +
				"            def firstNestedClassMethod(self):\r\n" +
				"                pass\r\n" +
				"\r\n" +
				"    def secondMethod(self):\r\n" +
				"        pass\r\n" +
				"\r\n";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);
			if (compilationUnit.Classes.Count > 0) {
				c = compilationUnit.Classes[0];
			}
		}
		
		[Test]
		public void CompilationUnitHasOneClass()
		{
			Assert.AreEqual(1, compilationUnit.Classes.Count);
		}
		
		[Test]
		public void MyClassHasTwoMethods()
		{
			Assert.AreEqual(2, c.Methods.Count);
		}
	}
}
