// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonMethodReturnValueResolver : IPythonResolver
	{
		PythonMemberResolver memberResolver;
		
		public PythonMethodReturnValueResolver(PythonMemberResolver memberResolver)
		{
			this.memberResolver = memberResolver;
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext)
		{
			string methodName = GetMethodName(resolverContext.Expression);
			if (methodName != null) {
				PythonResolverContext newResolverContext = resolverContext.Clone(methodName);
				IMember member = memberResolver.FindMember(newResolverContext);
				return CreateResolveResult(member);
			}
			return null;
		}

		string GetMethodName(string expression)
		{
			int methodParametersStartIndex = expression.IndexOf('(');
			if ((methodParametersStartIndex > 0) && expression.EndsWith(")")) {
				return expression.Substring(0, methodParametersStartIndex);
			}
			return null;
		}
		
		MemberResolveResult CreateResolveResult(IMember member)
		{
			if (member != null) {
				return new MemberResolveResult(null, null, member);
			}
			return null;
		}
	}
}
