// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Activities;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Drawing.Design;
using System.Reflection;

using ICSharpCode.SharpDevelop;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets.SideBar;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
#endregion

namespace WorkflowDesigner
{
	

	/// <summary>
	/// Description of ViewContentControl.
	/// </summary>
	public partial class ViewContentControl : UserControl, IHasPropertyContainer
	{
		private DesignSurface designSurface;
		private	BasicDesignerLoader loader;
		private IViewContent viewContent;

		// HACK: Temporary sidebar creation.
		static SideTab	sideTab;
		
		static ViewContentControl()
		{
			sideTab = new SideTab("Workflow");
			
			// Make sure the side bar has actually bee created!
			if (SharpDevelopSideBar.SideBar == null)
				WorkbenchSingleton.Workbench.GetPad(typeof(SideBarView)).CreatePad();
			
			sideTab.CanSaved = false;
			
			// Add the standard pointer.
			SharpDevelopSideTabItem sti = new SharpDevelopSideTabItem("Pointer");
			sti.CanBeRenamed = false;
			sti.CanBeDeleted = false;
			Bitmap pointerBitmap = new Bitmap(IconService.GetBitmap("Icons.16x16.FormsDesigner.PointerIcon"), 16, 16);
			sti.Icon = pointerBitmap;
			sti.Tag = null;
			sideTab.Items.Add(sti);


			// Load activities from the standard assembly.
			Assembly assembly = AppDomain.CurrentDomain.Load("System.Workflow.Activities, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			ICollection toolboxItems = System.Drawing.Design.ToolboxService.GetToolboxItems(assembly.GetName());
			foreach (ToolboxItem tbi in toolboxItems)
			{
				sti = new SharpDevelopSideTabItem(tbi.DisplayName);
				sti.CanBeDeleted = false;
				sti.CanBeRenamed = false;
				sti.Tag = tbi;
				sti.Icon = tbi.Bitmap;
				sideTab.Items.Add(sti);
			}
			
		}
		
		public ViewContentControl(IViewContent viewContent)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.viewContent = viewContent;
			
			// Make sure the standard workflow sidebar is on screen.
			if (!SharpDevelopSideBar.SideBar.Tabs.Contains(sideTab)) {
				SharpDevelopSideBar.SideBar.Tabs.Add(sideTab);
				SharpDevelopSideBar.SideBar.Refresh();
			}
		}
		
		public IDesignerHost DesignerHost {
			get {
				if (designSurface == null)
					return null;
				
				return (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
			}
		}

		public IRootDesigner RootDesigner
		{
			get
			{
				return DesignerHost.GetDesigner(DesignerHost.RootComponent) as IRootDesigner;
			}
		}

		public WorkflowView WorkflowView
		{
			get
			{
				return (WorkflowView)RootDesigner.GetView(ViewTechnology.Default);
			}
		}

		
		
		internal void LoadWorkflow(BasicDesignerLoader loader)
		{
			
			this.loader = loader;

			try {
				
				LoadDesigner();
				
				if (designSurface != null && Controls.Count == 0) {
					Control designer = designSurface.View as Control;
					designer.Dock = DockStyle.Fill;
					Controls.Add(designer);
					Selected(); // HACK: Selected event not working so calling directly at present
				}
			} catch (Exception e) {
				TextBox errorText = new TextBox();
				errorText.Multiline = true;
				if (!designSurface.IsLoaded && designSurface.LoadErrors != null) {
					errorText.Text = "Error loading designer:\r\n\r\n";
					foreach(Exception le in designSurface.LoadErrors) {
						errorText.Text += le.ToString();
						errorText.Text += "\r\n";
					}
				} else {
					errorText.Text = e.ToString();
				}
				
				errorText.Dock = DockStyle.Fill;
				Controls.Add(errorText);
				errorText.BringToFront();
				Control title = new Label();
				title.Text = "Failed to load designer. Check the source code for syntax errors and check if all references are available.";
				title.Dock = DockStyle.Top;
				Controls.Add(title);
			}
		}
		
		internal void SaveWorkflow(Stream stream)
		{
			loader.Flush();
			
			if (loader is XomlDesignerLoader) {
				using (StreamWriter w = new StreamWriter(stream)) {
					w.Write(((XomlDesignerLoader)loader).Xoml);
				}
			}
		}
		
		internal void UnloadWorkflow()
		{
			UnloadDesigner();
		}
		
		void UnloadDesigner()
		{
			this.Controls.Clear();
			designSurface.Dispose();
			designSurface = null;
			
		}
		
		void LoadDesigner()
		{

			designSurface = new DesignSurface();
			designSurface.BeginLoad(loader);

			// Monitor for updates and make the view dirty.
			IComponentChangeService componentChangeService = (IComponentChangeService)designSurface.GetService(typeof(IComponentChangeService));
			//componentChangeService.ComponentAdding += new ComponentEventHandler(ComponentAddingHandler);
			componentChangeService.ComponentAdded += new ComponentEventHandler(ComponentAddedHandler);
			componentChangeService.ComponentChanged += new ComponentChangedEventHandler(ComponentChangedHandler);

			// Attach the selection service to capture objects selection changes to update property pad.
			ISelectionService selectionService = (ISelectionService)designSurface.GetService(typeof(ISelectionService));
			selectionService.SelectionChanged += new EventHandler(SelectionChangedHandler);
			
		}
		
		void UpdateCCU()
		{
			IWorkflowDesignerEventBindingService srv = this.DesignerHost.GetService(typeof(IEventBindingService)) as IWorkflowDesignerEventBindingService;
			srv.UpdateCCU();
		}
		
		void ComponentAddedHandler(object sender, ComponentEventArgs args)
		{
			UpdateCCU();
			LoggingService.Debug("ComponentAddedHandler");
		}
		
		void ComponentAddingHandler(object sender, ComponentEventArgs args)
		{
			LoggingService.Debug("ComponentAddingHandler");
		}

		void ComponentChangedHandler(object sender, ComponentChangedEventArgs args)
		{
			UpdateCCU();
			viewContent.PrimaryFile.MakeDirty();
			ISelectionService selectionService = (ISelectionService)designSurface.GetService(typeof(ISelectionService));
			UpdatePropertyPadSelection(selectionService);
		}

		void SelectionChangedHandler(object sender, EventArgs args)
		{
			UpdatePropertyPadSelection((ISelectionService)sender);
		}

		void UpdatePropertyPadSelection(ISelectionService selectionService)
		{
			ICollection selection = selectionService.GetSelectedComponents();
			object[] selArray = new object[selection.Count];
			selection.CopyTo(selArray, 0);
			propertyContainer.SelectableObjects = DesignerHost.Container.Components;
			propertyContainer.Host = DesignerHost;
			propertyContainer.SelectedObjects = selArray;
			PropertyPad.Grid.CommandsVisibleIfAvailable = false;
		}
		
		internal void Selected()
		{
			WorkflowToolboxService srv = this.DesignerHost.GetService(typeof(IToolboxService)) as WorkflowToolboxService;
			//srv.ShowSideTabs();
		}
		
		internal void Deselected()
		{
			propertyContainer.SelectableObjects = null;
			propertyContainer.Host = null;
			propertyContainer.SelectedObjects = null;
		}
		
		#region IHasPropertyContainer
		PropertyContainer propertyContainer = new PropertyContainer();
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
		}
		#endregion
		
		
	}
}
