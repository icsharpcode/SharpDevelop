// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Design;
using System.ComponentModel.Design;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets.SideBar;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// This class manages the display of the SideTabs in the tools pad.
	/// There a three types of SideTab:
	/// 	Standard   - contains the standard workflow components from System.Workflow.Activities.
	/// 	Project    - contains the activities in the current project.
	/// 	References - One sidetab for each project assembly reference that contains activities.
	/// </summary>
	public sealed class WorkflowSideTabService
	{
		private static IViewContent activeViewContent;
		private static IProject activeProject;
		private static SideTab standardSideTab;
		private static int viewCount;
		private static Dictionary<IProject, Dictionary<ReferenceProjectItem, SideTab>> projects;
		private static bool initialised;
		
		
		#region Properties
		private static IProject ActiveProject {
			get { return activeProject; }
			set {
				if (activeProject == value)
					return;
				
				if (value == null)
					RemoveAllSideTabs();
				else {
					if (activeProject != null)
						RemoveProjectSideTabs(activeProject);
					ShowSideTabsForProject(value);
				}
				
				activeProject = value;
			}
		}
		
		private static Dictionary<IProject, Dictionary<ReferenceProjectItem, SideTab>> Projects {
			get {
				if (projects == null)
					projects = new Dictionary<IProject, Dictionary<ReferenceProjectItem, SideTab>> ();
				
				return projects;
			}
		}
		
		private static int ViewCount {
			get { return viewCount; }
			set {
				viewCount = value;
				
				if (viewCount == 0)	{
					standardSideTab = null;
					ActiveProject = null;
				}
			}
		}
		
		static SharpDevelopSideBar workflowSideBar;
		
		public static SharpDevelopSideBar WorkflowSideBar {
			get {
				Debug.Assert(WorkbenchSingleton.InvokeRequired == false);
				if (workflowSideBar == null) {
					workflowSideBar = new SharpDevelopSideBar();
					
				}
				return workflowSideBar;
			}
		}
		
		#endregion
		
		private static void Initialise()
		{
			// Make sure the side bar has actually been created!
			ProjectService.ProjectItemRemoved += ProjectItemRemovedEventHandler;
			ProjectService.ProjectItemAdded += ProjectItemAddedEventHandler;
			ProjectService.SolutionClosing += SolutionClosingEventHandler;
			
			initialised = true;
		}
		
		public static void AddViewContent(IViewContent viewContent)
		{
			if (viewContent == null)
				throw new ArgumentNullException("viewContent");
			
			if (!initialised)
				Initialise();
			
			// Make sure the standard workflow sidebar exists
			if (standardSideTab == null) {
				standardSideTab = CreateSideTabFromAssembly("Workflow", new AssemblyName("System.Workflow.Activities, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"));
				LoadSideTabItemsFromAssembly(new AssemblyName("System.Workflow.ComponentModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), standardSideTab);
			}

			// Attach the handlers.
			viewContent.SwitchedTo += ViewContentActivatedEventHandler;
			viewContent.Disposed += ViewContentDisposedEventHandler;
			
			ViewCount++;
		}
		
		
		#region ProjectService event handlers
		private static void ProjectItemAddedEventHandler(object sender, ProjectItemEventArgs e)
		{
			if (e.Project == null) return;
			if (!Projects.ContainsKey(e.Project)) return;
			
			ReferenceProjectItem item = e.ProjectItem as ReferenceProjectItem;
			if (item == null) return;
			
			Dictionary<ReferenceProjectItem, SideTab> references = Projects[e.Project];
			if (item is ProjectReferenceProjectItem) {
				references.Add(item, CreateSideTabForProjectItem(item));
				return;
			} else if (item is ReferenceProjectItem) {
				
				if (!e.ProjectItem.Include.StartsWith("System")){
					references.Add(item, CreateSideTabForProjectItem(item));
				}
				
			} else {
				return;
			}
			
			if (ActiveProject == e.Project)
			{
				if (!WorkflowSideBar.Tabs.Contains(references[item])) {
					WorkflowSideBar.Tabs.Add(references[item]);
				}
			}
		}

		private static void ProjectItemRemovedEventHandler(object sender, ProjectItemEventArgs e)
		{
			if (e.Project == null) return;
			if (!Projects.ContainsKey(e.Project)) return;
			
			ReferenceProjectItem item = e.ProjectItem as ReferenceProjectItem;
			if (item == null) return;
			
			Dictionary<ReferenceProjectItem, SideTab> references = Projects[e.Project];
			
			if (references.ContainsKey(item)){
				if (WorkflowSideBar.Tabs.Contains(references[item])) {
					WorkflowSideBar.Tabs.Remove(references[item]);
				}
				references.Remove(item);
			}
		}
		
		private static void SolutionClosingEventHandler(object sender, SolutionEventArgs e)
		{
			foreach (IProject project in e.Solution.Projects) {
				if (Projects.ContainsKey(project)) {
					RemoveProjectSideTabs(project);
					Projects.Remove(project);
				}
			}
		}
		
		
		#endregion
		
		#region IViewContent event handlers
		private static void ViewContentDisposedEventHandler(object sender, EventArgs args)
		{
			LoggingService.DebugFormatted("ViewContentDisposedEventHandler {0}", sender);

			ViewCount--;
			
		}

		private static void ViewContentActivatedEventHandler(object sender, EventArgs args)
		{
			LoggingService.DebugFormatted("ViewActivated {0}", sender);

			if (activeViewContent == sender)
				return;

			// Make sure the standard workflow sidebar is on screen.
			if (!WorkflowSideBar.Tabs.Contains(standardSideTab)) {
				WorkflowSideBar.Tabs.Add(standardSideTab);
				if (WorkflowSideBar.Tabs.Count == 1) {
					WorkflowSideBar.ActiveTab = WorkflowSideBar.Tabs[0];
				}
			}

			
			LoggingService.DebugFormatted("ViewActivated {0}", sender);
			activeViewContent = sender as IViewContent;
			ActiveProject = ProjectService.OpenSolution.FindProjectContainingFile(activeViewContent.PrimaryFileName);
			
			WorkflowSideBar.Refresh();
		}
		#endregion;
		
		private static void ShowSideTabsForProject(IProject project)
		{
			if (!Projects.ContainsKey(project)){
				Dictionary<ReferenceProjectItem, SideTab> tabs = new Dictionary<ReferenceProjectItem, SideTab>();
				tabs.Add(new ReferenceProjectItem(project), CreateCustomComponentsSideTab(project));
				LoadProjectReferenceSideTabs(project, tabs);
				Projects.Add(project, tabs);
			}
			
			Dictionary<ReferenceProjectItem, SideTab> references = Projects[project];
			foreach (SideTab sideTab in references.Values) {
				if (sideTab.Items.Count > 1) {
					if (!WorkflowSideBar.Tabs.Contains(sideTab)) {
						WorkflowSideBar.Tabs.Add(sideTab);
					}
				}
			}
		}
		
		private static SideTab CreateCustomComponentsSideTab(IProject project)
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.CodeBase = project.OutputAssemblyFullPath;
			
			SideTab sideTab = new SideTab("Project components");
			sideTab.CanSaved = false;
			AddPointerToSideTab(sideTab);

			// TODO: Need to load the sidetab with activities from the current project.
			//       Cannot use LoadSideTabFromAssembly as it will only
			//		 load public components from the assembly.
			
			IProjectContent projectContent = ParserService.GetProjectContent(project);
			foreach (IProjectContent pc in  projectContent.ReferencedContents){
				//LoggingService.DebugFormatted(pc.ToString());
			}
			
			SortSideTabItems(sideTab);
			return sideTab;
		}
		
		private static void RemoveAllSideTabs()
		{
			foreach (IProject project in Projects.Keys) {
				RemoveProjectSideTabs(project);
			}
		}
		
		private static void RemoveProjectSideTabs(IProject project)
		{
			if (!Projects.ContainsKey(project))
				return;
			
			Dictionary<ReferenceProjectItem, SideTab> references = Projects[project];
			foreach (SideTab sideTab in references.Values) {
				if (WorkflowSideBar.Tabs.Contains(sideTab)) {
					WorkflowSideBar.Tabs.Remove(sideTab);
				}
			}
		}
		
		private static void LoadProjectReferenceSideTabs(IProject project, Dictionary<ReferenceProjectItem, SideTab> tabs)
		{
			foreach (ProjectItem item in project.Items) {
				if (item is ProjectReferenceProjectItem) {
					tabs.Add(item as ReferenceProjectItem, CreateSideTabForProjectItem(item));
					
				} else 	if (item is ReferenceProjectItem) {
					if (!item.Include.StartsWith("System")){
						tabs.Add(item as ReferenceProjectItem, CreateSideTabForProjectItem(item));
					}
				}
			}
		}
		
		private static SideTab CreateSideTabForProjectItem(ProjectItem item)
		{
			AssemblyName assemblyName = null;
			
			if (item is ProjectReferenceProjectItem) {
				ProjectReferenceProjectItem pitem = item as ProjectReferenceProjectItem;
				assemblyName = new AssemblyName();
				
				assemblyName.CodeBase = pitem.ReferencedProject.OutputAssemblyFullPath;
				return CreateSideTabFromAssembly(pitem.ReferencedProject.Name, assemblyName);
				
			} else if (item is ReferenceProjectItem) {
				assemblyName = new AssemblyName();
				assemblyName.CodeBase = item.FileName;
				return CreateSideTabFromAssembly(Path.GetFileNameWithoutExtension(item.FileName) + " components",assemblyName);
			}
			
			return null;
		}
		
		private static SideTab CreateSideTabFromAssembly(AssemblyName assemblyName)
		{
			return CreateSideTabFromAssembly(assemblyName.FullName + " components", assemblyName);
		}

		private static SideTab CreateSideTabFromAssembly(string name, AssemblyName assemblyName)
		{
			SideTab sideTab = new SideTab(name);
			sideTab.CanSaved = false;
			AddPointerToSideTab(sideTab);
			LoadSideTabItemsFromAssembly(assemblyName, sideTab);
			SortSideTabItems(sideTab);
			return sideTab;
		}
		
		private static void AddPointerToSideTab(SideTab sideTab)
		{
			// Add the standard pointer.
			SharpDevelopSideTabItem sti = new SharpDevelopSideTabItem("Pointer");
			sti.CanBeRenamed = false;
			sti.CanBeDeleted = false;
			Bitmap pointerBitmap = new Bitmap(IconService.GetBitmap("Icons.16x16.FormsDesigner.PointerIcon"), 16, 16);
			sti.Icon = pointerBitmap;
			sti.Tag = null;
			sideTab.Items.Add(sti);
		}
		
		private static void LoadSideTabItemsFromAssembly(AssemblyName assemblyName, SideTab sideTab)
		{
			ICollection toolboxItems = System.Drawing.Design.ToolboxService.GetToolboxItems(assemblyName);
			
			foreach (ToolboxItem tbi in toolboxItems)
			{
				//TODO: Add further checking to see if this component can actually be put on the sidetab.
				
				SharpDevelopSideTabItem sti = new SharpDevelopSideTabItem(tbi.DisplayName);
				sti.CanBeDeleted = false;
				sti.CanBeRenamed = false;
				sti.Tag = tbi;
				sti.Icon = tbi.Bitmap;
				sideTab.Items.Add(sti);
			}
			
			System.Drawing.Design.ToolboxService.UnloadToolboxItems();
		}
		
		private static void SortSideTabItems(SideTab sideTab)
		{
			SortedDictionary<string, SideTabItem> list = new SortedDictionary<string, SideTabItem>();
			
			SideTabItem pointer = sideTab.Items[0];
			
			sideTab.Items.RemoveAt(0);
			foreach (SideTabItem item in sideTab.Items)
				list.Add(item.Name, item);
			
			sideTab.Items.Clear();
			sideTab.Items.Add(pointer);
			foreach (SideTabItem item in list.Values)
				sideTab.Items.Add(item);
			
		}
	}
}
