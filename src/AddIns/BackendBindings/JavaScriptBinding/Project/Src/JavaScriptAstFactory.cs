// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using ICSharpCode.SharpDevelop;
using Xebic.Parsers.ES3;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptAstFactory
	{
		string code;
		CommonTokenStream tokenStream;
		ES3Parser.program_return programReturn;
		
		public JavaScriptAstFactory(ITextBuffer textBuffer)
		{
			code = textBuffer.Text;
		}
		
		public JavaScriptAst Create()
		{
			CreateTokenStream();
			var parser = new ES3Parser(tokenStream);
			programReturn = parser.program();
			return new JavaScriptAst(tokenStream, programReturn.Tree as CommonTree);
		}
		
		void CreateTokenStream()
		{
			var stream = new ANTLRStringStream(code);
			var lexer = new ES3Lexer(stream);
			tokenStream = new CommonTokenStream(lexer);
		}
	}
}
