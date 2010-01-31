// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using PythonBinding.Tests;

namespace PythonBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseImportMultipleModulesTestFixture
	{
		ICompilationUnit compilationUnit;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string python = "import System, System.Console";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);
		}
		
		[Test]
		public void OneUsingStatementCreatedByParser()
		{
			Assert.AreEqual(1, compilationUnit.UsingScope.Usings.Count);
		}
		
		[Test]
		public void FirstNamespaceImportedIsSystem()
		{
			Assert.AreEqual("System", compilationUnit.UsingScope.Usings[0].Usings[0]);
		}

		[Test]
		public void SecondNamespaceImportedIsSystemConsole()
		{
			Assert.AreEqual("System.Console", compilationUnit.UsingScope.Usings[0].Usings[1]);
		}
	}
}
