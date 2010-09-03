// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonImportResolver : IPythonResolver
	{
		public PythonImportResolver()
		{
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			if (IsNamespace(expressionResult)) {
				PythonImportExpression importExpression = new PythonImportExpression(expressionResult.Expression);
				PythonImportExpressionContext context = expressionResult.Context as PythonImportExpressionContext;
				context.HasFromAndImport = importExpression.HasFromAndImport;
				
				return new PythonImportModuleResolveResult(importExpression);
			}
			return null;
		}
		
		bool IsNamespace(ExpressionResult expressionResult)
		{
			return expressionResult.Context is PythonImportExpressionContext;
		}
	}
}
