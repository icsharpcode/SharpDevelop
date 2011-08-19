// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.FormsDesigner.Services
{
	public class DomTypeLocator : MarshalByRefObject, ITypeLocator
	{
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
		
		public DomTypeLocator(string formSourceFileName)
		{
			this.formSourceFileName = formSourceFileName;
		}
		
		static readonly Dictionary<IProjectContent, object> projectContentsCurrentlyLoadingAssembly = new Dictionary<IProjectContent, object>();
		
		public AssemblyInfo LocateType(string name, out AssemblyInfo[] referencedAssemblies)
		{
			IProjectContent pc = CallingProject;
			
			if (pc != null) {
				// find assembly containing type by using SharpDevelop.Dom
				IClass foundClass = pc.GetClassByReflectionName(name, true);
				if (foundClass != null) {
					AssemblyInfo assembly = GetPathToAssembly(foundClass.ProjectContent);
					
					if (assembly == AssemblyInfo.Empty) {
						referencedAssemblies = new AssemblyInfo[0];
						return AssemblyInfo.Empty;
					}
					
					List<AssemblyInfo> assemblies = new List<AssemblyInfo>();
					
					FindReferencedAssemblies(assemblies, pc);
					
					if (assemblies.Contains(assembly))
						assemblies.Remove(assembly);
					
					referencedAssemblies = assemblies.ToArray();
					return assembly;
				}
			}
			
			referencedAssemblies = new AssemblyInfo[0];
			return AssemblyInfo.Empty;
		}
		
		void FindReferencedAssemblies(List<AssemblyInfo> assemblies, IProjectContent pc)
		{
			// prevent StackOverflow when project contents have cyclic dependencies
			// Very popular example of cyclic dependency: System <-> System.Xml (yes, really!)
			if (projectContentsCurrentlyLoadingAssembly.ContainsKey(pc))
				return;
			projectContentsCurrentlyLoadingAssembly.Add(pc, null);
			
			AssemblyInfo assembly = GetPathToAssembly(pc);
			
			if (!assemblies.Contains(assembly))
				assemblies.Add(assembly);
			
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

		AssemblyInfo GetPathToAssembly(IProjectContent pc)
		{
			string path = "";
			string fullName = "";
			
			if (pc.Project != null) {
				path = ((IProject)pc.Project).OutputAssemblyFullPath;
				fullName = ((IProject)pc.Project).AssemblyName;
			} else if (pc is ReflectionProjectContent) {
				path = ((ReflectionProjectContent)pc).AssemblyLocation;
				fullName = ((ReflectionProjectContent)pc).AssemblyFullName;
			}
			
			return new AssemblyInfo(fullName, path, GacInterop.IsWithinGac(path));
		}
	}
	
	public class DomGacWrapper : MarshalByRefObject, IGacWrapper
	{
		public bool IsGacAssembly(string path)
		{
			return GacInterop.IsWithinGac(path);
		}
	}
}
