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
	public class PythonClassResolver : IPythonResolver
	{
		PythonResolverContext resolverContext;
		
		public ResolveResult Resolve(PythonResolverContext resolverContext)
		{
			IClass matchingClass = GetClass(resolverContext);
			if (matchingClass != null) {
				return CreateTypeResolveResult(matchingClass);
			}
			return null;
		}
		
		public IClass GetClass(PythonResolverContext resolverContext)
		{
			string name = resolverContext.Expression;
			return GetClass(resolverContext, name);
		}
		
		public IClass GetClass(PythonResolverContext resolverContext, string name)
		{
			this.resolverContext = resolverContext;
			
			if (String.IsNullOrEmpty(name)) {
				return null;
			}
			
			IClass matchedClass = resolverContext.GetClass(name);
			if (matchedClass != null) {
				return matchedClass;
			}
			
			matchedClass = GetClassFromImportedNames(name);
			if (matchedClass != null) {
				return matchedClass;
			}
			
			matchedClass = GetClassFromNamespaceThatImportsEverything(name);
			if (matchedClass != null) {
				return matchedClass;
			}
			
			return GetClassFromDottedImport(name);
		}
		
		TypeResolveResult CreateTypeResolveResult(IClass c)
		{
			return new TypeResolveResult(null, null, c);
		}
		
		IClass GetClassFromImportedNames(string name)
		{
			string moduleName = resolverContext.GetModuleForImportedName(name);
			if (moduleName != null) {
				name = resolverContext.UnaliasImportedName(name);
				string fullyQualifiedName = GetQualifiedClassName(moduleName, name);
				return resolverContext.GetClass(fullyQualifiedName);
			}
			return null;
		}
		
		string GetQualifiedClassName(string namespacePrefix, string className)
		{
			return namespacePrefix + "." + className;
		}
		
		IClass GetClassFromNamespaceThatImportsEverything(string name)
		{
			foreach (string moduleName in resolverContext.GetModulesThatImportEverything()) {
				string fullyQualifiedName = GetQualifiedClassName(moduleName, name);
				IClass matchedClass = resolverContext.GetClass(fullyQualifiedName);
				if (matchedClass != null) {
					return matchedClass;
				}
			}
			return null;
		}
		
		IClass GetClassFromDottedImport(string name)
		{
			string moduleName = resolverContext.FindStartOfDottedModuleNameInImports(name);
			if (moduleName != null) {
				string fullyQualifiedName = UnaliasClassName(moduleName, name);
				return resolverContext.GetClass(fullyQualifiedName);
			}
			return null;
		}
		
		string UnaliasClassName(string moduleName, string fullClassName)
		{
			string actualModuleName = resolverContext.UnaliasImportedModuleName(moduleName);
			string lastPartOfClassName = fullClassName.Substring(moduleName.Length + 1);
			return GetQualifiedClassName(actualModuleName, lastPartOfClassName);
		}
	}
}
