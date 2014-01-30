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
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;

using ICSharpCode.Core;

namespace ICSharpCode.Reports.Addin
{
	public class TypeResolutionService : ITypeResolutionService
	{
		readonly static List<Assembly> designerAssemblies = new List<Assembly>();
		
		/// <summary>
		/// List of assemblies used by the form designer. This static list is not an optimal solution,
		/// but better than using AppDomain.CurrentDomain.GetAssemblies(). See SD2-630.
		/// </summary>
		public static List<Assembly> DesignerAssemblies {
			get {
				return designerAssemblies;
			}
		}
		
		static TypeResolutionService()
		{
			DesignerAssemblies.Add(typeof(object).Assembly);
			DesignerAssemblies.Add(typeof(Uri).Assembly);
			DesignerAssemblies.Add(typeof(System.Drawing.Point).Assembly);
			DesignerAssemblies.Add(typeof(System.Windows.Forms.Design.AnchorEditor).Assembly);
			DesignerAssemblies.Add(typeof(TypeResolutionService).Assembly);
		}
		
		public Assembly GetAssembly(AssemblyName name)
		{
			return LoadAssembly(name, false);
		}
		
		public Assembly GetAssembly(AssemblyName name, bool throwOnError)
		{
			return LoadAssembly(name, throwOnError);
		}
		
		static Assembly LoadAssembly(AssemblyName name, bool throwOnError)
		{
			try {
				return Assembly.Load(name);
			} catch (System.IO.FileLoadException) {
				if (throwOnError)
					throw;
				return null;
			}
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
			return GetType(name, false, false);
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
			#if DEBUG
			if (!name.StartsWith("System.",StringComparison.InvariantCultureIgnoreCase)) {
				LoggingService.Debug("TypeResolutionService: Looking for " + name);
			}
			#endif
			try {
				
				Type type = Type.GetType(name, false, ignoreCase);
				
				if (type == null) {
					lock (designerAssemblies) {
						foreach (Assembly asm in DesignerAssemblies) {
							Type t = asm.GetType(name, false);
							if (t != null) {
								return t;
							}
						}
					}
				}
				
				if (throwOnError && type == null)
					throw new TypeLoadException(name + " not found by TypeResolutionService");
				
				return type;
			} catch (Exception e) {
				LoggingService.Error(e);
			}
			return null;
		}
		
		public void ReferenceAssembly(AssemblyName name)
		{
			ICSharpCode.Core.LoggingService.Warn("TODO: Add Assembly reference : " + name);
		}
	}
}
