using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.NRefactory.Tests.Lexer.VB
{
	[TestFixture]
	public sealed class LiteralsTests
	{
		ILexer GenerateLexer(StringReader sr)
		{
			return ParserFactory.CreateLexer(SupportedLanguages.VBNet, sr);
		}
		
		Token GetSingleToken(string text)
		{
			ILexer lexer = GenerateLexer(new StringReader(text));
			Token t = lexer.NextToken();
			Assert.AreEqual(Tokens.EOL, lexer.NextToken().kind, "Tokens.EOL");
			Assert.AreEqual(Tokens.EOF, lexer.NextToken().kind, "Tokens.EOF");
			return t;
		}
		
		void CheckToken(string text, int tokenType, object val)
		{
			Token t = GetSingleToken(text);
			Assert.AreEqual(tokenType, t.kind, "Tokens.Literal");
			Assert.IsNotNull(t.literalValue, "literalValue is null");
			Assert.AreEqual(val, t.literalValue, "literalValue");
		}
		
		[Test]
		public void TestSingleDigit()
		{
			CheckToken("5", Tokens.LiteralInteger, 5);
		}
		
		[Test]
		public void TestZero()
		{
			CheckToken("0", Tokens.LiteralInteger, 0);
		}
		
		[Test]
		public void TestInteger()
		{
			CheckToken("15", Tokens.LiteralInteger, 15);
			CheckToken("8581", Tokens.LiteralInteger, 8581);
		}
		
		[Test]
		public void TestHexadecimalInteger()
		{
			CheckToken("&H10", Tokens.LiteralInteger, 0x10);
			CheckToken("&H10&", Tokens.LiteralInteger, 0x10);
			CheckToken("&h3ff&", Tokens.LiteralInteger, 0x3ff);
		}
		
		[Test]
		public void TestStringLiterals()
		{
			CheckToken("\"\"", Tokens.LiteralString, "");
			CheckToken("\"Hello, World!\"", Tokens.LiteralString, "Hello, World!");
			CheckToken("\"\"\"\"", Tokens.LiteralString, "\"");
		}
		
		[Test]
		public void TestCharacterLiterals()
		{
			CheckToken("\" \"c", Tokens.LiteralCharacter, ' ');
			CheckToken("\"!\"c", Tokens.LiteralCharacter, '!');
			CheckToken("\"\"\"\"c", Tokens.LiteralCharacter, '"');
		}
		
		/*
		 * TODO : Test the following:
		public const int LiteralDouble        = 6;
		public const int LiteralSingle        = 7;
		public const int LiteralDecimal       = 8;
		public const int LiteralDate          = 9;
		 */
	}
}
