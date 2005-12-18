// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using ICSharpCode.Core;

namespace ICSharpCode.FormsDesigner.Services
{
	public class TypeResolutionService : ITypeResolutionService
	{
		public Assembly GetAssembly(AssemblyName name)
		{
			return GetAssembly(name, false);
		}
		
		public Assembly GetAssembly(AssemblyName name, bool throwOnError)
		{
			return Assembly.Load(name);
		}
			
		public string GetPathOfAssembly(AssemblyName name)
		{
			Assembly assembly = GetAssembly(name);
			if (assembly != null) {
				return assembly.Location;
			}
			return null;
		}
		
		public Type GetType(string name)
		{
			return GetType(name, false);
		}
		
		public Type GetType(string name, bool throwOnError)
		{
			return GetType(name, throwOnError, false);
		}
		
		public Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			if (name == null || name.Length == 0) {
				return null;
			}
			if (IgnoreType(name)) {
				return null;
			}
			try {
				Assembly lastAssembly = null;
				foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
					Type t = asm.GetType(name, throwOnError);
					if (t != null) {
						lastAssembly = asm;
					}
				}
				if (lastAssembly != null) {
					return lastAssembly.GetType(name, throwOnError, ignoreCase);
				}
				
				Type type = Type.GetType(name, throwOnError, ignoreCase);
				
				// type lookup for typename, assembly, xyz style lookups
				if (type == null) {
					int idx = name.IndexOf(",");
					if (idx > 0) {
						string[] splitName = name.Split(',');
						string typeName     = splitName[0];
						string assemblyName = splitName[1].Substring(1);
						Assembly assembly = null;
						try {
							assembly = Assembly.Load(assemblyName);
						} catch (Exception e) {
							Console.WriteLine(e);
						}
						if (assembly != null) {
							type = assembly.GetType(typeName, throwOnError, ignoreCase);
						} else {
							type = Type.GetType(typeName, throwOnError, ignoreCase);
						}
					}
				}
				
				return type;
			} catch (Exception e) {
				Console.WriteLine(e); 
			}
			return null;
		}
		
		public void ReferenceAssembly(AssemblyName name)
		{
			ICSharpCode.Core.LoggingService.Warn("TODO: Add Assembly reference : " + name);
		}
		
		/// <summary>
		/// HACK - Ignore any requests for types from the Microsoft.VSDesigner
		/// assembly.  There are smart tag problems if data adapter 
		/// designers are used from this assembly.
		/// </summary>
		bool IgnoreType(string name)
		{
			int idx = name.IndexOf(",");
			if (idx > 0) {
				string[] splitName = name.Split(',');
				string assemblyName = splitName[1].Substring(1);
				if (assemblyName == "Microsoft.VSDesigner") {
					return true;
				}
			}
			return false;
		}
	}
}
