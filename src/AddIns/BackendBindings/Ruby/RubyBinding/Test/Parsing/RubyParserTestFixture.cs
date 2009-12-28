// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using RubyBinding.Tests;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Parsing
{
	/// <summary>
	/// Tests the RubyParser.
	/// </summary>
	[TestFixture]
	public class ParserTestFixture
	{
		RubyParser parser;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			parser = new RubyParser();
		}
		
		[Test]
		public void CanParseRubyFileName()
		{
			Assert.IsTrue(parser.CanParse("test.rb"));
		}

		[Test]
		public void CannotParseTextFileName()
		{
			Assert.IsFalse(parser.CanParse("test.txt"));
		}
		
		[Test]
		public void CannotParseNullFileName()
		{
			string fileName = null;
			Assert.IsFalse(parser.CanParse(fileName));
		}
		
		[Test]
		public void CanParseUpperCaseRubyFileName()
		{
			Assert.IsTrue(parser.CanParse("TEST.RB"));
		}
		
		[Test]
		public void NoLexerTags()
		{
			Assert.AreEqual(0, parser.LexerTags.Length);
		}
		
		[Test]
		public void SetLexerTags()
		{
			RubyParser parser = new RubyParser();
			string[] tags = new string[] {"Test"};
			parser.LexerTags = tags;
			
			Assert.AreEqual(tags, parser.LexerTags);
		}
		
		[Test]
		public void LanguageProperties()
		{
			Assert.IsNotNull(parser.Language);
		}
		
		[Test]
		public void CannotParseNullProject()
		{
			IProject project = null;
			Assert.IsFalse(parser.CanParse(project));
		}
		
		[Test]
		public void CanParseRubyProject()
		{
			ProjectCreateInformation createInfo = new ProjectCreateInformation();
			createInfo.Solution = new Solution();
			createInfo.OutputProjectFileName = @"C:\projects\test.rbproj";
			RubyProject project = new RubyProject(createInfo);
			Assert.IsTrue(parser.CanParse(project));
		}
	
		[Test]
		public void CannotParseNonRubyLanguageProject()
		{
			MockProject project = new MockProject();
			project.Language = "Test";
			Assert.IsFalse(parser.CanParse(project));
		}
		
		[Test]
		public void ParseWithNullFileContent()
		{
			DefaultProjectContent projectContent = new DefaultProjectContent();
			ICompilationUnit unit = parser.Parse(projectContent, null, null);
			Assert.IsInstanceOf(typeof(DefaultCompilationUnit), unit);
		}
	}
}
