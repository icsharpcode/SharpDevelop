// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
