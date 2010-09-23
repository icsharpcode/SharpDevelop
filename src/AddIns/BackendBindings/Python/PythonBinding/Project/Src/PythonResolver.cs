// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		PythonMethodResolver methodResolver;
		PythonMemberResolver memberResolver;
		PythonLocalVariableResolver localVariableResolver = new PythonLocalVariableResolver();
		
		List<IPythonResolver> resolvers = new List<IPythonResolver>();
		
		public PythonResolver()
		{
			methodResolver = new PythonMethodResolver(classResolver, standardModuleResolver);
			memberResolver = new PythonMemberResolver(classResolver);
			
			resolvers.Add(importResolver);
			resolvers.Add(classResolver);
			resolvers.Add(standardModuleResolver);
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
			
			resolverContext = new PythonResolverContext(parseInfo, fileContent);
			if (!resolverContext.GetCallingMember(expressionResult.Region)) {
				return null;
			}
			
			return Resolve(resolverContext, expressionResult);
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			foreach (IPythonResolver resolver in resolvers) {
				ResolveResult resolveResult = resolver.Resolve(resolverContext, expressionResult);
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
