// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
			
			try {
				foreach (Type type in assembly.GetExportedTypes()) {
					if (!type.IsNested) {
						AddClassToNamespaceListInternal(new ReflectionClass(assemblyCompilationUnit, type, null));
					}
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
			
			if (assembly == typeof(void).Assembly) {
				// Replace void through the special class (ReturnType(void).GetMethods() does not return
				// the methods of System.Object and System.ValueType)
				AddClassToNamespaceListInternal(new ReflectionClass.VoidClass(assemblyCompilationUnit));
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
		ArrayList missingNames;
		
		public void InitializeReferences()
		{
			if (initialized) {
				if (missingNames != null) {
					for (int i = 0; i < missingNames.Count; i++) {
						IProjectContent content = ProjectContentRegistry.GetExistingProjectContent((AssemblyName)missingNames[i]);
						if (content != null) {
							ReferencedContents.Add(content);
							missingNames.RemoveAt(i--);
						}
					}
					if (missingNames.Count == 0)
						missingNames = null;
				}
				return;
			}
			initialized = true;
			foreach (AssemblyName name in assembly.GetReferencedAssemblies()) {
				IProjectContent content = ProjectContentRegistry.GetExistingProjectContent(name);
				if (content != null) {
					ReferencedContents.Add(content);
				} else {
					if (missingNames == null)
						missingNames = new ArrayList();
					missingNames.Add(name);
				}
			}
		}
		
		public override string ToString()
		{
			return string.Format("[{0}: {1}]", GetType().Name, assembly.FullName);
		}
	}
}
