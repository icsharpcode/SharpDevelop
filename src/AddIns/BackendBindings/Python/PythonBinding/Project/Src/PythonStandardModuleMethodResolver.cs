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
