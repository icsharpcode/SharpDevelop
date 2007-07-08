// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

using ICSharpCode.SharpDevelop.Dom.ReflectionLayer;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class ReflectionProjectContent : DefaultProjectContent
	{
		string assemblyFullName;
		DomAssemblyName[] referencedAssemblyNames;
		ICompilationUnit assemblyCompilationUnit;
		string assemblyLocation;
		ProjectContentRegistry registry;
		
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
		
		/// <summary>
		/// Gets the list of assembly names referenced by this project content.
		/// </summary>
		public IList<DomAssemblyName> ReferencedAssemblyNames {
			get { return Array.AsReadOnly(referencedAssemblyNames); }
		}
		
		public ICompilationUnit AssemblyCompilationUnit {
			get { return assemblyCompilationUnit; }
		}
		
		public override IList<IAttribute> GetAssemblyAttributes()
		{
			return assemblyCompilationUnit.Attributes;
		}
		
		DateTime assemblyFileLastWriteTime;
		
		/// <summary>
		/// Gets if the project content is representing the current version of the assembly.
		/// This property always returns true for ParseProjectContents but might return false
		/// for ReflectionProjectContent/CecilProjectContent if the file was changed.
		/// </summary>
		public override bool IsUpToDate {
			get {
				DateTime newWriteTime;
				try {
					newWriteTime = File.GetLastWriteTimeUtc(assemblyLocation);
				} catch (Exception ex) {
					LoggingService.Warn(ex);
					return true;
				}
				return assemblyFileLastWriteTime == newWriteTime;
			}
		}
		
		public ReflectionProjectContent(Assembly assembly, ProjectContentRegistry registry)
			: this(assembly, assembly.Location, registry)
		{
		}
		
		public ReflectionProjectContent(Assembly assembly, string assemblyLocation, ProjectContentRegistry registry)
			: this(assembly.FullName, assemblyLocation, DomAssemblyName.Convert(assembly.GetReferencedAssemblies()), registry)
		{
			foreach (Type type in assembly.GetExportedTypes()) {
				string name = type.FullName;
				if (name.IndexOf('+') < 0) { // type.IsNested
					AddClassToNamespaceListInternal(new ReflectionClass(assemblyCompilationUnit, type, name, null));
				}
			}
			ReflectionClass.AddAttributes(this, assemblyCompilationUnit.Attributes, CustomAttributeData.GetCustomAttributes(assembly));
			InitializeSpecialClasses();
		}
		
		public ReflectionProjectContent(string assemblyFullName, string assemblyLocation, DomAssemblyName[] referencedAssemblies, ProjectContentRegistry registry)
		{
			if (assemblyFullName == null)
				throw new ArgumentNullException("assemblyFullName");
			if (assemblyLocation == null)
				throw new ArgumentNullException("assemblyLocation");
			if (registry == null)
				throw new ArgumentNullException("registry");
			
			this.registry = registry;
			this.assemblyFullName = assemblyFullName;
			this.referencedAssemblyNames = referencedAssemblies;
			this.assemblyLocation = assemblyLocation;
			this.assemblyCompilationUnit = new DefaultCompilationUnit(this);
			
			try {
				assemblyFileLastWriteTime = File.GetLastWriteTimeUtc(assemblyLocation);
			} catch (Exception ex) {
				LoggingService.Warn(ex);
			}
			
			string fileName = LookupLocalizedXmlDoc(assemblyLocation);
			// Not found -> look in runtime directory.
			if (fileName == null) {
				string runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
				fileName = LookupLocalizedXmlDoc(Path.Combine(runtimeDirectory, Path.GetFileName(assemblyLocation)));
			}
			if (fileName == null) {
				// still not found -> look in WinFX reference directory
				string referenceDirectory = WinFXReferenceDirectory;
				if (!string.IsNullOrEmpty(referenceDirectory)) {
					fileName = LookupLocalizedXmlDoc(Path.Combine(referenceDirectory, Path.GetFileName(assemblyLocation)));
				}
			}
			
			if (fileName != null) {
				if (registry.persistence != null) {
					this.XmlDoc = XmlDoc.Load(fileName, Path.Combine(registry.persistence.CacheDirectory, "XmlDoc"));
				} else {
					this.XmlDoc = XmlDoc.Load(fileName, null);
				}
			}
		}
		
		static string WinFXReferenceDirectory {
			get {
				RegistryKey k = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0\Setup\Windows Communication Foundation");
				if (k == null)
					return null;
				object o = k.GetValue("ReferenceInstallPath");
				k.Close();
				if (o == null)
					return null;
				else
					return o.ToString();
			}
		}
		
		public void InitializeSpecialClasses()
		{
			if (GetClassInternal(VoidClass.VoidName, 0, Language) != null) {
				AddClassToNamespaceList(VoidClass.Instance);
			}
		}
		
		bool initialized = false;
		List<DomAssemblyName> missingNames;
		
		public void InitializeReferences()
		{
			bool changed = false;
			if (initialized) {
				if (missingNames != null) {
					for (int i = 0; i < missingNames.Count; i++) {
						IProjectContent content = registry.GetExistingProjectContent(missingNames[i]);
						if (content != null) {
							changed = true;
							lock (ReferencedContents) {
								ReferencedContents.Add(content);
							}
							missingNames.RemoveAt(i--);
						}
					}
					if (missingNames.Count == 0) {
						missingNames = null;
					}
				}
			} else {
				initialized = true;
				foreach (DomAssemblyName name in referencedAssemblyNames) {
					IProjectContent content = registry.GetExistingProjectContent(name);
					if (content != null) {
						changed = true;
						lock (ReferencedContents) {
							ReferencedContents.Add(content);
						}
					} else {
						if (missingNames == null)
							missingNames = new List<DomAssemblyName>();
						missingNames.Add(name);
					}
				}
			}
			if (changed)
				OnReferencedContentsChanged(EventArgs.Empty);
		}
		
		public override string ToString()
		{
			return string.Format("[{0}: {1}]", GetType().Name, assemblyFullName);
		}
	}
}
