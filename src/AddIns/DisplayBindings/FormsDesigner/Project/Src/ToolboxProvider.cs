// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Gui;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.FormsDesigner
{
	public class ToolboxProvider
	{
		static ICSharpCode.FormsDesigner.Services.ToolboxService toolboxService = null;
		
		static SharpDevelopSideBar sideBar;
		
		static CustomComponentsSideTab customTab;
		
		static ComponentLibraryLoader componentLibraryLoader = new ComponentLibraryLoader();

		public static ComponentLibraryLoader ComponentLibraryLoader {
			get {
				return componentLibraryLoader;
			}
		}
		public static ICSharpCode.FormsDesigner.Services.ToolboxService ToolboxService {
			get {
				CreateToolboxService();
				return toolboxService;
			}
		}
		
		public static SharpDevelopSideBar FormsDesignerSideBar {
			get {
				CreateToolboxService();
				return sideBar;
			}
		}
		
		static void CreateToolboxService()
		{
			Debug.Assert(WorkbenchSingleton.InvokeRequired == false);
			if (toolboxService == null) {
				sideBar = new SharpDevelopSideBar();
				LoadToolbox();
				toolboxService = new ICSharpCode.FormsDesigner.Services.ToolboxService();
				ReloadSideTabs(false);
				toolboxService.SelectedItemUsed += new EventHandler(SelectedToolUsedHandler);
				sideBar.SideTabDeleted += SideTabDeleted;
			}
		}
		
		static string componentLibraryFile = "SharpDevelopControlLibrary.sdcl";
		
		static string GlobalConfigFile {
			get {
				return PropertyService.DataDirectory + Path.DirectorySeparatorChar +
					"options" + Path.DirectorySeparatorChar +
					componentLibraryFile;
			}
		}
		
		static string UserConfigFile {
			get {
				return Path.Combine(PropertyService.ConfigDirectory, componentLibraryFile);
			}
		}
		
		public static void SaveToolbox()
		{
			componentLibraryLoader.SaveToolComponentLibrary(UserConfigFile);
		}
		
		public static void LoadToolbox()
		{
			if (!componentLibraryLoader.LoadToolComponentLibrary(UserConfigFile)) {
				if (!componentLibraryLoader.LoadToolComponentLibrary(GlobalConfigFile)) {
					
					MessageService.ShowWarning("${res:ICSharpCode.SharpDevelop.FormDesigner.ToolboxProvider.CantLoadSidbarComponentLibraryWarning}");
				}
			}
		}
		
		public static void ReloadSideTabs(bool doInsert)
		{
			CreateToolboxService();
			
			sideBar.Tabs.Clear();
			foreach (Category category in componentLibraryLoader.Categories) {
				if (category.IsEnabled) {
					try {
						SideTabDesigner newTab = new SideTabDesigner(sideBar, category, toolboxService);
						newTab.ItemRemoved += SideTabItemRemoved;
						newTab.ItemsExchanged += SideTabItemsExchanged;
						sideBar.Tabs.Add(newTab);
					} catch (Exception e) {
						ICSharpCode.Core.LoggingService.Warn("Can't add tab : " + e);
					}
				}
			}
			if (customTab != null) {
				customTab.Dispose();
			}
			customTab = new CustomComponentsSideTab(sideBar, ResourceService.GetString("ICSharpCode.SharpDevelop.FormDesigner.ToolboxProvider.CustomComponents"), toolboxService);
			customTab.ItemRemoved += SideTabItemRemoved;
			customTab.ItemsExchanged += SideTabItemsExchanged;
			sideBar.Tabs.Add(customTab);
			sideBar.ActiveTab = customTab;
			
			// Clear selected toolbox item after reloading the tabs.
			toolboxService.SetSelectedToolboxItem(null);
		}
		
		static void SelectedToolUsedHandler(object sender, EventArgs e)
		{
			LoggingService.Debug("SelectedToolUsedHandler");
			SideTab tab = sideBar.ActiveTab;
			
			// try to add project reference
			if (sender != null && sender is ICSharpCode.FormsDesigner.Services.ToolboxService) {
				ToolboxItem selectedItem = (sender as IToolboxService).GetSelectedToolboxItem();
				if (tab is CustomComponentsSideTab) {
					if (selectedItem != null && selectedItem.TypeName != null) {
						LoggingService.Debug("Checking for reference to CustomComponent: " + selectedItem.TypeName);
						// Check current project has the custom component first.
						IProjectContent currentProjectContent = ParserService.CurrentProjectContent;
						if (currentProjectContent != null) {
							if (currentProjectContent.GetClass(selectedItem.TypeName, 0) == null) {
								// Check other projects in the solution.
								LoggingService.Debug("Checking other projects in the solution.");
								IProject projectContainingType = FindProjectContainingType(selectedItem.TypeName);
								if (projectContainingType != null) {
									AddProjectReferenceToProject(ProjectService.CurrentProject, projectContainingType);
								}
							}
						}
					}
				} else {
					if (selectedItem != null && selectedItem.AssemblyName != null) {
						IProject currentProject = ProjectService.CurrentProject;
						if (currentProject != null) {
							if (!ProjectContainsReference(currentProject, selectedItem.AssemblyName)) {
								AddReferenceToProject(currentProject, selectedItem.AssemblyName);
							}
						}
					}
				}
			}
			
			if (tab.Items.Count > 0) {
				tab.ChoosedItem = tab.Items[0];
			}
			sideBar.Refresh();
		}
		
		static bool ProjectContainsReference(IProject project, AssemblyName referenceName)
		{
			LoggingService.Debug("Checking project has reference: " + referenceName.FullName);
			bool isAlreadyInRefFolder = false;
			
			foreach (ProjectItem projectItem in project.Items) {
				ReferenceProjectItem referenceItem = projectItem as ReferenceProjectItem;
				if (referenceItem != null) {
					if (referenceItem.ItemType == ItemType.Reference) {
						LoggingService.Debug("Checking project reference: " + referenceItem.Include);
						if (referenceItem.HintPath.Length > 0 && File.Exists(referenceItem.FileName)) {
							LoggingService.Debug("Checking assembly reference");
							AssemblyName assemblyName = AssemblyName.GetAssemblyName(referenceItem.FileName);
							if (assemblyName != null && assemblyName.FullName == referenceName.FullName) {
								isAlreadyInRefFolder = true;
								break;
							}
						} else { // GAC reference.
							LoggingService.Debug("Checking GAC reference");
							if (referenceItem.Include == referenceName.FullName || referenceItem.Include == referenceName.Name) {
								LoggingService.Debug("Found existing GAC reference");
								isAlreadyInRefFolder = true;
								break;
							}
						}
					}
				}
			}
			return isAlreadyInRefFolder;
		}
		
		static void AddReferenceToProject(IProject project, AssemblyName referenceName)
		{
			LoggingService.Warn("Adding reference to project: " + referenceName.FullName);
			ReferenceProjectItem reference = new ReferenceProjectItem(project, "Reference");
			ToolComponent toolComponent = ToolboxProvider.ComponentLibraryLoader.GetToolComponent(referenceName.FullName);
			if (toolComponent == null || toolComponent.HintPath == null) {
				reference.Include = referenceName.FullName;
				LoggingService.Debug("Added GAC reference to project: " + reference.Include);
			} else {
				reference.Include = referenceName.FullName;
				reference.HintPath = FileUtility.GetRelativePath(project.Directory, toolComponent.FileName);
				LoggingService.Debug("Added assembly reference to project: " + reference.Include);
			}
			ProjectService.AddProjectItem(project, reference);
			project.Save();
		}
		
		/// <summary>
		/// Looks for the specified type in all the projects in the open solution
		/// excluding the current project.
		/// </summary>
		static IProject FindProjectContainingType(string type)
		{
			IProject currentProject = ProjectService.CurrentProject;
			if (currentProject == null) {
				return null;
			}
			
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				if (project != currentProject) {
					IProjectContent projectContent = ParserService.GetProjectContent(project);
					if (projectContent != null) {
						if (projectContent.GetClass(type, 0) != null) {
							LoggingService.Debug("Found project containing type: " + project.FileName);
							return project;
						}
					}
				}
			}
			return null;
		}

		static void AddProjectReferenceToProject(IProject project, IProject referenceTo)
		{
			LoggingService.Warn("Adding project reference to project.");
			ProjectReferenceProjectItem reference = new ProjectReferenceProjectItem(project, referenceTo);
			ProjectService.AddProjectItem(project, reference);
			project.Save();
		}
		
		static void SideTabDeleted(object source, SideTabEventArgs e)
		{
			componentLibraryLoader.RemoveCategory(e.SideTab.Name);
			SaveToolbox();
		}
		
		static void SideTabItemRemoved(object source, SideTabItemEventArgs e)
		{
			SideTabDesigner tab = source as SideTabDesigner;
			ToolboxItem toolboxItem = e.Item.Tag as ToolboxItem;
			if (tab != null && toolboxItem != null) {
				componentLibraryLoader.DisableToolComponent(tab.Name, toolboxItem.TypeName);
				SaveToolbox();
			}
		}
		
		static void SideTabItemsExchanged(object source, SideTabItemExchangeEventArgs e)
		{
			SideTabDesigner tab = source as SideTabDesigner;
			ToolboxItem toolboxItem1 = e.Item1.Tag as ToolboxItem;
			ToolboxItem toolboxItem2 = e.Item2.Tag as ToolboxItem;
			if (tab != null && toolboxItem1 != null && toolboxItem2 != null) {
				componentLibraryLoader.ExchangeToolComponents(tab.Name, toolboxItem1.TypeName, toolboxItem2.TypeName);
				SaveToolbox();
			}
		}
	}
}
