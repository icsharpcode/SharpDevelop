// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
