// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
