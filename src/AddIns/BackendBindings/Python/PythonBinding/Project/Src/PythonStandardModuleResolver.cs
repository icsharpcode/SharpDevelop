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
	public class PythonStandardModuleResolver : IPythonResolver
	{
		PythonStandardModules standardPythonModules = new PythonStandardModules();
		
		public PythonStandardModuleResolver()
		{
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			PythonStandardModuleType type = GetStandardModuleTypeIfImported(resolverContext, expressionResult.Expression);
			if (type != null) {
				return new PythonStandardModuleResolveResult(type);
			}
			return null;
		}
		
		public PythonStandardModuleType GetStandardModuleTypeIfImported(PythonResolverContext resolverContext, string moduleName)
		{
			if (resolverContext.HasImport(moduleName) || PythonBuiltInModuleMemberName.IsBuiltInModule(moduleName)) {
				string actualModuleName = resolverContext.UnaliasImportedModuleName(moduleName);
				return standardPythonModules.GetModuleType(actualModuleName);
			}
			return null;
		}
		
		public PythonStandardModuleType GetStandardModuleType(string moduleName)
		{
			return standardPythonModules.GetModuleType(moduleName);
		}
	}
}
