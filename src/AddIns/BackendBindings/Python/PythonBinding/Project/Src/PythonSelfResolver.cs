// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonSelfResolver : IPythonResolver
	{
		public ResolveResult Resolve(PythonResolverContext resolverContext)
		{
			if (resolverContext.HasCallingClass) {
				if (IsSelfExpression(resolverContext)) {
					return CreateResolveResult(resolverContext);
				} else if (IsSelfExpressionAtStart(resolverContext)) {
					MemberName memberName = resolverContext.CreateExpressionMemberName();
					return new PythonMethodGroupResolveResult(resolverContext.CallingClass, memberName.Name);
				}
			}
			return null;
		}
		
		public static bool IsSelfExpression(string expression)
		{
			return expression == "self";
		}
		
		bool IsSelfExpression(PythonResolverContext resolverContext)
		{
			return IsSelfExpression(resolverContext.Expression);
		}
		
		ResolveResult CreateResolveResult(PythonResolverContext resolverContext)
		{
			IClass callingClass = resolverContext.CallingClass;
			IReturnType returnType = callingClass.DefaultReturnType;
			return new ResolveResult(callingClass, null, returnType);
		}
		
		bool IsSelfExpressionAtStart(PythonResolverContext resolverContext)
		{
			return resolverContext.Expression.StartsWith("self.");
		}
	}
}
