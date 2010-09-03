// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

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
