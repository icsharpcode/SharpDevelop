// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Antlr.Runtime;
using Xebic.Parsers.ES3;

namespace ICSharpCode.JavaScriptBinding
{
	public static class ITokenExtensions
	{
		public static int BeginColumn(this IToken token)
		{
			return token.CharPositionInLine + 1;
		}
		
		public static int EndColumn(this IToken token)
		{
			return token.CharPositionInLine + token.Text.Length + 1;
		}
		
		public static bool IsSingleLineComment(this IToken token)
		{
			return token.Type == ES3Lexer.SingleLineComment;
		}
	}
}
