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
	public class PythonMethodResolver
	{
		PythonClassResolver classResolver;
		PythonStandardModuleResolver standardModuleResolver;
		
		public PythonMethodResolver(PythonClassResolver classResolver, PythonStandardModuleResolver standardModuleResolver)
		{
			this.classResolver = classResolver;
			this.standardModuleResolver = standardModuleResolver;
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			MemberName memberName = new MemberName(expressionResult.Expression);
			MethodGroupResolveResult resolveResult = GetDotNetMethodResolveResult(resolverContext, memberName);
			if (resolveResult != null) {
				return resolveResult;
			}
			return GetStandardModuleMethodResolveResult(resolverContext, memberName);
		}
		
		MethodGroupResolveResult GetDotNetMethodResolveResult(PythonResolverContext resolverContext, MemberName memberName)
		{
			IClass matchingClass = classResolver.GetClass(resolverContext, memberName.Type);
			if (matchingClass != null) {
				return CreateMethodGroupResolveResult(matchingClass, memberName.Name);
			}
			return null;
		}
		
		MethodGroupResolveResult GetStandardModuleMethodResolveResult(PythonResolverContext resolverContext, MemberName memberName)
		{
			MethodGroupResolveResult result = GetStandardModuleMethodResolveResultFromImportedIdentifiers(resolverContext, memberName);
			if (result != null) {
				return result;
			}
			return GetStandardModuleMethodResolveResultIfFromImports(resolverContext, memberName);
		}
		
		MethodGroupResolveResult GetStandardModuleMethodResolveResultFromImportedIdentifiers(PythonResolverContext resolverContext, MemberName memberName)
		{
			if (!memberName.HasName) {
				string identifier = memberName.Type;
				string moduleName = resolverContext.GetModuleForIdentifier(identifier);
				if (moduleName != null) {
					PythonStandardModuleType type = standardModuleResolver.GetStandardModuleType(moduleName);
					if (type != null) {
						identifier = resolverContext.UnaliasIdentifier(identifier);
						return GetStandardModuleMethodResolveResult(type, identifier);
					}
				}
			}
			return null;
		}
		
		MethodGroupResolveResult GetStandardModuleMethodResolveResultIfFromImports(PythonResolverContext resolverContext, MemberName memberName)
		{
			if (!memberName.HasName) {
				memberName = CreateBuiltinModuleMemberName(memberName.Type);
			}
			
			PythonStandardModuleType type = standardModuleResolver.GetStandardModuleTypeIfImported(resolverContext, memberName.Type);
			if (type != null) {
				return GetStandardModuleMethodResolveResult(type, memberName.Name);
			}
			return null;
		}
		
		MemberName CreateBuiltinModuleMemberName(string memberName)
		{
			return new MemberName(PythonStandardModuleResolver.PythonBuiltInModuleName, memberName);
		}
		
		MethodGroupResolveResult GetStandardModuleMethodResolveResult(PythonStandardModuleType type, string methodName)
		{
			PythonModuleCompletionItems completionItems = PythonModuleCompletionItemsFactory.Create(type);
			MethodGroup methods = completionItems.GetMethods(methodName);
			if (methods.Count > 0) {
				return CreateMethodGroupResolveResult(methods);
			}
			return null;
		}
		
		MethodGroupResolveResult CreateMethodGroupResolveResult(IClass c, string methodName)
		{
			return new MethodGroupResolveResult(null, null, c.DefaultReturnType, methodName);
		}
		
		MethodGroupResolveResult CreateMethodGroupResolveResult(MethodGroup methods)
		{
			MethodGroup[] methodGroups = new MethodGroup[] { methods };
			IMethod method = methods[0];
			IReturnType returnType = new DefaultReturnType(method.DeclaringType);
			return new MethodGroupResolveResult(null, null, returnType, method.Name, methodGroups);
		}
	}
}
