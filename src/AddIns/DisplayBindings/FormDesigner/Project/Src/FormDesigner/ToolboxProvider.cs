// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

using ICSharpCode.Core;

using ICSharpCode.FormDesigner.Services;
using ICSharpCode.FormDesigner.Gui;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace ICSharpCode.FormDesigner
{
	public class ToolboxProvider
	{
		static ICSharpCode.FormDesigner.Services.ToolboxService         toolboxService = null;
		static ITypeResolutionService typeResolutionService = new TypeResolutionService();
		public static ArrayList       SideTabs = new ArrayList();
		
		static ComponentLibraryLoader componentLibraryLoader = new ComponentLibraryLoader();
		
		public static ITypeResolutionService TypeResolutionService {
			get {
				return typeResolutionService;
			}
		}
		
		public static ComponentLibraryLoader ComponentLibraryLoader {
			get {
				return componentLibraryLoader;
			}
		}
		public static ICSharpCode.FormDesigner.Services.ToolboxService ToolboxService {
			get {
				if (toolboxService == null) {
					toolboxService = new ICSharpCode.FormDesigner.Services.ToolboxService();
					ReloadSideTabs(false);
					toolboxService.SelectedItemUsed += new EventHandler(SelectedToolUsedHandler);
				}
				return toolboxService;
			}
		}
		
		static ToolboxProvider()
		{
			PadDescriptor pad = WorkbenchSingleton.Workbench.GetPad(typeof(SideBarView));
			pad.CreatePad();
			LoadToolbox();
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
			bool reInsertTabs = false;
			foreach(AxSideTab tab in SideTabs) {
				if (SharpDevelopSideBar.SideBar.Tabs.Contains(tab)) {
					SharpDevelopSideBar.SideBar.Tabs.Remove(tab);
					reInsertTabs = true;;
				}
			}
			reInsertTabs &= doInsert;
			
			SideTabs.Clear();
			foreach (Category category in componentLibraryLoader.Categories) {
				if (category.IsEnabled) {
					try {
						SideTabDesigner newTab = new SideTabDesigner(SharpDevelopSideBar.SideBar, category, toolboxService);
						SideTabs.Add(newTab);
					} catch (Exception e) {
						ICSharpCode.Core.LoggingService.Warn("Can't add tab : " + e);
					}
				}
			}
			SideTabDesigner customTab = new CustomComponentsSideTab(SharpDevelopSideBar.SideBar, "Custom Components", toolboxService);
			SideTabs.Add(customTab);
			if (reInsertTabs) {
				foreach(AxSideTab tab in SideTabs) {
					SharpDevelopSideBar.SideBar.Tabs.Add(tab);
				}
			}
		}
		
		static void SelectedToolUsedHandler(object sender, EventArgs e)
		{
			LoggingService.Debug("SelectedToolUsedHandler");
			AxSideTab tab = SharpDevelopSideBar.SideBar.ActiveTab;
						
			// try to add project reference
			if (sender != null && sender is ICSharpCode.FormDesigner.Services.ToolboxService) {
				ToolboxItem selectedItem = (sender as IToolboxService).GetSelectedToolboxItem();
				if (tab is CustomComponentsSideTab) {
					if (selectedItem != null && selectedItem.TypeName != null) {
						LoggingService.Debug("Checking for reference to CustomComponent: " + selectedItem.TypeName);
						// Check current project has the custom component first.
						IProjectContent currentProjectContent = ParserService.CurrentProjectContent;
						if (currentProjectContent != null) {
							if (currentProjectContent.GetClass(selectedItem.TypeName) == null) {
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
			SharpDevelopSideBar.SideBar.Refresh();
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
						if (referenceItem.HintPath.Length > 0) {
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
			LoggingService.Debug("Adding reference to project: " + referenceName.FullName);
			ReferenceProjectItem reference = new ReferenceProjectItem(project, "Reference");
			ToolComponent toolComponent = ToolboxProvider.ComponentLibraryLoader.GetToolComponent(referenceName.FullName);
			if (toolComponent == null || toolComponent.HintPath == null) {
				reference.Include = referenceName.FullName;
				LoggingService.Debug("Added GAC reference to project: " + reference.Include);
			} else {
				reference.Include = referenceName.FullName;
				reference.HintPath = FileUtility.GetRelativePath(project.Directory, toolComponent.FileName);
				reference.SpecificVersion = false;
				LoggingService.Debug("Added assembly reference to project: " + reference.Include);
			}
			ProjectService.AddProjectItem(project, reference);
			ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
			project.Save();
		}
		
		/// <summary>
		/// Looks for the specified type in all the projects in the open solution
		/// excluding the current project.
		/// </summary>
		static IProject FindProjectContainingType(string type)
		{
			IProject currentProject = ProjectService.CurrentProject;
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				if (project != currentProject) {
					IProjectContent projectContent = ParserService.GetProjectContent(project);
					if (projectContent != null) {
						if (projectContent.GetClass(type) != null) {
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
			LoggingService.Debug("Adding project reference to project.");
			ProjectReferenceProjectItem reference = new ProjectReferenceProjectItem(project, referenceTo);
			ProjectService.AddProjectItem(project, reference);
			ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
			project.Save();
		}
	}
}
