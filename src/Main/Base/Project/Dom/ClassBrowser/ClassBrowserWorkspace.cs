// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Description of ClassBrowserWorkspace.
	/// </summary>
	public class ClassBrowserWorkspace
	{
		ISolution assignedSolution;
		IModelCollection<IAssemblyModel> loadedAssemblies;
		string workspaceName;
		
		public ClassBrowserWorkspace(ISolution assignedSolution, IEnumerable<IAssemblyModel> assemblies = null)
			: this(assignedSolution.FileName, assemblies)
		{
			this.assignedSolution = assignedSolution;
		}
		
		public ClassBrowserWorkspace(string workspaceName, IEnumerable<IAssemblyModel> assemblies = null)
		{
			this.workspaceName = workspaceName;
			this.loadedAssemblies = new SimpleModelCollection<IAssemblyModel>(assemblies ?? EmptyList<IAssemblyModel>.Instance);
		}
		
		public bool IsAssigned {
			get { return assignedSolution != null; }
		}
		
		public ISolution AssignedSolution {
			get { return assignedSolution; }
		}
		
		public string Name {
			get { return workspaceName; }
		}
		
		public IModelCollection<IAssemblyModel> LoadedAssemblies {
			get { return loadedAssemblies; }
		}
	}
	
	public class ClassBrowserSettings
	{
		IAssemblyReference[] ResolveReferences(IUnresolvedAssembly asm)
		{
			return new IAssemblyReference[0];
		}

		IAssemblyModel[] LoadAssemblyList(string name)
		{
			var assemblyNames = Container.GetList<string>("AssemblyList." + name);
			CecilLoader loader = new CecilLoader();
			var factory = SD.GetRequiredService<IModelFactory>();
			return assemblyNames
				.Select(loader.LoadAssemblyFile)
				.Select(asm => new AssemblyEntityModelContext(asm, ResolveReferences(asm)))
				.Select(factory.CreateAssemblyModel)
				.ToArray();
		}

		readonly Properties Container = SD.PropertyService.NestedProperties(typeof(ClassBrowserSettings).FullName);
		
		public ClassBrowserWorkspace LoadDefaultWorkspace()
		{
			return LoadWorkspace("<default>");
		}
		
		public ClassBrowserWorkspace LoadWorkspace(string name)
		{
			return new ClassBrowserWorkspace(name, LoadAssemblyList(name));
		}
		
		public ClassBrowserWorkspace LoadForSolution(ISolution solution)
		{
			// maybe use solution.Preferences?
			return new ClassBrowserWorkspace(solution, LoadAssemblyList(solution.FileName));
		}
		
		public void SaveWorkspace(ClassBrowserWorkspace workspace)
		{
			
		}
	}
}
