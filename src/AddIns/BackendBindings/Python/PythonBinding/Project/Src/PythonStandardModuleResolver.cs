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
	public class PythonStandardModuleResolver
	{
		public const string PythonBuiltInModuleName = "__builtin__";
		PythonStandardModules standardPythonModules = new PythonStandardModules();
		
		public PythonStandardModuleResolver()
		{
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			PythonStandardModuleType type = GetStandardModuleType(resolverContext, expressionResult.Expression);
			if (type != null) {
				return new PythonStandardModuleResolveResult(type);
			}
			return null;
		}
		
		public PythonStandardModuleType GetStandardModuleType(PythonResolverContext resolverContext, string name)
		{
			if (resolverContext.HasImport(name) || IsBuiltInModule(name)) {
				return standardPythonModules.GetModuleType(name);
			}
			return null;
		}
		
		bool IsBuiltInModule(string name)
		{
			return name == PythonBuiltInModuleName;
		}
	}
}
