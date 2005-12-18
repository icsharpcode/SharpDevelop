// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class ReflectionProjectContent : DefaultProjectContent
	{
		string assemblyFullName;
		AssemblyName[] referencedAssemblies;
		ICompilationUnit assemblyCompilationUnit;
		string assemblyLocation;

		public string AssemblyLocation {
			get {
				return assemblyLocation;
			}
		}
		
		public string AssemblyFullName {
			get {
				return assemblyFullName;
			}
		}
		
		public AssemblyName[] ReferencedAssemblies {
			get {
				return referencedAssemblies;
			}
		}
		
		public ICompilationUnit AssemblyCompilationUnit {
			get {
				return assemblyCompilationUnit;
			}
		}
		
		public ReflectionProjectContent(Assembly assembly)
			: this(assembly, assembly.Location)
		{
		}
		
		public ReflectionProjectContent(Assembly assembly, string assemblyLocation)
			: this(assembly.FullName, assemblyLocation, assembly.GetReferencedAssemblies())
		{
			foreach (Type type in assembly.GetExportedTypes()) {
				string name = type.FullName;
				if (name.IndexOf('+') < 0) { // type.IsNested
					AddClassToNamespaceListInternal(new ReflectionClass(assemblyCompilationUnit, type, name, null));
				}
			}
			InitializeSpecialClasses();
		}
		
		public ReflectionProjectContent(string assemblyFullName, string assemblyLocation, AssemblyName[] referencedAssemblies)
		{
			this.assemblyFullName = assemblyFullName;
			this.referencedAssemblies = referencedAssemblies;
			this.assemblyLocation = assemblyLocation;
			this.assemblyCompilationUnit = new DefaultCompilationUnit(this);
			
			string fileName = LookupLocalizedXmlDoc(assemblyLocation);
			// Not found -> look in runtime directory.
			if (fileName == null) {
				string runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
				fileName = LookupLocalizedXmlDoc(Path.Combine(runtimeDirectory, Path.GetFileName(assemblyLocation)));
			}
			
			if (fileName != null) {
				xmlDoc = XmlDoc.Load(fileName);
			}
		}
		
		public void InitializeSpecialClasses()
		{
			if (GetClassInternal(VoidClass.VoidName, 0, Language) != null) {
				AddClassToNamespaceList(new VoidClass(assemblyCompilationUnit));
			}
		}
		
		private class VoidClass : ReflectionClass
		{
			internal static readonly string VoidName = typeof(void).FullName;
			
			public VoidClass(ICompilationUnit compilationUnit) : base(compilationUnit, typeof(void), VoidName, null) {}
			
			protected override IReturnType CreateDefaultReturnType() {
				return ReflectionReturnType.Void;
			}
		}
		
		bool initialized = false;
		ArrayList missingNames;
		
		public void InitializeReferences()
		{
			bool changed = false;
			if (initialized) {
				if (missingNames != null) {
					for (int i = 0; i < missingNames.Count; i++) {
						IProjectContent content = ProjectContentRegistry.GetExistingProjectContent((AssemblyName)missingNames[i]);
						if (content != null) {
							changed = true;
							ReferencedContents.Add(content);
							missingNames.RemoveAt(i--);
						}
					}
					if (missingNames.Count == 0) {
						missingNames = null;
					}
				}
				if (changed)
					WorkbenchSingleton.SafeThreadAsyncCall(this, "OnReferencedContentsChanged", EventArgs.Empty);
				return;
			}
			initialized = true;
			foreach (AssemblyName name in referencedAssemblies) {
				IProjectContent content = ProjectContentRegistry.GetExistingProjectContent(name);
				if (content != null) {
					changed = true;
					ReferencedContents.Add(content);
				} else {
					if (missingNames == null)
						missingNames = new ArrayList();
					missingNames.Add(name);
				}
			}
			if (changed)
				WorkbenchSingleton.SafeThreadAsyncCall(this, "OnReferencedContentsChanged", EventArgs.Empty);
		}
		
		public override string ToString()
		{
			return string.Format("[{0}: {1}]", GetType().Name, assemblyFullName);
		}
	}
}
