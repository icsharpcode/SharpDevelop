// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonStandardModuleResolver : IPythonResolver
	{
		PythonStandardModules standardPythonModules = new PythonStandardModules();
		
		public ResolveResult Resolve(PythonResolverContext resolverContext)
		{
			PythonStandardModuleType type = GetStandardModuleTypeIfImported(resolverContext);
			if (type != null) {
				return new PythonStandardModuleResolveResult(type);
			}
			return null;
		}
		
		PythonStandardModuleType GetStandardModuleTypeIfImported(PythonResolverContext resolverContext)
		{
			string moduleName = resolverContext.Expression;
			return GetStandardModuleTypeIfImported(resolverContext, moduleName);
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
