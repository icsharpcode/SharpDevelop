// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using MbUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.CSharp;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.NRefactory.Tests.Lexer.CSharp
{
	[TestFixture]
	public sealed class CustomLexerTests
	{
		ILexer GenerateLexer(StringReader sr)
		{
			return ParserFactory.CreateLexer(SupportedLanguages.CSharp, sr);
		}
		
		[Test]
		public void TestEmptyBlock()
		{
			ILexer lexer = GenerateLexer(new StringReader("{}+"));
			Assert.AreEqual(Tokens.OpenCurlyBrace, lexer.NextToken().kind);
			Assert.AreEqual(Tokens.CloseCurlyBrace, lexer.NextToken().kind);
			Assert.AreEqual(Tokens.Plus, lexer.NextToken().kind);
			Assert.AreEqual(Tokens.EOF, lexer.NextToken().kind);
		}
		
		[Test]
		public void TestSkippedEmptyBlock()
		{
			ILexer lexer = GenerateLexer(new StringReader("{}+"));
			Assert.AreEqual(Tokens.OpenCurlyBrace, lexer.NextToken().kind);
			lexer.NextToken();
			lexer.SkipCurrentBlock();
			Assert.AreEqual(Tokens.CloseCurlyBrace, lexer.LookAhead.kind);
			Assert.AreEqual(Tokens.Plus, lexer.NextToken().kind);
			Assert.AreEqual(Tokens.EOF, lexer.NextToken().kind);
		}
		
		[Test]
		public void TestSkippedNonEmptyBlock()
		{
			ILexer lexer = GenerateLexer(new StringReader("{ TestMethod('}'); /* }}} */ while(1) {break;} }+"));
			Assert.AreEqual(Tokens.OpenCurlyBrace, lexer.NextToken().kind);
			lexer.NextToken();
			lexer.SkipCurrentBlock();
			Assert.AreEqual(Tokens.CloseCurlyBrace, lexer.LookAhead.kind);
			Assert.AreEqual(Tokens.Plus, lexer.NextToken().kind);
			Assert.AreEqual(Tokens.EOF, lexer.NextToken().kind);
		}
	}
}
