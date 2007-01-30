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
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Workflow.Activities;
using ICSharpCode.Core;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets.SideBar;
#endregion

namespace WorkflowDesigner
{
	// TODO - Replace this a class based on System.Drawing.Design.ToolboxService
	public class WorkflowToolboxService : IToolboxService, IServiceProvider
	{
		private string category = "Workflow";
		private static SideTab standardSideTab;
		
		public WorkflowToolboxService(IServiceProvider provider)
		{
			this.provider = provider;
			
			// Make sure the side bar has actually been created!
			if (SharpDevelopSideBar.SideBar == null)
				WorkbenchSingleton.Workbench.GetPad(typeof(SideBarView)).CreatePad();

			if (standardSideTab == null) {
				Assembly assembly = AppDomain.CurrentDomain.Load("System.Workflow.Activities, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
				standardSideTab = CreateSideTabFromAssembly("Workflow", assembly);
			}
		}

		#region IServiceProvider implementation
		IServiceProvider provider;
		public object GetService(Type serviceType)
		{
			return provider.GetService(serviceType);
		}
		#endregion
		
		
		public void ShowSideTabs()
		{
			// Make sure the standard workflow sidebar is on screen.
			if (!SharpDevelopSideBar.SideBar.Tabs.Contains(standardSideTab)) {
				SharpDevelopSideBar.SideBar.Tabs.Add(standardSideTab);
			}
			
			SharpDevelopSideBar.SideBar.Refresh();
		}
		
		public CategoryNameCollection CategoryNames {
			get {
				return new CategoryNameCollection(new string[] { "Workflow" });
			}
		}
		
		public string SelectedCategory {
			get {
				return category;
			}
			set {
				category = value;
			}
		}
		
		public void AddCreator(ToolboxItemCreatorCallback creator, string format)
		{
			throw new NotImplementedException();
		}
		
		public void AddCreator(ToolboxItemCreatorCallback creator, string format, System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public void AddToolboxItem(ToolboxItem toolboxItem)
		{
			throw new NotImplementedException();
		}
		
		public void AddToolboxItem(ToolboxItem toolboxItem, string category)
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItem DeserializeToolboxItem(object serializedObject)
		{
			return DeserializeToolboxItem(serializedObject, null);
		}
		
		public ToolboxItem DeserializeToolboxItem(object serializedObject, System.ComponentModel.Design.IDesignerHost host)
		{
			IDataObject dataObject = (System.Windows.Forms.IDataObject)serializedObject;

			SharpDevelopSideTabItem sti = (SharpDevelopSideTabItem)dataObject.GetData(typeof(SharpDevelopSideTabItem));
			ToolboxItem toolboxItem = (ToolboxItem)sti.Tag;

			return toolboxItem;
		}
		
		public ToolboxItem GetSelectedToolboxItem()
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItem GetSelectedToolboxItem(System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItemCollection GetToolboxItems()
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItemCollection GetToolboxItems(System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItemCollection GetToolboxItems(string category)
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItemCollection GetToolboxItems(string category, System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public bool IsSupported(object serializedObject, System.ComponentModel.Design.IDesignerHost host)
		{
			return true;
		}
		
		public bool IsSupported(object serializedObject, System.Collections.ICollection filterAttributes)
		{
			return true;
		}
		
		public bool IsToolboxItem(object serializedObject)
		{
			throw new NotImplementedException();
		}
		
		public bool IsToolboxItem(object serializedObject, System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public void Refresh()
		{
			throw new NotImplementedException();
		}
		
		public void RemoveCreator(string format)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveCreator(string format, System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveToolboxItem(ToolboxItem toolboxItem)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveToolboxItem(ToolboxItem toolboxItem, string category)
		{
			throw new NotImplementedException();
		}
		
		public void SelectedToolboxItemUsed()
		{
			throw new NotImplementedException();
		}
		
		public object SerializeToolboxItem(ToolboxItem toolboxItem)
		{
			throw new NotImplementedException();
		}
		
		public bool SetCursor()
		{
			throw new NotImplementedException();
		}
		
		public void SetSelectedToolboxItem(ToolboxItem toolboxItem)
		{
			throw new NotImplementedException();
		}
		
		#region Helpers
		static SideTab CreateSideTabFromAssembly(Assembly assembly)
		{
			return CreateSideTabFromAssembly(assembly.FullName + " components", assembly);
		}

		static SideTab CreateSideTabFromAssembly(string name, Assembly assembly)
		{
			SideTab sideTab = new SideTab(name);
			sideTab.CanSaved = false;
			AddPointerToSideTab(sideTab);
			LoadSideTabItemsFromAssembly(assembly, sideTab);
			return sideTab;
		}
		
		static void AddPointerToSideTab(SideTab sideTab)
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
		

		static void LoadSideTabItemsFromAssembly(Assembly assembly, SideTab sideTab)
		{
			ICollection toolboxItems = System.Drawing.Design.ToolboxService.GetToolboxItems(assembly.GetName());
			foreach (ToolboxItem tbi in toolboxItems)
			{
				SharpDevelopSideTabItem sti = new SharpDevelopSideTabItem(tbi.DisplayName);
				sti.CanBeDeleted = false;
				sti.CanBeRenamed = false;
				sti.Tag = tbi;
				sti.Icon = tbi.Bitmap;
				sideTab.Items.Add(sti);
			}
		}
		#endregion
	}

}
