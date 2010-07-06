// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Lexer.VB
{
	[TestFixture]
	public class ImplicitLineContinuationTests
	{
		[Test]
		public void Example1()
		{
			string code = @"Module Test
    Sub Print(
        Param1 As Integer,
        Param2 As Integer)

        If (Param1 < Param2) Or
           (Param1 > Param2) Then
            Console.WriteLine(""Not equal"")
        End If
    End Sub
End Module";
			
			ILexer lexer = GenerateLexer(new StringReader(code));
			
			CheckTokens(lexer, Tokens.Module, Tokens.Identifier, Tokens.EOL,
			            Tokens.Sub, Tokens.Identifier, Tokens.OpenParenthesis,
			            Tokens.Identifier, Tokens.As, Tokens.Integer, Tokens.Comma,
			            Tokens.Identifier, Tokens.As, Tokens.Integer, Tokens.CloseParenthesis, Tokens.EOL,
			            Tokens.If, Tokens.OpenParenthesis, Tokens.Identifier, Tokens.LessThan, Tokens.Identifier, Tokens.CloseParenthesis, Tokens.Or,
			            Tokens.OpenParenthesis, Tokens.Identifier, Tokens.GreaterThan, Tokens.Identifier, Tokens.CloseParenthesis, Tokens.Then, Tokens.EOL,
			            Tokens.Identifier, Tokens.Dot, Tokens.Identifier, Tokens.OpenParenthesis, Tokens.LiteralString, Tokens.CloseParenthesis, Tokens.EOL,
			            Tokens.End, Tokens.If, Tokens.EOL,
			            Tokens.End, Tokens.Sub, Tokens.EOL,
			            Tokens.End, Tokens.Module);
		}
		
		[Test]
		public void QualifierInWith()
		{
			string code = @"Module Test
	Sub Print
		With xml
			Dim a = b.
				d
			Dim c = .
				Count
		End With
	End Sub
End Module";
			
			ILexer lexer = GenerateLexer(new StringReader(code));
			
			CheckTokens(lexer, Tokens.Module, Tokens.Identifier, Tokens.EOL,
			            Tokens.Sub, Tokens.Identifier, Tokens.EOL,
			            Tokens.With, Tokens.Identifier, Tokens.EOL,
			            Tokens.Dim, Tokens.Identifier, Tokens.Assign, Tokens.Identifier, Tokens.Dot, Tokens.Identifier, Tokens.EOL,
			            Tokens.Dim, Tokens.Identifier, Tokens.Assign, Tokens.Dot, Tokens.EOL,
			            Tokens.Identifier, Tokens.EOL,
			            Tokens.End, Tokens.With, Tokens.EOL,
			            Tokens.End, Tokens.Sub, Tokens.EOL,
			            Tokens.End, Tokens.Module);
		}
		
		[Test]
		public void Example2()
		{
			string code = @"Module Test
	Sub Print
		Dim a = _
			
			y
	End Sub
End Module";
			
			ILexer lexer = GenerateLexer(new StringReader(code));
			
			CheckTokens(lexer, Tokens.Module, Tokens.Identifier, Tokens.EOL,
			            Tokens.Sub, Tokens.Identifier, Tokens.EOL,
			            Tokens.Dim, Tokens.Identifier, Tokens.Assign, Tokens.EOL, Tokens.Identifier, Tokens.EOL,
			            Tokens.End, Tokens.Sub, Tokens.EOL,
			            Tokens.End, Tokens.Module);
		}
		
		#region Helpers
		ILexer GenerateLexer(StringReader sr)
		{
			return ParserFactory.CreateLexer(SupportedLanguage.VBNet, sr);
		}
		
		void CheckTokens(ILexer lexer, params int[] tokens)
		{
			for (int i = 0; i < tokens.Length; i++) {
				int token = tokens[i];
				Token t = lexer.NextToken();
				int next = t.Kind;
				Assert.AreEqual(token, next, "{2} of {3}: {0} != {1}; at {4}", Tokens.GetTokenString(token), Tokens.GetTokenString(next), i + 1, tokens.Length, t.Location);
			}
		}
		#endregion
	}
}
