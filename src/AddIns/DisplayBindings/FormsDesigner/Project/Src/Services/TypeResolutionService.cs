// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;
using HashFunction = System.Security.Cryptography.SHA1Managed;

namespace ICSharpCode.FormsDesigner.Services
{
	public class TypeResolutionService : ITypeResolutionService
	{
		readonly static List<Assembly> designerAssemblies = new List<Assembly>();
		// hash of file content -> Assembly
		readonly static Dictionary<string, Assembly> assemblyDict = new Dictionary<string, Assembly>();
		
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
			DesignerAssemblies.Add(ProjectContentRegistry.MscorlibAssembly);
			DesignerAssemblies.Add(ProjectContentRegistry.SystemAssembly);
			DesignerAssemblies.Add(typeof(System.Drawing.Point).Assembly);
		}
		
		string formSourceFileName;
		IProjectContent callingProject;
		
		/// <summary>
		/// Gets the project content of the project that created this TypeResolutionService.
		/// Returns null when no calling project was specified.
		/// </summary>
		public IProjectContent CallingProject {
			get {
				if (formSourceFileName != null) {
					if (ProjectService.OpenSolution != null) {
						IProject p = ProjectService.OpenSolution.FindProjectContainingFile(formSourceFileName);
						if (p != null) {
							callingProject = ParserService.GetProjectContent(p);
						}
					}
					formSourceFileName = null;
				}
				return callingProject;
			}
		}
		
		public TypeResolutionService()
		{
		}
		
		public TypeResolutionService(string formSourceFileName)
		{
			this.formSourceFileName = formSourceFileName;
		}
		
		/// <summary>
		/// Loads the assembly represented by the project content. Returns null on failure.
		/// </summary>
		public static Assembly LoadAssembly(IProjectContent pc)
		{
			if (pc.Project != null) {
				return LoadAssembly(pc.Project.OutputAssemblyFullPath);
			} else if (pc is ReflectionProjectContent) {
				return LoadAssembly((pc as ReflectionProjectContent).AssemblyLocation);
			} else {
				return null;
			}
		}
		
		/// <summary>
		/// Loads the file in none-locking mode. Returns null on failure.
		/// </summary>
		public static Assembly LoadAssembly(string fileName)
		{
			if (!File.Exists(fileName))
				return null;
			byte[] data = File.ReadAllBytes(fileName);
			string hash;
			using (HashFunction hashFunction = new HashFunction()) {
				hash = Convert.ToBase64String(hashFunction.ComputeHash(data));
			}
			lock (assemblyDict) {
				Assembly asm;
				if (assemblyDict.TryGetValue(hash, out asm))
					return asm;
				asm = Assembly.Load(data);
				lock (designerAssemblies) {
					if (!designerAssemblies.Contains(asm))
						designerAssemblies.Add(asm);
				}
				assemblyDict[hash] = asm;
				return asm;
			}
		}
		
		public Assembly GetAssembly(AssemblyName name)
		{
			return GetAssembly(name, false);
		}
		
		public Assembly GetAssembly(AssemblyName name, bool throwOnError)
		{
			try {
				Assembly asm = Assembly.Load(name);
				lock (designerAssemblies) {
					if (!designerAssemblies.Contains(asm))
						designerAssemblies.Add(asm);
				}
				return asm;
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
			if (IgnoreType(name)) {
				return null;
			}
			try {
				lock (designerAssemblies) {
					foreach (Assembly asm in DesignerAssemblies) {
						Type t = asm.GetType(name, false);
						if (t != null) {
							return t;
						}
					}
				}
				
				Type type = Type.GetType(name, false, ignoreCase);
				
				if (type == null) {
					IProjectContent pc = this.CallingProject;
					if (pc != null) {
						IClass foundClass = pc.GetClass(name);
						if (foundClass != null) {
							Assembly assembly = LoadAssembly(foundClass.ProjectContent);
							if (assembly != null) {
								type = assembly.GetType(name, false, ignoreCase);
							}
						}
					}
				}
				
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
							LoggingService.Error(e);
						}
						if (assembly != null) {
							lock (designerAssemblies) {
								if (!designerAssemblies.Contains(assembly))
									designerAssemblies.Add(assembly);
							}
							type = assembly.GetType(typeName, false, ignoreCase);
						} else {
							type = Type.GetType(typeName, false, ignoreCase);
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
