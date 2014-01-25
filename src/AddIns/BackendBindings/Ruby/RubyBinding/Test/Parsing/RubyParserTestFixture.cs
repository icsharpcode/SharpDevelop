// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using RubyBinding.Tests;
using RubyBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

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
			createInfo.Solution = new Solution(new MockProjectChangeWatcher());
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
		public void ParseWithNullTextBuffer()
		{
			DefaultProjectContent projectContent = new DefaultProjectContent();
			ITextBuffer textBuffer = null;
			ICompilationUnit unit = parser.Parse(projectContent, null, textBuffer);
			Assert.IsInstanceOf(typeof(DefaultCompilationUnit), unit);
		}
	}
}
