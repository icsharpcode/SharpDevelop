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
	public class PythonNamespaceResolver : IPythonResolver
	{
		public PythonNamespaceResolver()
		{
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			if (!resolverContext.HasImport(expressionResult.Expression)) {
				return null;
			}
			
			string actualNamespace = resolverContext.UnaliasImportedModuleName(expressionResult.Expression);
			return CreateNamespaceResolveResultIfNamespaceExists(resolverContext, actualNamespace);
		}
		
		ResolveResult CreateNamespaceResolveResultIfNamespaceExists(PythonResolverContext resolverContext, string namespaceName)
		{
			if (resolverContext.NamespaceExistsInProjectReferences(namespaceName)) {
				return new NamespaceResolveResult(null, null, namespaceName);
			}
			return null;
		}
	}
}
