// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.CSharp;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.NRefactory.Tests.Lexer.CSharp
{
	[TestFixture]
	public sealed class NumberLexerTests
	{
		ILexer GenerateLexer(StringReader sr)
		{
			return ParserFactory.CreateLexer(SupportedLanguages.CSharp, sr);
		}
		
		Token GetSingleToken(string text)
		{
			ILexer lexer = GenerateLexer(new StringReader(text));
			Token t = lexer.NextToken();
			Assert.AreEqual(Tokens.EOF, lexer.NextToken().kind, "Tokens.EOF");
			return t;
		}
		
		void CheckToken(string text, object val)
		{
			Token t = GetSingleToken(text);
			Assert.AreEqual(Tokens.Literal, t.kind, "Tokens.Literal");
			Assert.IsNotNull(t.literalValue, "literalValue is null");
			Assert.AreEqual(val, t.literalValue, "literalValue");
		}
		
		[Test]
		public void TestSingleDigit()
		{
			CheckToken("5", 5);
		}
		
		[Test]
		public void TestZero()
		{
			CheckToken("0", 0);
		}
		
		[Test]
		public void TestInteger()
		{
			CheckToken("66", 66);
		}
		
		[Test]
		public void TestOctalInteger()
		{
			CheckToken("077", 077);
			CheckToken("056", 056);
		}
		
		[Test]
		public void TestHexadecimalInteger()
		{
			CheckToken("0x99F", 0x99F);
			CheckToken("0xAB1f", 0xAB1f);
		}
		
		[Test]
		public void TestDouble()
		{
			CheckToken("1.0", 1.0);
		}
		
		[Test]
		public void TestFloat()
		{
			CheckToken("1.0f", 1.0f);
		}
	}
}
