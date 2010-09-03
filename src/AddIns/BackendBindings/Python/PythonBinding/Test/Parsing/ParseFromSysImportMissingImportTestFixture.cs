// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseFromSysImportMissingImportTestFixture
	{
		ICompilationUnit compilationUnit;
		PythonFromImport import;
		
		[SetUp]
		public void Init()
		{
			string python = "from sys";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);
			import = compilationUnit.UsingScope.Usings[0] as PythonFromImport;
		}
		
		[Test]
		public void UsingAsPythonImportDoesNotHaveEmptyStringIdentifier()
		{
			Assert.IsFalse(import.IsImportedName(String.Empty));
		}
		
		[Test]
		public void UsingAsPythonImportDoesNotHaveNullIdentifier()
		{
			Assert.IsFalse(import.IsImportedName(null));
		}
		
		[Test]
		public void UsingAsPythonImportContainsSysModuleName()
		{
			Assert.AreEqual("sys", import.Module);
		}
	}
}
