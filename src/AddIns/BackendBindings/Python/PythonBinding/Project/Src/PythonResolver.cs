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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonResolver : IResolver, IPythonResolver
	{
		PythonResolverContext resolverContext;
		PythonImportResolver importResolver = new PythonImportResolver();
		PythonNamespaceResolver namespaceResolver = new PythonNamespaceResolver();
		PythonClassResolver classResolver = new PythonClassResolver();
		PythonStandardModuleResolver standardModuleResolver = new PythonStandardModuleResolver();
		PythonSelfResolver selfResolver = new PythonSelfResolver();
		PythonStandardModuleMethodResolver methodResolver;
		PythonMemberResolver memberResolver;
		PythonLocalVariableResolver localVariableResolver;
		PythonMethodReturnValueResolver methodReturnValueResolver;
		
		List<IPythonResolver> resolvers = new List<IPythonResolver>();
		
		public PythonResolver()
		{
			methodResolver = new PythonStandardModuleMethodResolver(standardModuleResolver);
			localVariableResolver = new PythonLocalVariableResolver(classResolver);
			memberResolver = new PythonMemberResolver(classResolver, localVariableResolver);
			methodReturnValueResolver = new PythonMethodReturnValueResolver(memberResolver);
			
			resolvers.Add(importResolver);
			resolvers.Add(classResolver);
			resolvers.Add(standardModuleResolver);
			resolvers.Add(methodReturnValueResolver);
			resolvers.Add(memberResolver);
			resolvers.Add(methodResolver);
			resolvers.Add(selfResolver);
			resolvers.Add(namespaceResolver);
			resolvers.Add(localVariableResolver);
		}
		
		public ResolveResult Resolve(ExpressionResult expressionResult, ParseInformation parseInfo, string fileContent)
		{
			if (String.IsNullOrEmpty(fileContent)) {
				return null;
			}
			
			resolverContext = new PythonResolverContext(parseInfo, expressionResult, fileContent);
			if (!resolverContext.HasProjectContent) {
				return null;
			}
			
			return Resolve(resolverContext);
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext)
		{
			foreach (IPythonResolver resolver in resolvers) {
				ResolveResult resolveResult = resolver.Resolve(resolverContext);
				if (resolveResult != null) {
					return resolveResult;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Called when Ctrl+Space is entered by the user.
		/// </summary>
		public List<ICompletionEntry> CtrlSpace(int caretLine, int caretColumn, ParseInformation parseInfo, string fileContent, ExpressionContext context)
		{
			resolverContext = new PythonResolverContext(parseInfo, fileContent);
			return CtrlSpace(resolverContext, context);
		}
		
		List<ICompletionEntry> CtrlSpace(PythonResolverContext resolverContext, ExpressionContext expressionContext)
		{
			if (resolverContext.HasProjectContent) {
				if (expressionContext == ExpressionContext.Namespace) {
					return GetImportCompletionItems(resolverContext.ProjectContent);
				}
				return resolverContext.GetImportedTypes();
			}
			return new List<ICompletionEntry>();
		}
		
		List<ICompletionEntry> GetImportCompletionItems(IProjectContent projectContent)
		{
			PythonImportCompletion importCompletion = new PythonImportCompletion(projectContent);
			return importCompletion.GetCompletionItems();
		}
	}
}
