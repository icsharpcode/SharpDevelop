// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
					
					MessageService.ShowWarning("${res:ICSharpCode.FormDesigner.ToolboxProvider.CantLoadSidbarComponentLibraryWarning}");
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
						Console.WriteLine("Can't add tab : " + e);
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
			AxSideTab tab = SharpDevelopSideBar.SideBar.ActiveTab;
			
			// try to add project reference
			if (sender != null && sender is ICSharpCode.FormDesigner.Services.ToolboxService && !(tab is CustomComponentsSideTab)) {
				ToolboxItem selectedItem = (sender as IToolboxService).GetSelectedToolboxItem();
				if (selectedItem != null) {
					if (selectedItem.AssemblyName != null) {
//	TODO: Project system...		
//						//We Put the assembly reference into the reference project folder
//						IProject currentProject = ProjectService.CurrentProject;
//						
//						if (currentProject != null) {
//							bool isAlreadyInRefFolder = false;
//							
//							if (currentProject.ProjectType == "C#" || currentProject.ProjectType == "VBNET") {
//								foreach (string assembly in DefaultParserService.AssemblyList) {
//									if (selectedItem.AssemblyName.FullName.StartsWith(assembly + ",")) {
//										isAlreadyInRefFolder = true;
//										break;
//									}
//								}
//							}
//							
//							foreach (ProjectReference refproj in currentProject.ProjectReferences) {
//								if (refproj.ReferenceType == ReferenceType.Assembly) {
//									AssemblyName assemblyName = AssemblyName.GetAssemblyName(refproj.Reference);
//									if (assemblyName != null && assemblyName.FullName == selectedItem.AssemblyName.FullName) {
//										isAlreadyInRefFolder = true;
//										break;
//									}
//								} else if (refproj.ReferenceType == ReferenceType.Gac) {
//									if (refproj.Reference == selectedItem.AssemblyName.FullName) {
//										isAlreadyInRefFolder = true;
//										break;
//									}
//								}
//							}
//							
//							if (!isAlreadyInRefFolder && !selectedItem.AssemblyName.FullName.StartsWith("System.")) {
//								ToolComponent toolComponent = ToolboxProvider.ComponentLibraryLoader.GetToolComponent(selectedItem.AssemblyName.FullName);
//								if (toolComponent == null || toolComponent.HintPath == null) {
//									currentProject.ProjectReferences.Add(new ProjectReference(ReferenceType.Gac, selectedItem.AssemblyName.FullName));
//								} else {
//									currentProject.ProjectReferences.Add(new ProjectReference(ReferenceType.Assembly, toolComponent.FileName));
//								}
//								ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ProjectBrowserView pbv = (ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ProjectBrowserView)WorkbenchSingleton.Workbench.GetPad(typeof(ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ProjectBrowserView));
//								pbv.UpdateCombineTree();
//								projectService.SaveCombine();
//							}
//						} 
					}
				}
			}
			
			if (tab.Items.Count > 0) {
				tab.ChoosedItem = tab.Items[0];
			}
			SharpDevelopSideBar.SideBar.Refresh();
		}
	}
}
