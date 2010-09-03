// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Parsing
{
	/// <summary>
	/// Tests that import statements are added to the compilation 
	/// unit's Using property.
	/// </summary>
	[TestFixture]
	public class ParseImportTestFixture
	{
		ICompilationUnit compilationUnit;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string python = "import System";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);			
		}

		[Test]
		public void OneUsing()
		{
			Assert.AreEqual(1, compilationUnit.UsingScope.Usings.Count);
		}
		
		[Test]
		public void UsingSystem()
		{
			Assert.AreEqual("System", compilationUnit.UsingScope.Usings[0].Usings[0]);
		}
	}
}
