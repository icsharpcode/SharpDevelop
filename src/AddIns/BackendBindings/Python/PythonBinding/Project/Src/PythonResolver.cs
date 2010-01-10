// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonResolver : IResolver
	{
		PythonResolverContext resolverContext;
		PythonNamespaceResolver namespaceResolver = new PythonNamespaceResolver();
		PythonClassResolver classResolver = new PythonClassResolver();
		PythonStandardModuleResolver standardModuleResolver = new PythonStandardModuleResolver();
		PythonMethodResolver methodResolver;
		
		public PythonResolver()
		{
			 methodResolver = new PythonMethodResolver(classResolver, standardModuleResolver);
		}
		
		public ResolveResult Resolve(ExpressionResult expressionResult, ParseInformation parseInfo, string fileContent)
		{
			if (String.IsNullOrEmpty(fileContent)) {
				return null;
			}
			
			resolverContext = new PythonResolverContext(parseInfo);
			if (!resolverContext.GetCallingMember(expressionResult.Region)) {
				return null;
			}
			
			ResolveResult resolveResult = namespaceResolver.Resolve(expressionResult);
			if (resolveResult != null) {
				return resolveResult;
			}
			
			resolveResult = classResolver.Resolve(resolverContext, expressionResult);
			if (resolveResult != null) {
				return resolveResult;
			}
			
			resolveResult = standardModuleResolver.Resolve(resolverContext, expressionResult);
			if (resolveResult != null) {
				return resolveResult;
			}
			
			resolveResult = methodResolver.Resolve(resolverContext, expressionResult);
			if (resolveResult != null) {
				return resolveResult;
			}
			
			// Search for a local variable.
			LocalResolveResult localResolveResult = GetLocalVariable(expressionResult.Expression, parseInfo.BestCompilationUnit.FileName, fileContent);
			if (localResolveResult != null) {
				return localResolveResult;
			}
			
			// Search for a namespace.
			if (resolverContext.NamespaceExists(expressionResult.Expression)) {
				return new NamespaceResolveResult(null, null, expressionResult.Expression);
			}
			return null;
		}
		
		/// <summary>
		/// Called when Ctrl+Space is entered by the user.
		/// </summary>
		public ArrayList CtrlSpace(int caretLine, int caretColumn, ParseInformation parseInfo, string fileContent, ExpressionContext context)
		{
			resolverContext = new PythonResolverContext(parseInfo);
			if (resolverContext.HasProjectContent) {
				if (context == ExpressionContext.Namespace) {
					PythonImportCompletion importCompletion = new PythonImportCompletion(resolverContext.ProjectContent);
					return importCompletion.GetCompletionItems();
				} else {
					return resolverContext.GetImportedTypes();
				}
			}
			return new ArrayList();
		}
		
		/// <summary>
		/// Tries to find the type that matches the local variable name.
		/// </summary>
		LocalResolveResult GetLocalVariable(string expression, string fileName, string fileContent)
		{
//			PythonVariableResolver resolver = new PythonVariableResolver();
//			string typeName = resolver.Resolve(expression, fileName, fileContent);
//			if (typeName != null) {
//				IClass resolvedClass = GetClass(typeName);
//				if (resolvedClass != null) {
//					DefaultClass dummyClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "Global");
//					DefaultMethod dummyMethod = new DefaultMethod(dummyClass, String.Empty);
//					DefaultField.LocalVariableField field = new DefaultField.LocalVariableField(resolvedClass.DefaultReturnType, expression, DomRegion.Empty, dummyClass);
//					return new LocalResolveResult(dummyMethod, field);
//				}
//			}
			return null;
		}
	}
}
