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
	public class ParseFromSysImportExitAsMyExitTestFixture
	{
		ICompilationUnit compilationUnit;
		PythonFromImport import;
		
		[SetUp]
		public void Init()
		{
			string python = "from sys import exit as myexit";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);
			import = compilationUnit.UsingScope.Usings[0] as PythonFromImport;
		}
		
		[Test]
		public void UsingAsPythonImportHasMyExitIdentifier()
		{
			Assert.IsTrue(import.IsImportedName("myexit"));
		}
		
		[Test]
		public void UsingAsPythonImportDoesNotHaveExitIdentifier()
		{
			Assert.IsFalse(import.IsImportedName("exit"));
		}
		
		[Test]
		public void PythonImportGetIdentifierFromAliasReturnsExitForMyExit()
		{
			Assert.AreEqual("exit", import.GetOriginalNameForAlias("myexit"));
		}
	}
}
