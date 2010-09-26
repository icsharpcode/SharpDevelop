// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonDotNetMethodResolver
	{
		PythonClassResolver classResolver;
		
		public PythonDotNetMethodResolver(PythonClassResolver classResolver)
		{
			this.classResolver = classResolver;
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext)
		{
			MemberName memberName = resolverContext.CreateExpressionMemberName();
			IClass matchingClass = classResolver.GetClass(resolverContext, memberName.Type);
			if (matchingClass != null) {
				return new PythonMethodGroupResolveResult(matchingClass, memberName.Name);
			}
			return null;
		}
	}
}
