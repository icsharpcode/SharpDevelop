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
