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
using PythonBinding.Tests;

namespace PythonBinding.Tests.Parsing
{
	/// <summary>
	/// Tests that the python parser does not throw an exception
	/// when the python code is invalid.
	/// </summary>
	[TestFixture]
	public class ParseInvalidPythonCodeTestFixture
	{
		ICompilationUnit compilationUnit;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string python = "class";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);			
		}
		
		[Test]
		public void CompilationUnitIsNotNull()
		{
			Assert.IsNotNull(compilationUnit);
		}
		
		[Test]
		public void FileNameSet()
		{
			Assert.AreEqual(@"C:\test.py", compilationUnit.FileName);
		}
	}
}
