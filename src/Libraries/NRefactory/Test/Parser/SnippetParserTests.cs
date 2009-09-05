// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
