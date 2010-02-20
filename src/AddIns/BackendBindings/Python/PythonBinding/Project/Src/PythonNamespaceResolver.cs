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
		PythonResolverContext resolverContext;
		ExpressionResult expressionResult;
		
		public PythonNamespaceResolver()
		{
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			this.resolverContext = resolverContext;
			this.expressionResult = expressionResult;
			
			if (resolverContext.HasImport(expressionResult.Expression)) {
				return ResolveFullNamespace();
			}
			return ResolvePartialNamespaceMatch();
		}
			
		ResolveResult ResolveFullNamespace()
		{
			string actualNamespace = resolverContext.UnaliasImportedModuleName(expressionResult.Expression);
			return ResolveIfNamespaceExistsInProjectReferences(actualNamespace);
		}	
		
		ResolveResult ResolvePartialNamespaceMatch()
		{
			string fullNamespace = expressionResult.Expression;
			if (resolverContext.IsStartOfDottedModuleNameImported(fullNamespace)) {
				return ResolveIfPartialNamespaceExistsInProjectReferences(fullNamespace);
			} else if (resolverContext.HasDottedImportNameThatStartsWith(fullNamespace)) {
				return CreateNamespaceResolveResult(fullNamespace);
			}
			return null;
		}
		
		ResolveResult ResolveIfNamespaceExistsInProjectReferences(string namespaceName)
		{
			if (resolverContext.NamespaceExistsInProjectReferences(namespaceName)) {
				return CreateNamespaceResolveResult(namespaceName);
			}
			return null;
		}
		
		ResolveResult ResolveIfPartialNamespaceExistsInProjectReferences(string namespaceName)
		{
			string actualNamespace = resolverContext.UnaliasStartOfDottedImportedModuleName(namespaceName);
			if (resolverContext.PartialNamespaceExistsInProjectReferences(actualNamespace)) {
				return CreateNamespaceResolveResult(actualNamespace);
			}
			return null;
		}
		
		ResolveResult CreateNamespaceResolveResult(string namespaceName)
		{
			return new NamespaceResolveResult(null, null, namespaceName);
		}
	}
}
