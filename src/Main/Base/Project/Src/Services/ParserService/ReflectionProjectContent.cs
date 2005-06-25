using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Core
{
	public class ReflectionProjectContent : DefaultProjectContent
	{
		Assembly assembly;
		
		public ReflectionProjectContent(Assembly assembly)
		{
			this.assembly = assembly;
			
			ICompilationUnit assemblyCompilationUnit = new DefaultCompilationUnit(this);
			
			foreach (Type type in assembly.GetTypes()) {
				if (!type.FullName.StartsWith("<") && type.IsPublic) {
					AddClassToNamespaceListInternal(new ReflectionClass(assemblyCompilationUnit, type, null));
				}
			}
			string fileName = LookupLocalizedXmlDoc(assembly.Location);
			// Not found -> look in runtime directory.
			if (fileName == null) {
				string runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
				fileName = LookupLocalizedXmlDoc(Path.Combine(runtimeDirectory, Path.GetFileName(assembly.Location)));
			}
			
			if (fileName != null) {
				xmlDoc = XmlDoc.Load(fileName);
			}
		}
		
		bool initialized = false;
		
		public void InitializeReferences()
		{
			if (initialized) return;
			initialized = true;
			foreach (AssemblyName name in assembly.GetReferencedAssemblies()) {
				IProjectContent content = ProjectContentRegistry.GetExistingProjectContent(name);
				if (content != null) {
					ReferencedContents.Add(content);
				}
			}
		}
		
		public override string ToString()
		{
			return string.Format("[{0}: {1}]", GetType().Name, assembly.FullName);
		}
	}
}
