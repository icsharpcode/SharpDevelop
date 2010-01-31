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
	public class PythonMethodResolver : IPythonResolver
	{
		PythonDotNetMethodResolver dotNetMethodResolver;
		PythonStandardModuleMethodResolver standardModuleMethodResolver;
		
		public PythonMethodResolver(PythonClassResolver classResolver, PythonStandardModuleResolver standardModuleResolver)
		{
			dotNetMethodResolver = new PythonDotNetMethodResolver(classResolver);
			standardModuleMethodResolver = new PythonStandardModuleMethodResolver(standardModuleResolver);
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			ResolveResult resolveResult = dotNetMethodResolver.Resolve(resolverContext, expressionResult);
			if (resolveResult != null) {
				return resolveResult;
			}
			return standardModuleMethodResolver.Resolve(resolverContext, expressionResult);
		}
	}
}
