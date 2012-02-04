// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.SharpDevelop.Dom.ReflectionLayer;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class ReflectionProjectContent : DefaultProjectContent
	{
		string assemblyFullName;
		string assemblyName;
		DomAssemblyName[] referencedAssemblyNames;
		ICompilationUnit assemblyCompilationUnit;
		string assemblyLocation;
		ProjectContentRegistry registry;
		
		/// <summary>
		/// Gets the file path to the (reference) assembly.
		/// </summary>
		public string AssemblyLocation {
			get {
				return assemblyLocation;
			}
		}
		
		/// <summary>
		/// Gets the full assembly name (e.g. "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
		/// </summary>
		public string AssemblyFullName {
			get {
				return assemblyFullName;
			}
		}
		
		/// <summary>
		/// Gets the short assembly name (e.g. "System")
		/// </summary>
		public override string AssemblyName {
			get { return assemblyName; }
		}
		
		volatile string assemblyLocationInGAC;
		
		string GetAssemblyLocationInGAC()
		{
			if (assemblyLocationInGAC == null) {
				assemblyLocationInGAC = GacInterop.FindAssemblyInNetGac(new DomAssemblyName(assemblyFullName)) ?? string.Empty;
			}
			return assemblyLocationInGAC;
		}
		
		/// <summary>
		/// Gets the real assembly location.
		/// For reference assemblies of GAC assemblies, this is the assembly in the GAC.
		/// Otherwise, this is the same as AssemblyLocation.
		/// </summary>
		public string RealAssemblyLocation {
			get {
				string gacAssembly = GetAssemblyLocationInGAC();
				if (string.IsNullOrEmpty(gacAssembly))
					return this.AssemblyLocation;
				else
					return gacAssembly;
			}
		}
		
		/// <summary>
		/// Gets whether this assembly is available in the GAC.
		/// This property also returns true for reference assemblies when the corresponding real assembly is available in the GAC.
		/// </summary>
		public bool IsGacAssembly {
			get { return !string.IsNullOrEmpty(GetAssemblyLocationInGAC()); }
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
			InitializeSpecialClasses();
			AddAssemblyAttributes(assembly);
			assemblyCompilationUnit.Freeze();
		}
		
		/// <summary>
		/// Adds assembly attributes from the specified assembly.
		/// 
		/// The constructor already does this, this method is meant for unit tests only!
		/// </summary>
		public void AddAssemblyAttributes(Assembly assembly)
		{
			ReflectionClass.AddAttributes(this, assemblyCompilationUnit.Attributes, CustomAttributeData.GetCustomAttributes(assembly));
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
			this.assemblyName = (assemblyFullName.IndexOf(',') > -1) ? assemblyFullName.Substring(0, assemblyFullName.IndexOf(',')) : assemblyFullName;
			this.referencedAssemblyNames = referencedAssemblies;
			this.assemblyLocation = assemblyLocation;
			this.assemblyCompilationUnit = new DefaultCompilationUnit(this);
			
			try {
				assemblyFileLastWriteTime = File.GetLastWriteTimeUtc(assemblyLocation);
			} catch (Exception ex) {
				LoggingService.Warn(ex);
			}
			
			string fileName = null;
			if (assemblyLocation != typeof(object).Assembly.Location) {
				// First look in the assembly's directory.
				// mscorlib is the exception, because it is loaded from the runtime directory (C:\Windows\Microsoft.NET\Framework\v4.0.30319),
				// but that might contain an outdated version of mscorlib.xml (with less documented members than the mscorlib.xml in the Reference Assemblies)
				// (at least on my machine, lots of others don't seem to have the v4.0.30319\mscorlib.xml at all).
				fileName = XmlDoc.LookupLocalizedXmlDoc(assemblyLocation);
			}
			if (fileName == null) {
				// Not found -> look in other directories:
				foreach (string testDirectory in XmlDoc.XmlDocLookupDirectories) {
					fileName = XmlDoc.LookupLocalizedXmlDoc(Path.Combine(testDirectory, Path.GetFileName(assemblyLocation)));
					if (fileName != null)
						break;
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
		
		public void InitializeSpecialClasses()
		{
			// Replace the class representing System.Void with VoidClass.Instance
			IClass voidClass = GetClassInternal(VoidClass.VoidName, 0, Language);
			if (voidClass != null) {
				RemoveClass(voidClass);
				AddClassToNamespaceList(new VoidClass(this));
			}
		}
		
		bool initialized = false;
		List<DomAssemblyName> missingNames;
		
		public void InitializeReferences()
		{
			InitializeReferences(new IProjectContent[0]);
		}
		
		public void InitializeReferences(IProjectContent[] existingContents)
		{
			bool changed = false;
			if (initialized) {
				if (missingNames != null) {
					for (int i = 0; i < missingNames.Count; i++) {
						IProjectContent content = GetExistingProjectContent(existingContents, missingNames[i]);
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
		
		IProjectContent GetExistingProjectContent(IProjectContent[] existingProjectContents, DomAssemblyName fullAssemblyName)
		{
			IProjectContent content = registry.GetExistingProjectContent(fullAssemblyName);
			if (content != null) {
				return content;
			} else if (existingProjectContents.Any()) {
				return GetExistingProjectContentForShortName(existingProjectContents, fullAssemblyName.ShortName);
			}
			return null;
		}
		
		IProjectContent GetExistingProjectContentForShortName(IProjectContent[] existingProjectContents, string shortName)
		{
			return existingProjectContents.FirstOrDefault(pc => pc.AssemblyName == shortName);
		}
		
		public override string ToString()
		{
			return string.Format("[{0}: {1}]", GetType().Name, assemblyFullName);
		}
	}
}
