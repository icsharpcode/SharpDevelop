// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Workflow.ComponentModel.Compiler;
using System.Reflection;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// This service maintains a single TypeProvider for each workflow project. All designers in
	/// the project will use the same typeprovider. The providers are kept up to date with the
	/// project references so the designers do no need magane it themselves.
	/// </summary>
	public class TypeProviderService
	{
		private static Dictionary<IProject, TypeProvider> providers;
		private static bool initialised;

		#region Property Accessor
		private static Dictionary<IProject, TypeProvider> Providers {
			get {
				if (providers == null)
					providers = new Dictionary<IProject, TypeProvider>();
				
				return providers;
			}
		}
		#endregion
		
		
		private static void Initialise()
		{
			ProjectService.ProjectItemRemoved += ProjectItemRemovedEventHandler;
			ProjectService.ProjectItemAdded += ProjectItemAddedEventHandler;
			ProjectService.SolutionClosing += SolutionClosingEventHandler;
			
			initialised = true;
		}
		
		
		/// <summary>
		/// Return the type provider for the specified project.
		/// </summary>
		/// <param name="project">Project whose ITypeProvider is required</param>
		/// <returns>The ITypeProvider for the pass IProject, or a default provider if
		/// no project passed.</returns>
		public static ITypeProvider GetTypeProvider(IProject project)
		{
			if (!initialised) Initialise();

			if ((project != null) && (Providers.ContainsKey(project)))
				return Providers[project];
			
			TypeProvider typeProvider = new TypeProvider(null);
			
			// Always need the core runtime!
			Assembly assembly2 = AppDomain.CurrentDomain.Load("mscorlib");
			typeProvider.AddAssembly(assembly2);

			if (project == null)
				return typeProvider;
			
			// Load the references for the project.
			foreach (ProjectItem item in project.Items) {
				if (item is ReferenceProjectItem) {
					
					Assembly assembly = ReflectionLoader.ReflectionLoadGacAssembly(item.Include, false);
					
					if (assembly == null) {
						AssemblyName name = new AssemblyName();
						name.CodeBase = item.FileName;
						assembly = AppDomain.CurrentDomain.Load(name);
					}
					
					typeProvider.AddAssembly(assembly);
				}
			}
			
			Providers.Add(project, typeProvider);
			
			return typeProvider;
		}
		
		private static void ProjectItemAddedEventHandler(object sender, ProjectItemEventArgs e)
		{
			if (e.Project == null) return;
			if (!Providers.ContainsKey(e.Project)) return;
			
			ReferenceProjectItem item = e.ProjectItem as ReferenceProjectItem;
			if (item == null) return;
			
			Assembly assembly = ReflectionLoader.ReflectionLoadGacAssembly(item.Include, false);
			
			if (assembly == null) {
				AssemblyName name = new AssemblyName();
				name.CodeBase = item.FileName;
				assembly = AppDomain.CurrentDomain.Load(name);
			}
			
			Providers[e.Project].AddAssembly(assembly);
			
		}

		private static void ProjectItemRemovedEventHandler(object sender, ProjectItemEventArgs e)
		{
			if (e.Project == null) return;
			if (!Providers.ContainsKey(e.Project)) return;
			
			ReferenceProjectItem item = e.ProjectItem as ReferenceProjectItem;
			if (item == null) return;
			
			Assembly assembly = ReflectionLoader.ReflectionLoadGacAssembly(item.Include, false);
			
			if (assembly == null) {
				AssemblyName name = new AssemblyName();
				name.CodeBase = item.FileName;
				assembly = AppDomain.CurrentDomain.Load(name);
			}
			
			Providers[e.Project].RemoveAssembly(assembly);
			
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
	}
}
