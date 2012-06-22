// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.NRefactory.Ast;
using System;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests
{
	[TestFixture]
	public class SnippetParserTests
	{
		[Test]
		public void InvalidExpressionSyntax()
		{
			// SD2-1584: ensure we don't crash on this invalid VB code
			SnippetParser parser = new SnippetParser(SupportedLanguage.VBNet);
			INode node = parser.Parse("i == 5");
			Assert.IsTrue(parser.Errors.Count > 0);
		}
		
		[Test]
		public void SimpleXmlDocument()
		{
			SnippetParser parser = new SnippetParser(SupportedLanguage.VBNet);
			INode node = parser.Parse("Dim doc = <test><%= test %></test>");
			Assert.IsTrue(parser.Errors.Count == 0);
		}
		
		[Test]
		public void InvalidXmlLiteral()
		{
			// ensure we don't crash on this invalid VB code
			SnippetParser parser = new SnippetParser(SupportedLanguage.VBNet);
			INode node = parser.Parse("<Cell StyleID=<%= print_title %>>");
			Assert.IsTrue(parser.Errors.Count > 0);
		}
	}
}
