// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Workflow.ComponentModel.Compiler;
using System.Reflection;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop;
#endregion

namespace WorkflowDesigner
{
	
	/// <summary>
	/// This service maintains a single TypeProvider for each workflow project. All designers in
	/// the project will use the same typeprovider. The providers are kept up to date with the
	/// project references so the designers do no need manage it themselves.
	/// </summary>
	public class TypeProviderService
	{
		private static Dictionary<IProject, TypeProvider> providers = null;

		#region Property Accessorors
		private static Dictionary<IProject, TypeProvider> Providers {
			get {
				if (providers == null)
					providers = new Dictionary<IProject, TypeProvider>();
				
				return providers;
			}
		}
		#endregion
		
		
		static TypeProviderService()
		{
			ProjectService.ProjectItemRemoved += ProjectItemRemovedEventHandler;
			ProjectService.ProjectItemAdded += ProjectItemAddedEventHandler;
			ProjectService.SolutionClosing += SolutionClosingEventHandler;
		}
		
		
		/// <summary>
		/// Return the type provider for the specified project.
		/// </summary>
		/// <param name="project">Project whose ITypeProvider is required</param>
		/// <returns>The ITypeProvider for the pass IProject, or a default provider if
		/// no project passed.</returns>
		public static ITypeProvider GetTypeProvider(IProject project)
		{

			if ((project != null) && (Providers.ContainsKey(project)))
				return Providers[project];
			
			TypeProvider typeProvider = new TypeProvider(null);
			
			// Always need the core runtime!
			Assembly assembly2 = AppDomain.CurrentDomain.Load("mscorlib");
			typeProvider.AddAssembly(assembly2);

			if (project == null)
				return typeProvider;
			
			LoadProjectReferences(project, typeProvider);
			
			Providers.Add(project, typeProvider);

			MSBuildBasedProject p = project as MSBuildBasedProject;
			p.ActiveConfigurationChanged += ActiveConfigurationChangedEventHandler;
			
			return typeProvider;
		}
		
		private static void LoadProjectReferences(IProject project, TypeProvider typeProvider)
		{
			
			foreach (ProjectItem item in project.Items) {
				
				Assembly assembly = LoadAssembly(item, AppDomain.CurrentDomain);
				
				if (assembly != null)
					typeProvider.AddAssembly(assembly);
			}
			
		}
		
		private static void ProjectItemAddedEventHandler(object sender, ProjectItemEventArgs e)
		{
			if (e.Project == null) return;
			if (!Providers.ContainsKey(e.Project)) return;

			ReferenceProjectItem item = e.ProjectItem as ReferenceProjectItem;
			if (item == null) return;

			Assembly assembly = LoadAssembly(item);
			
			if (assembly != null)
				Providers[e.Project].AddAssembly(assembly);
		}

		private static void ProjectItemRemovedEventHandler(object sender, ProjectItemEventArgs e)
		{
			if (e.Project == null) return;
			if (!Providers.ContainsKey(e.Project)) return;
			
			ReferenceProjectItem item = e.ProjectItem as ReferenceProjectItem;
			if (item == null) return;

			Assembly assembly = LoadAssembly(item);
			
			if (assembly != null)
				Providers[e.Project].RemoveAssembly(assembly);
			
			
		}
		
		private static Assembly LoadAssembly(ProjectItem item)
		{
			return LoadAssembly(item, AppDomain.CurrentDomain);
		}
		
		private static Assembly LoadAssembly(ProjectItem item, AppDomain appDomain)
		{
			
			Assembly assembly = null;
			
			if (item is ProjectReferenceProjectItem) {
				ProjectReferenceProjectItem pitem = item as ProjectReferenceProjectItem;
				AssemblyName name = new AssemblyName();
				name.CodeBase = pitem.ReferencedProject.OutputAssemblyFullPath;
				
				// TODO: This is only a temporary solution so the assembly is not locked.
				// Need to look at this in terms of using a separate domain.
				assembly = appDomain.Load(File.ReadAllBytes(pitem.ReferencedProject.OutputAssemblyFullPath));

			} else if (item is ReferenceProjectItem) {
				assembly = ReflectionLoader.ReflectionLoadGacAssembly(item.Include, false);
				
				if (assembly == null) {
					AssemblyName name = new AssemblyName();
					name.CodeBase = item.FileName;
					assembly = appDomain.Load(name);
				}
				
			}
			
			return assembly;
		}
		
		private static void SolutionClosingEventHandler(object sender, SolutionEventArgs e)
		{
			// Remove unsed providers for closed projects.
			foreach (IProject project in e.Solution.Projects)
			{
				if (Providers.ContainsKey(project))
				{
					Providers[project].Dispose();
					Providers.Remove(project);
				}
			}
		}

		private static void ActiveConfigurationChangedEventHandler(object sender, EventArgs e)
		{
			IProject project = sender as IProject;

			if (!Providers.ContainsKey(project))
				return;
			
			// Reload the typeprovider.
			ICSharpCode.Core.LoggingService.DebugFormatted("Reloading TypeProvider assemblies for project {0}", project.Name);
			TypeProvider typeProvider = Providers[project];
			foreach (Assembly asm in typeProvider.ReferencedAssemblies)
				typeProvider.RemoveAssembly(asm);
			
			LoadProjectReferences(project, typeProvider);
			
		}
		
	}
}
