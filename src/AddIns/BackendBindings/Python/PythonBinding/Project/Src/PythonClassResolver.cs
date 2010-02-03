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
		public PythonClassResolver()
		{
		}
		
		public ResolveResult Resolve(PythonResolverContext context, ExpressionResult expressionResult)
		{
			IClass matchingClass = GetClass(context, expressionResult.Expression);
			if (matchingClass != null) {
				return CreateTypeResolveResult(matchingClass);
			}
			return null;
		}
		
		public IClass GetClass(PythonResolverContext context, string name)
		{
			if (String.IsNullOrEmpty(name)) {
				return null;
			}
			
			IClass matchedClass = context.GetClass(name);
			if (matchedClass != null) {
				return matchedClass;
			}
			
			matchedClass = GetClassFromImportedNames(context, name);
			if (matchedClass != null) {
				return matchedClass;
			}
			
			matchedClass = GetClassFromNamespaceThatImportsEverything(context, name);
			if (matchedClass != null) {
				return matchedClass;
			}
			return null;
		}
		
		TypeResolveResult CreateTypeResolveResult(IClass c)
		{
			return new TypeResolveResult(null, null, c);
		}
		
		IClass GetClassFromImportedNames(PythonResolverContext context, string name)
		{
			string moduleName = context.GetModuleForImportedName(name);
			if (moduleName != null) {
				name = context.UnaliasImportedName(name);
				string fullyQualifiedName = GetQualifiedClassName(moduleName, name);
				return context.GetClass(fullyQualifiedName);
			}
			return null;
		}
		
		string GetQualifiedClassName(string namespacePrefix, string className)
		{
			return namespacePrefix + "." + className;
		}
		
		IClass GetClassFromNamespaceThatImportsEverything(PythonResolverContext context, string name)
		{
			foreach (string moduleName in context.GetModulesThatImportEverything()) {
				string fullyQualifiedName = GetQualifiedClassName(moduleName, name);
				IClass matchedClass = context.GetClass(fullyQualifiedName);
				if (matchedClass != null) {
					return matchedClass;
				}
			}
			return null;
		}
	}
}
