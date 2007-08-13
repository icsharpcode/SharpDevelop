// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2658$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
namespace ICSharpCode.NRefactory.Tests.Lexer.VB
{
	[TestFixture]
	public class CustomLexerTests
	{
		ILexer GenerateLexer(StringReader sr)
		{
			return ParserFactory.CreateLexer(SupportedLanguage.VBNet, sr);
		}
		
		[Test]
		public void TestSingleEOLForMulitpleLines()
		{
			ILexer lexer = GenerateLexer(new StringReader("Stop\n\n\nEnd"));
			Assert.That(lexer.NextToken().kind, Is.EqualTo(Tokens.Stop));
			Assert.That(lexer.NextToken().kind, Is.EqualTo(Tokens.EOL));
			Assert.That(lexer.NextToken().kind, Is.EqualTo(Tokens.End));
			Assert.That(lexer.NextToken().kind, Is.EqualTo(Tokens.EOL));
			Assert.That(lexer.NextToken().kind, Is.EqualTo(Tokens.EOF));
		}
		
		[Test]
		public void TestSingleEOLForMulitpleLinesWithContinuation()
		{
			ILexer lexer = GenerateLexer(new StringReader("Stop\n _\n\nEnd"));
			Assert.That(lexer.NextToken().kind, Is.EqualTo(Tokens.Stop));
			Assert.That(lexer.NextToken().kind, Is.EqualTo(Tokens.EOL));
			Assert.That(lexer.NextToken().kind, Is.EqualTo(Tokens.End));
			Assert.That(lexer.NextToken().kind, Is.EqualTo(Tokens.EOL));
			Assert.That(lexer.NextToken().kind, Is.EqualTo(Tokens.EOF));
		}
	}
}
