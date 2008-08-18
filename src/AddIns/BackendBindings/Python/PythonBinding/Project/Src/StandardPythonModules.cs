// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
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
	public class StandardPythonModules
	{
		public StandardPythonModules()
		{
		}
				
		/// <summary>
		/// Gets the names of the standard Python library modules.
		/// </summary>
		public string[] GetNames()
		{
			List<string> names = new List<string>();
			names.Add("sys");
			Assembly assembly = typeof(Builtin).Assembly;
			foreach (Attribute customAttribute in Attribute.GetCustomAttributes(assembly, typeof(PythonModuleAttribute))) {
				PythonModuleAttribute pythonModuleAttribute = customAttribute as PythonModuleAttribute;
				names.Add(pythonModuleAttribute.Name);
			}
			return names.ToArray();
		}
	}
}
