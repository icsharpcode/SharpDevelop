// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests;

namespace RubyBinding.Tests.Parsing
{
	/// <summary>
	/// Tests that the rubyparser does not throw an exception
	/// when the ruby code is invalid.
	/// </summary>
	[TestFixture]
	public class ParseInvalidRubyCodeTestFixture
	{
		ICompilationUnit compilationUnit;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string Ruby = "class Class1\r\n" +
				"    @\r\n" +
				"end";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			RubyParser parser = new RubyParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.rb", Ruby);			
		}
		
		[Test]
		public void CompilationUnitIsNotNull()
		{
			Assert.IsNotNull(compilationUnit);
		}
		
		[Test]
		public void FileNameSet()
		{
			Assert.AreEqual(@"C:\test.rb", compilationUnit.FileName);
		}
	}
}
