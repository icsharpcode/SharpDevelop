// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class PersistedWorkspace
	{
		public PersistedWorkspace()
		{
			AssemblyFiles = new List<string>();
		}
		
		public string Name { get; set; }
		public List<String> AssemblyFiles { get; set; }
		public bool IsActive { get; set; }
	}
	
	class UnresolvedAssemblyEntityModelContext : IEntityModelContext
	{
		string assemblyName;
		string location;
		
		public UnresolvedAssemblyEntityModelContext(string assemblyName, string location)
		{
			this.assemblyName = assemblyName;
			this.location = location;
		}
		
		public ICompilation GetCompilation()
		{
			return null;
		}
		
		public bool IsBetterPart(IUnresolvedTypeDefinition part1, IUnresolvedTypeDefinition part2)
		{
			return false;
		}
		
		public IProject Project {
			get {
				return null;
			}
		}
		
		public string AssemblyName {
			get {
				return assemblyName;
			}
		}
		
		public string Location {
			get {
				return location;
			}
		}
		
		public bool IsValid {
			get { return false; }
		}
	}
	
	class ClassBrowserPad : AbstractPadContent, IClassBrowser
	{
		#region IClassBrowser implementation

		public ICollection<SharpTreeNode> SpecialNodes {
			get { return treeView.SpecialNodes; }
		}

		public AssemblyList AssemblyList {
			get { return treeView.AssemblyList; }
			set { treeView.AssemblyList = value; }
		}
		
		#endregion
		
		const string PersistedWorkspaceSetting = "ClassBrowser.Workspaces";
		const string DefaultWorkspaceName = "<default>";

		IProjectService projectService;
		ClassBrowserTreeView treeView;
		DockPanel panel;
		ToolBar toolBar;
		
		List<PersistedWorkspace> persistedWorkspaces;
		PersistedWorkspace activeWorkspace;

		public ClassBrowserPad()
			: this(SD.GetRequiredService<IProjectService>())
		{
		}
		
		public ClassBrowserPad(IProjectService projectService)
		{
			this.projectService = projectService;
			panel = new DockPanel();
			treeView = new ClassBrowserTreeView(); // treeView must be created first because it's used by CreateToolBar

			toolBar = CreateToolBar("/SharpDevelop/Pads/ClassBrowser/Toolbar");
			panel.Children.Add(toolBar);
			DockPanel.SetDock(toolBar, Dock.Top);
			
			panel.Children.Add(treeView);
			
			//treeView.ContextMenu = CreateContextMenu("/SharpDevelop/Pads/UnitTestsPad/ContextMenu");
			projectService.CurrentSolutionChanged += ProjectServiceCurrentSolutionChanged;
			ProjectServiceCurrentSolutionChanged(null, null);
			
			// Load workspaces from configuration
			LoadWorkspaces();
		}
		
		public override void Dispose()
		{
			projectService.CurrentSolutionChanged -= ProjectServiceCurrentSolutionChanged;
			base.Dispose();
		}
		
		public override object Control {
			get { return panel; }
		}
		
		public IClassBrowserTreeView TreeView {
			get { return treeView; }
		}
		
		void ProjectServiceCurrentSolutionChanged(object sender, EventArgs e)
		{
			foreach (var node in treeView.SpecialNodes.OfType<SolutionTreeNode>().ToArray())
				treeView.SpecialNodes.Remove(node);
			if (projectService.CurrentSolution != null)
				treeView.SpecialNodes.Add(new SolutionTreeNode(projectService.CurrentSolution));
		}
		
		void AssemblyListCollectionChanged(IReadOnlyCollection<IAssemblyModel> removedItems, IReadOnlyCollection<IAssemblyModel> addedItems)
		{
			foreach (var assembly in addedItems) {
				// Add this assembly to current workspace
				if (activeWorkspace != null) {
					activeWorkspace.AssemblyFiles.Add(assembly.Context.Location);
				}
			}
			
			foreach (var assembly in removedItems) {
				// Add this assembly to current workspace
				if (activeWorkspace != null) {
					activeWorkspace.AssemblyFiles.Remove(assembly.Context.Location);
				}
			}
			
			// Update workspace list in configuration
			SaveWorkspaces();
		}
		
		/// <summary>
		/// Virtual method so we can override this method and return
		/// a dummy ToolBar when testing.
		/// </summary>
		protected virtual ToolBar CreateToolBar(string name)
		{
			Debug.Assert(treeView != null);
			return ToolBarService.CreateToolBar(treeView, treeView, name);
		}
		
		/// <summary>
		/// Virtual method so we can override this method and return
		/// a dummy ContextMenu when testing.
		/// </summary>
		protected virtual ContextMenu CreateContextMenu(string name)
		{
			Debug.Assert(treeView != null);
			return MenuService.CreateContextMenu(treeView, name);
		}
		
		/// <summary>
		/// Loads persisted workspaces from configuration.
		/// </summary>
		void LoadWorkspaces()
		{
			persistedWorkspaces = SD.PropertyService.GetList<PersistedWorkspace>(PersistedWorkspaceSetting).ToList();
			if (!persistedWorkspaces.Any())
			{
				// Add at least default workspace
				persistedWorkspaces = new List<PersistedWorkspace>();
				persistedWorkspaces.Add(new PersistedWorkspace()
				                        {
				                        	Name = DefaultWorkspaceName
				                        });
			}
			
			// Load all assemblies (for now always from default workspace)
			PersistedWorkspace defaultWorkspace = persistedWorkspaces.FirstOrDefault(w => w.Name == DefaultWorkspaceName);
			ActivateWorkspace(defaultWorkspace);
		}
		
		/// <summary>
		/// Stores currently saved workspaces in configuration.
		/// </summary>
		void SaveWorkspaces()
		{
			SD.PropertyService.SetList<PersistedWorkspace>(PersistedWorkspaceSetting, persistedWorkspaces);
		}
		
		public static IAssemblyModel CreateAssemblyModelFromFile(string fileName)
		{
			var loader = new CecilLoader();
			loader.IncludeInternalMembers = true;
			loader.LazyLoad = true;
			var assembly = loader.LoadAssemblyFile(fileName);
			
			IEntityModelContext context = new AssemblyEntityModelContext(assembly);
			IAssemblyModel model = SD.GetRequiredService<IModelFactory>().CreateAssemblyModel(context);
			if (model is IUpdateableAssemblyModel) {
				((IUpdateableAssemblyModel)model).Update(EmptyList<IUnresolvedTypeDefinition>.Instance, assembly.TopLevelTypeDefinitions.ToList());
				((IUpdateableAssemblyModel) model).AssemblyName = assembly.AssemblyName;
			}
			return model;
		}
		
		static IAssemblyModel SafelyCreateAssemblyModelFromFile(string fileName)
		{
			try {
				return CreateAssemblyModelFromFile(fileName);
			} catch (Exception) {
				// Special AssemblyModel for unresolved file references
				IEntityModelContext unresolvedContext = new UnresolvedAssemblyEntityModelContext(Path.GetFileName(fileName), fileName);
				IAssemblyModel unresolvedModel = SD.GetRequiredService<IModelFactory>().CreateAssemblyModel(unresolvedContext);
				if (unresolvedModel is IUpdateableAssemblyModel) {
					((IUpdateableAssemblyModel) unresolvedModel).AssemblyName = unresolvedContext.AssemblyName;
				}
				
				return unresolvedModel;
			}
		}
		
		void AppendAssemblyFileToList(string assemblyFile)
		{
			IAssemblyModel assemblyModel = SafelyCreateAssemblyModelFromFile(assemblyFile);
			if (assemblyModel != null) {
				AssemblyList.Assemblies.Add(assemblyModel);
			}
		}
		
		/// <summary>
		/// Activates the specified workspace.
		/// </summary>
		void ActivateWorkspace(PersistedWorkspace workspace)
		{
			// Update the activation flags in workspace list
			foreach (var workspaceElement in persistedWorkspaces) {
				workspaceElement.IsActive = (workspaceElement == workspace);
			}
			
			UpdateActiveWorkspace();
		}
		
		/// <summary>
		/// Updates active workspace and AssemblyList according to <see cref="PersistedWorkspace.IsActive"/> flags.
		/// </summary>
		void UpdateActiveWorkspace()
		{
			if ((AssemblyList != null) && (activeWorkspace != null)) {
				// Temporarily detach from event handler
				AssemblyList.Assemblies.CollectionChanged -= AssemblyListCollectionChanged;
			}
			
			activeWorkspace = persistedWorkspaces.FirstOrDefault(w => w.IsActive);
			if (activeWorkspace == null) {
				// If no workspace is active, activate default
				var defaultWorkspace = persistedWorkspaces.FirstOrDefault(w => w.Name == DefaultWorkspaceName);
				activeWorkspace = defaultWorkspace;
				defaultWorkspace.IsActive = true;
			}
			
			AssemblyList.Assemblies.Clear();
			if (activeWorkspace != null) {
				foreach (string assemblyFile in activeWorkspace.AssemblyFiles) {
					AppendAssemblyFileToList(assemblyFile);
				}
			}
			
			// Attach to event handler, again.
			if (AssemblyList != null) {
				AssemblyList.Assemblies.CollectionChanged += AssemblyListCollectionChanged;
			}
		}
	}
}
