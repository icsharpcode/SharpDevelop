// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonSelfResolver : IPythonResolver
	{
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			if (resolverContext.HasCallingClass) {
				if (IsSelfExpression(expressionResult)) {
					return CreateResolveResult(resolverContext);
				}
			}
			return null;
		}
		
		bool IsSelfExpression(ExpressionResult expressionResult)
		{
			return expressionResult.Expression == "self";
		}
		
		ResolveResult CreateResolveResult(PythonResolverContext resolverContext)
		{
			IClass callingClass = resolverContext.CallingClass;
			IReturnType returnType = callingClass.DefaultReturnType;
			return new ResolveResult(callingClass, null, returnType);
		}
	}
}
