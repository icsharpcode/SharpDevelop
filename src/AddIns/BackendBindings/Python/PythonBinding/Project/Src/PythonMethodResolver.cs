// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonMethodResolver : IPythonResolver
	{
		PythonDotNetMethodResolver dotNetMethodResolver;
		PythonStandardModuleMethodResolver standardModuleMethodResolver;
		
		public PythonMethodResolver(PythonClassResolver classResolver, PythonStandardModuleResolver standardModuleResolver)
		{
			dotNetMethodResolver = new PythonDotNetMethodResolver(classResolver);
			standardModuleMethodResolver = new PythonStandardModuleMethodResolver(standardModuleResolver);
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext)
		{
			ResolveResult resolveResult = dotNetMethodResolver.Resolve(resolverContext);
			if (resolveResult != null) {
				return resolveResult;
			}
			return standardModuleMethodResolver.Resolve(resolverContext);
		}
	}
}
