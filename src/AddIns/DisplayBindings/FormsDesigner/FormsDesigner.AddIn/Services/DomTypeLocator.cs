// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.FormsDesigner.Services
{
	public class DomTypeLocator : ITypeLocator
	{
		string formSourceFileName;
		IProjectContent callingProject;
		
		public System.ComponentModel.Design.ITypeResolutionService ParentService { get; set; }
		
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
		
		public DomTypeLocator(string formSourceFileName)
		{
			this.formSourceFileName = formSourceFileName;
		}
		
		static readonly Dictionary<IProjectContent, object> projectContentsCurrentlyLoadingAssembly = new Dictionary<IProjectContent, object>();
		
		public string LocateType(string name, out string[] referencedAssemblies)
		{
			IProjectContent pc = CallingProject;
			
			if (pc != null) {
				// find assembly containing type by using SharpDevelop.Dom
				IClass foundClass;
				if (name.Contains("`")) {
					int typeParameterCount;
					int.TryParse(name.Substring(name.IndexOf('`') + 1), out typeParameterCount);
					foundClass = pc.GetClass(name.Substring(0, name.IndexOf('`')).Replace('+', '.'), typeParameterCount);
				} else {
					foundClass = pc.GetClass(name.Replace('+', '.'), 0);
				}
				if (foundClass != null) {
					string path = GetPathToAssembly(pc);
					
					if (string.IsNullOrEmpty(path)) {
						referencedAssemblies = new string[0];
						return "";
					}
					
					List<string> assemblies = new List<string>();
					
					FindReferencedAssemblies(assemblies, pc);
					
					if (assemblies.Contains(path))
						assemblies.Remove(path);
					
					referencedAssemblies = assemblies.ToArray();
					return path;
				}
			}
			
			referencedAssemblies = new string[0];
			return "";
		}
		
		void FindReferencedAssemblies(List<string> assemblies, IProjectContent pc)
		{
			// prevent StackOverflow when project contents have cyclic dependencies
			// Very popular example of cyclic dependency: System <-> System.Xml (yes, really!)
			if (projectContentsCurrentlyLoadingAssembly.ContainsKey(pc))
				return;
			projectContentsCurrentlyLoadingAssembly.Add(pc, null);
			
			string path = GetPathToAssembly(assemblies, pc);
			
			if (!string.IsNullOrEmpty(path) && !assemblies.Contains(path))
				assemblies.Add(path);
			
			try {
				// load dependencies of current assembly
				foreach (IProjectContent rpc in pc.ReferencedContents) {
					if (rpc is ParseProjectContent) {
						FindReferencedAssemblies(assemblies, rpc);
					} else if (rpc is ReflectionProjectContent) {
						ReflectionProjectContent rrpc = (ReflectionProjectContent)rpc;
						if (!GacInterop.IsWithinGac(rrpc.AssemblyLocation))
							FindReferencedAssemblies(assemblies, rpc);
					}
				}
			} finally {
				projectContentsCurrentlyLoadingAssembly.Remove(pc);
			}
		}

		string GetPathToAssembly(IProjectContent pc)
		{
			if (pc.Project != null)
				return ((IProject)pc.Project).OutputAssemblyFullPath;
			else if (pc is ReflectionProjectContent)
				return ((ReflectionProjectContent)pc).AssemblyLocation;
			
			return null;
		}
		
		public bool IsGacAssembly(string path)
		{
			return GacInterop.IsWithinGac(path);
		}
	}
}
