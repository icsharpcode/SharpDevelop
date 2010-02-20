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
	public class PythonClassResolver : IPythonResolver
	{
		PythonResolverContext resolverContext;
		
		public PythonClassResolver()
		{
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			IClass matchingClass = GetClass(resolverContext, expressionResult.Expression);
			if (matchingClass != null) {
				return CreateTypeResolveResult(matchingClass);
			}
			return null;
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
