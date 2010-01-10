// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using IronPython.Modules;
using IronPython.Runtime;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents the standard library modules that are implemented in
	/// IronPython.
	/// </summary>
	public class PythonStandardModules : ReadOnlyCollectionBase
	{
		List<string> moduleNames = new List<string>();
		Dictionary<string, Type> moduleTypes = new Dictionary<string, Type>();
		
		public PythonStandardModules()
		{
			GetPythonStandardModuleNames();
			InnerList.AddRange(GetNames());
		}
		
		void GetPythonStandardModuleNames()
		{
			GetPythonModuleNamesFromAssembly(typeof(Builtin).Assembly);
			GetPythonModuleNamesFromAssembly(typeof(ModuleOps).Assembly);
		}
		
		/// <summary>
		/// Gets the names of the standard Python library modules.
		/// </summary>
		public string[] GetNames()
		{
			return moduleNames.ToArray();
		}
		
		void GetPythonModuleNamesFromAssembly(Assembly assembly)
		{
			foreach (Attribute attribute in Attribute.GetCustomAttributes(assembly, typeof(PythonModuleAttribute))) {
				PythonModuleAttribute pythonModuleAttribute = attribute as PythonModuleAttribute;
				moduleNames.Add(pythonModuleAttribute.Name);
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
