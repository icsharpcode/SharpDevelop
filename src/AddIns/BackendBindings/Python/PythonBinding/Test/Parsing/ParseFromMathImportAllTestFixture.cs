// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseFromMathImportAllTestFixture
	{
		ICompilationUnit compilationUnit;
		PythonFromImport import;
		
		[SetUp]
		public void Init()
		{
			string python = "from math import *";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);
			import = compilationUnit.UsingScope.Usings[0] as PythonFromImport;
		}
		
		[Test]
		public void PythonImportContainsMathModuleName()
		{
			Assert.AreEqual("math", import.Module);
		}
		
		[Test]
		public void PythonImportImportsEverythingReturnsTrue()
		{
			Assert.IsTrue(import.ImportsEverything);
		}
		
		[Test]
		public void PythonImportGetIdentifierForAliasDoesNotThrowNullReferenceException()
		{
			string identifier = String.Empty;
			Assert.DoesNotThrow(delegate { identifier = import.GetOriginalNameForAlias("unknown"); });
			Assert.IsNull(identifier);
		}
	}
}
