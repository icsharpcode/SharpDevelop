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

namespace PythonBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseFromSysImportWithoutIdentifierTestFixture
	{
		ICompilationUnit compilationUnit;
		PythonImport import;
		
		[SetUp]
		public void Init()
		{
			string python = "from sys import";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);
			import = compilationUnit.UsingScope.Usings[0] as PythonImport;
		}
		
		[Test]
		public void UsingAsPythonImportDoesNotHaveEmptyStringIdentifier()
		{
			Assert.IsFalse(import.HasIdentifier(String.Empty));
		}
		
		[Test]
		public void UsingAsPythonImportDoesNotHaveNullIdentifier()
		{
			Assert.IsFalse(import.HasIdentifier(null));
		}
		
		[Test]
		public void UsingAsPythonImportContainsSysModuleName()
		{
			Assert.AreEqual("sys", import.Module);
		}
	}
}
