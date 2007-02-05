// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Design;
using System.ComponentModel.Design;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets.SideBar;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
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
						RemoveSideTabsForProject(activeProject);
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
					if (SharpDevelopSideBar.SideBar.Tabs.Contains(standardSideTab)) {
						SharpDevelopSideBar.SideBar.Tabs.Remove(standardSideTab);
						standardSideTab = null;
					}

					ActiveProject = null;
					
					SharpDevelopSideBar.SideBar.Refresh();
					
				}
			}
		}
		#endregion
		
		private static void Initialise()
		{
			// Make sure the side bar has actually been created!
			if (SharpDevelopSideBar.SideBar == null)
				WorkbenchSingleton.Workbench.GetPad(typeof(SideBarView)).CreatePad();

			ProjectService.ProjectItemRemoved += ProjectItemRemovedEventHandler;
			ProjectService.ProjectItemAdded += ProjectItemAddedEventHandler;
			
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
				Assembly assembly = AppDomain.CurrentDomain.Load("System.Workflow.Activities, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
				standardSideTab = CreateSideTabFromAssembly("Workflow", assembly);
				assembly = AppDomain.CurrentDomain.Load("System.Workflow.ComponentModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
				LoadSideTabItemsFromAssembly(assembly, standardSideTab);
			}

			// Attach the handlers.
			viewContent.SwitchedTo += ViewContentActivatedEventHandler;
			viewContent.Disposed += ViewContentDisposedEventHandler;
			
			ViewCount++;
		}
		
		
		#region ProjectService ProjectItems added/removed handlers
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
				if (!SharpDevelopSideBar.SideBar.Tabs.Contains(references[item])) {
					SharpDevelopSideBar.SideBar.Tabs.Add(references[item]);
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
				if (SharpDevelopSideBar.SideBar.Tabs.Contains(references[item])) {
					SharpDevelopSideBar.SideBar.Tabs.Remove(references[item]);
				}
				references.Remove(item);
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
			if (!SharpDevelopSideBar.SideBar.Tabs.Contains(standardSideTab)) {
				SharpDevelopSideBar.SideBar.Tabs.Add(standardSideTab);
			}

			ActiveProject = ProjectService.CurrentProject;
			
			LoggingService.DebugFormatted("ViewActivated {0}", sender);
			activeViewContent = sender as IViewContent;
			
			SharpDevelopSideBar.SideBar.Refresh();
		}
		#endregion;
		
		private static void ShowSideTabsForProject(IProject project)
		{
			if (!Projects.ContainsKey(project)){
				Dictionary<ReferenceProjectItem, SideTab> tabs = new Dictionary<ReferenceProjectItem, SideTab>();
				LoadProjectReferenceSideTabs(project, tabs);
				Projects.Add(project, tabs);
			}
			
			Dictionary<ReferenceProjectItem, SideTab> references = Projects[project];
			foreach (SideTab sideTab in references.Values) {
				if (!SharpDevelopSideBar.SideBar.Tabs.Contains(sideTab)) {
					SharpDevelopSideBar.SideBar.Tabs.Add(sideTab);
				}
			}
		}
		
		private static void RemoveAllSideTabs()
		{
			foreach (IProject project in Projects.Keys) {
				RemoveSideTabsForProject(project);
			}
		}
		
		private static void RemoveSideTabsForProject(IProject project)
		{
			if (!Projects.ContainsKey(project))
				return;
			
			Dictionary<ReferenceProjectItem, SideTab> references = Projects[project];
			foreach (SideTab sideTab in references.Values) {
				if (SharpDevelopSideBar.SideBar.Tabs.Contains(sideTab)) {
					SharpDevelopSideBar.SideBar.Tabs.Remove(sideTab);
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
			Assembly assembly = null;
			
			if (item is ProjectReferenceProjectItem) {
				ProjectReferenceProjectItem pitem = item as ProjectReferenceProjectItem;
				AssemblyName name = new AssemblyName();
				
				name.CodeBase = pitem.ReferencedProject.OutputAssemblyFullPath;
				assembly = AppDomain.CurrentDomain.Load(name);
				
			} else if (item is ReferenceProjectItem) {
				AssemblyName name = new AssemblyName();
				name.CodeBase = item.FileName;
				assembly = AppDomain.CurrentDomain.Load(name);
			}
			
			return CreateSideTabFromAssembly(assembly);
		}
		
		private static SideTab CreateSideTabFromAssembly(Assembly assembly)
		{
			return CreateSideTabFromAssembly(assembly.GetName().Name + " components", assembly);
		}

		private static SideTab CreateSideTabFromAssembly(string name, Assembly assembly)
		{
			SideTab sideTab = new SideTab(name);
			sideTab.CanSaved = false;
			AddPointerToSideTab(sideTab);
			LoadSideTabItemsFromAssembly(assembly, sideTab);
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
		
		private static void LoadSideTabItemsFromAssembly(Assembly assembly, SideTab sideTab)
		{
			ICollection toolboxItems = System.Drawing.Design.ToolboxService.GetToolboxItems(assembly.GetName());
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
