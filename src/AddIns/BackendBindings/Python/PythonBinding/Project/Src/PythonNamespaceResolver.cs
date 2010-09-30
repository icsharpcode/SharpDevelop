// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonNamespaceResolver : IPythonResolver
	{
		PythonResolverContext resolverContext;
		ExpressionResult expressionResult;
		
		public ResolveResult Resolve(PythonResolverContext resolverContext)
		{
			return Resolve(resolverContext, resolverContext.ExpressionResult);
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
