// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonStandardModuleMethodResolver : IPythonResolver
	{
		PythonStandardModuleResolver standardModuleResolver;
		
		public PythonStandardModuleMethodResolver(PythonStandardModuleResolver standardModuleResolver)
		{
			this.standardModuleResolver = standardModuleResolver;
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext)
		{
			MemberName memberName = resolverContext.CreateExpressionMemberName();
			MethodGroupResolveResult result = ResolveMethodFromImportedNames(resolverContext, memberName);
			if (result != null) {
				return result;
			}
			result = ResolveIfMethodIsImported(resolverContext, memberName);
			if (result != null) {
				return result;
			}
			return ResolveMethodFromModulesThatImportEverything(resolverContext, memberName);
		}
		
		MethodGroupResolveResult ResolveMethodFromImportedNames(PythonResolverContext resolverContext, MemberName memberName)
		{
			if (!memberName.HasName) {
				string name = memberName.Type;
				string moduleName = resolverContext.GetModuleForImportedName(name);
				if (moduleName != null) {
					PythonStandardModuleType type = standardModuleResolver.GetStandardModuleType(moduleName);
					if (type != null) {
						name = resolverContext.UnaliasImportedName(name);
						return PythonStandardModuleMethodGroupResolveResult.Create(type, name);
					}
				}
			}
			return null;
		}
		
		MethodGroupResolveResult ResolveIfMethodIsImported(PythonResolverContext resolverContext, MemberName memberName)
		{
			if (!memberName.HasName) {
				memberName = new PythonBuiltInModuleMemberName(memberName.Type);
			}
			
			PythonStandardModuleType type = standardModuleResolver.GetStandardModuleTypeIfImported(resolverContext, memberName.Type);
			if (type != null) {
				return CreateResolveResult(type, memberName.Name);
			}
			return null;
		}
		
		MethodGroupResolveResult ResolveMethodFromModulesThatImportEverything(PythonResolverContext resolverContext, MemberName memberName)
		{
			foreach (string module in resolverContext.GetModulesThatImportEverything()) {
				PythonStandardModuleType type = standardModuleResolver.GetStandardModuleType(module);
				if (type != null) {
					MethodGroupResolveResult resolveResult = CreateResolveResult(type, memberName.Type);
					if (resolveResult != null) {
						return resolveResult;
					}
				}
			}
			return null;
		}
		
		MethodGroupResolveResult CreateResolveResult(PythonStandardModuleType type, string name)
		{
			return PythonStandardModuleMethodGroupResolveResult.Create(type, name);
		}
	}
}
