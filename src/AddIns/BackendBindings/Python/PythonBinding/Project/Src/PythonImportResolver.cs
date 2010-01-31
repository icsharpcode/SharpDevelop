// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
