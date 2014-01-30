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
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using ICSharpCode.SharpDevelop.Dom;
using Xebic.Parsers.ES3;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptMethodRegion
	{
		JavaScriptAst ast;
		CommonTree methodDefinition;
		IToken functionDefinitionEndToken;
		
		public JavaScriptMethodRegion(JavaScriptAst ast, CommonTree methodDefinition)
		{
			this.ast = ast;
			this.methodDefinition = methodDefinition;
		}
		
		public DomRegion GetHeaderRegion()
		{
			int beginLine = methodDefinition.Line;
			int endLine = beginLine; //TODO - function header definition can go across lines.
			int beginColumn = GetMethodHeaderBeginColumn();
			int endColumn = GetMethodHeaderEndColumn();
			return new DomRegion(beginLine, beginColumn, endLine, endColumn);
		}
		
		int GetMethodHeaderBeginColumn()
		{
			return methodDefinition.BeginColumn();
		}
		
		int GetMethodHeaderEndColumn()
		{
			CommonTree arguments = methodDefinition.GetFirstChildArguments();
			if (arguments != null) {
				IToken token = ast.GetToken(arguments.TokenStopIndex);
				return token.EndColumn();
			}
			return 0;
		}
		
		public DomRegion GetBodyRegion()
		{
			int beginLine = methodDefinition.Line;
			int endLine = GetMethodBodyEndLine();
			int beginColumn = GetMethodHeaderEndColumn();
			int endColumn = GetMethodBodyEndColumn();
			return new DomRegion(beginLine, beginColumn, endLine, endColumn);
		}
		
		int GetMethodBodyEndLine()
		{
			IToken token = GetFunctionDefinitionEndToken();
			return token.Line;
		}
		
		IToken GetFunctionDefinitionEndToken()
		{
			if (functionDefinitionEndToken == null) {
				functionDefinitionEndToken = ast.GetToken(methodDefinition.TokenStopIndex);
			}
			return functionDefinitionEndToken;
		}
		
		int GetMethodBodyEndColumn()
		{
			IToken token = GetFunctionDefinitionEndToken();
			return token.EndColumn();
		}
	}
}
