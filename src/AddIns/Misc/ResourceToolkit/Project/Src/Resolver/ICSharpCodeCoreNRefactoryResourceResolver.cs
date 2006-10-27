// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Detects and resolves resource references when using the ICSharpCode.Core
	/// ResourceService class.
	/// </summary>
	public class ICSharpCodeCoreNRefactoryResourceResolver : INRefactoryResourceResolver
	{
		
		/// <summary>
		/// Tries to find a resource reference in the specified expression.
		/// </summary>
		/// <param name="expressionResult">The ExpressionResult for the expression.</param>
		/// <param name="expr">The AST representation of the full expression.</param>
		/// <param name="resolveResult">SharpDevelop's ResolveResult for the expression.</param>
		/// <param name="caretLine">The line where the expression is located.</param>
		/// <param name="caretColumn">The column where the expression is located.</param>
		/// <param name="fileName">The name of the source file where the expression is located.</param>
		/// <param name="fileContent">The content of the source file where the expression is located.</param>
		/// <returns>A ResourceResolveResult describing the referenced resource, or <c>null</c>, if this expression does not reference a resource using the ICSharpCode.Core.ResourceService class.</returns>
		public ResourceResolveResult Resolve(ExpressionResult expressionResult, Expression expr, ResolveResult resolveResult, int caretLine, int caretColumn, string fileName, string fileContent)
		{
			IMember member = null;
			
			// "ResourceService.GetString(..." may be a MemberResolveResult or
			// MethodResolveResult, dependent on how much of the expression
			// has already been typed.
			MemberResolveResult mrr = resolveResult as MemberResolveResult;
			if (mrr != null) {
				member = mrr.ResolvedMember;
			} else {
				MethodResolveResult methrr = resolveResult as MethodResolveResult;
				if (methrr != null) {
					member = methrr.GetMethodIfSingleOverload();
				}
			}
			
			if (member is IMethod &&
			    LanguageProperties.CSharp.NameComparer.Equals(member.FullyQualifiedName, "ICSharpCode.Core.ResourceService.GetString")
			   ) {
				
				#if DEBUG
				LoggingService.Debug("ResourceToolkit: ICSharpCodeCoreNRefactoryResourceResolver: ResourceService resource access detected");
				#endif
				
				string key = GetKeyFromExpression(expr);
				string localResourceFileName = ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreLocalResourceFileName(fileName);
				string hostResourceFileName = ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreHostResourceFileName(fileName);
				IResourceFileContent content = null;
				
				// Merge the local and host resource file contents if available.
				
				if (!String.IsNullOrEmpty(localResourceFileName)) {
					content = ResourceFileContentRegistry.GetResourceFileContent(localResourceFileName);
				}
				
				if (!String.IsNullOrEmpty(hostResourceFileName)) {
					if (content == null) {
						content = ResourceFileContentRegistry.GetResourceFileContent(hostResourceFileName);
					} else {
						IResourceFileContent hostContent = ResourceFileContentRegistry.GetResourceFileContent(hostResourceFileName);
						if (hostContent != null) {
							content = new MergedResourceFileContent(content, new IResourceFileContent[] { hostContent });
						}
					}
				}
				
				if (content != null) {
					// TODO: Add information about return type (of the resource, if present).
					return new ResourceResolveResult(resolveResult.CallingClass, resolveResult.CallingMember, null, content, key);
				}
				
			}
			
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Tries to infer the resource key being referenced from the given expression.
		/// </summary>
		static string GetKeyFromExpression(Expression expr)
		{
			#if DEBUG
			LoggingService.Debug("ResourceToolkit: ICSharpCodeCoreNRefactoryResourceResolver trying to get key from expression: "+expr.ToString());
			#endif
			
			InvocationExpression invocation = expr as InvocationExpression;
			if (invocation != null) {
				FieldReferenceExpression fre = invocation.TargetObject as FieldReferenceExpression;
				if (fre != null) {
					if (fre.FieldName == "GetString") {
						if (invocation.Arguments.Count > 0) {
							PrimitiveExpression p = invocation.Arguments[0] as PrimitiveExpression;
							if (p != null) {
								string key = p.Value as string;
								if (key != null) {
									#if DEBUG
									LoggingService.Debug("ResourceToolkit: ICSharpCodeCoreNRefactoryResourceResolver found key: "+key);
									#endif
									return key;
								}
							}
						}
					}
				}
			}
			
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Gets a list of patterns that can be searched for in the specified file
		/// to find possible resource references that are supported by this
		/// resolver.
		/// </summary>
		/// <param name="fileName">The name of the file to get a list of possible patterns for.</param>
		public IEnumerable<string> GetPossiblePatternsForFile(string fileName)
		{
			return new string[] {
				"GetString"
			};
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ICSharpCodeCoreNRefactoryResourceResolver"/> class.
		/// </summary>
		public ICSharpCodeCoreNRefactoryResourceResolver()
		{
		}
	}
}
