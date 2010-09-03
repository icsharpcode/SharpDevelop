// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Runtime;
using IronPython.Compiler;
using IronPython.Runtime;

namespace ICSharpCode.PythonBinding
{
	public class PythonExpression
	{
		Tokenizer tokenizer;
		Token currentToken;
		
		public PythonExpression(ScriptEngine engine, string expression)
		{
			Init(engine, expression);
		}
		
		void Init(ScriptEngine engine, string expression)
		{
			PythonContext context = HostingHelpers.GetLanguageContext(engine) as PythonContext;
			SourceUnit source = CreateSourceUnit(context, expression);
			CreateTokenizer(source);
		}
		
		SourceUnit CreateSourceUnit(PythonContext context, string expression)
		{
			StringTextContentProvider textProvider = new StringTextContentProvider(expression);
			return context.CreateSourceUnit(textProvider, String.Empty, SourceCodeKind.SingleStatement);
		}
		
		void CreateTokenizer(SourceUnit source)
		{
			PythonCompilerSink sink = new PythonCompilerSink();
			PythonCompilerOptions options = new PythonCompilerOptions();
			
			tokenizer = new Tokenizer(sink, options);
			tokenizer.Initialize(source);
		}
		
		public Token GetNextToken()
		{
			currentToken = tokenizer.GetNextToken();
			return currentToken;
		}
		
		public Token CurrentToken {
			get { return currentToken; }
		}
		
		public bool IsImportToken(Token token)
		{
			return token.Kind == IronPython.Compiler.TokenKind.KeywordImport;
		}
		
		public bool IsFromToken(Token token)
		{
			return token.Kind == IronPython.Compiler.TokenKind.KeywordFrom;
		}
		
		public bool IsDotToken(Token token)
		{
			return token.Kind == IronPython.Compiler.TokenKind.Dot;
		}
		
		public bool IsNameToken(Token token)
		{
			return token.Kind == IronPython.Compiler.TokenKind.Name;
		}
	}
}
