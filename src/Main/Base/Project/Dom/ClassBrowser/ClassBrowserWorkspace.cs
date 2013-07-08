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
		IModelCollection<IUnresolvedAssembly> loadedAssemblies;
		string workspaceName;
		
		public ClassBrowserWorkspace(ISolution assignedSolution, IEnumerable<IUnresolvedAssembly> assemblies = null)
			: this(assignedSolution.FileName, assemblies)
		{
			this.assignedSolution = assignedSolution;
		}
		
		public ClassBrowserWorkspace(string workspaceName, IEnumerable<IUnresolvedAssembly> assemblies = null)
		{
			this.workspaceName = workspaceName;
			this.loadedAssemblies = new SimpleModelCollection<IUnresolvedAssembly>(assemblies ?? EmptyList<IUnresolvedAssembly>.Instance);
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
		
		public IModelCollection<IUnresolvedAssembly> LoadedAssemblies {
			get { return loadedAssemblies; }
		}
	}
	
	public static class ClassBrowserSettings
	{
		static IUnresolvedAssembly[] LoadAssemblyList(string name)
		{
			var assemblyNames = Container.GetList<string>("AssemblyList." + name);
			CecilLoader loader = new CecilLoader();
			return assemblyNames.Select(loader.LoadAssemblyFile).ToArray();
		}

		static readonly Properties Container = SD.PropertyService.NestedProperties(typeof(ClassBrowserSettings).FullName);
		
		public static ClassBrowserWorkspace LoadDefaultWorkspace()
		{
			return LoadWorkspace("<default>");
		}
		
		public static ClassBrowserWorkspace LoadWorkspace(string name)
		{
			return new ClassBrowserWorkspace(name, LoadAssemblyList(name));
		}
		
		public static ClassBrowserWorkspace LoadForSolution(ISolution solution)
		{
			return new ClassBrowserWorkspace(solution, LoadAssemblyList(solution.FileName));
		}
		
		public static void SaveWorkspace(ClassBrowserWorkspace workspace)
		{
			
		}
	}
}
