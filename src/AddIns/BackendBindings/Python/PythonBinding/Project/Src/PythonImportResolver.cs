// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonImportResolver : IPythonResolver
	{
		public ResolveResult Resolve(PythonResolverContext resolverContext)
		{
			if (IsNamespace(resolverContext.ExpressionResult)) {
				PythonImportExpression importExpression = new PythonImportExpression(resolverContext.Expression);
				PythonImportExpressionContext context = resolverContext.ExpressionContext as PythonImportExpressionContext;
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
