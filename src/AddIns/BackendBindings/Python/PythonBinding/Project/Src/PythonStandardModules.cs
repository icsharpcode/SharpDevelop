// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Modules;
using IronPython.Runtime;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents the standard library modules that are implemented in
	/// IronPython.
	/// </summary>
	public class PythonStandardModules : List<ICompletionEntry>
	{
		Dictionary<string, Type> moduleTypes = new Dictionary<string, Type>();
		
		public PythonStandardModules()
		{
			GetPythonStandardModuleNames();
		}
		
		void GetPythonStandardModuleNames()
		{
			GetPythonModuleNamesFromAssembly(typeof(Builtin).Assembly);
			GetPythonModuleNamesFromAssembly(typeof(ModuleOps).Assembly);
		}
		
		void GetPythonModuleNamesFromAssembly(Assembly assembly)
		{
			foreach (Attribute attribute in Attribute.GetCustomAttributes(assembly, typeof(PythonModuleAttribute))) {
				PythonModuleAttribute pythonModuleAttribute = attribute as PythonModuleAttribute;
				Add(new NamespaceEntry(pythonModuleAttribute.Name));
				moduleTypes.Add(pythonModuleAttribute.Name, pythonModuleAttribute.Type);
			}
		}
		
		public PythonStandardModuleType GetModuleType(string moduleName)
		{
			Type type = GetTypeForModule(moduleName);
			if (type != null) {
				return new PythonStandardModuleType(type, moduleName);
			}
			return null;
		}
		
		Type GetTypeForModule(string moduleName)
		{
			Type type;
			if (moduleTypes.TryGetValue(moduleName, out type)) {
				return type;
			}
			return null;
		}
	}
}
