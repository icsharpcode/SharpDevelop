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
	public class ParseImportSysTestFixture
	{
		ICompilationUnit compilationUnit;
		PythonImport import;
		
		[SetUp]
		public void Init()
		{
			string python = "import sys";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);
			import = compilationUnit.UsingScope.Usings[0] as PythonImport;
		}
		
		[Test]
		public void PythonImportHasExitIdentifier()
		{
			Assert.AreEqual("sys", import.Module);
		}
	}
}
