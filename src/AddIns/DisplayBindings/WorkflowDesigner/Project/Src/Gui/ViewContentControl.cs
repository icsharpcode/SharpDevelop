// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.ComponentModel.Design;
using System.Collections;
using System.Windows.Forms;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Activities;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Drawing.Design;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

using WorkflowDesigner.Loaders;
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
		private bool addedSideTab;

		public ViewContentControl(IViewContent viewContent)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.viewContent = viewContent;
			
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

			StatusBarService.SetMessage("Loading workflow " + Path.GetFileName(viewContent.PrimaryFileName) + "...");
			Application.UseWaitCursor = true;
			Application.DoEvents();
			
			if (!addedSideTab){
				WorkflowSideTabService.AddViewContent(this.viewContent);
				addedSideTab = true;
			}

			this.loader = loader;

			try {
				
				LoadDesigner();
				
				if (designSurface != null && Controls.Count == 0) {
					Control designer = designSurface.View as Control;
					designer.Dock = DockStyle.Fill;
					Controls.Add(designer);
					DesignerHost.Activate();
				}
			} catch (Exception e) {
				TextBox errorText = new TextBox();
				errorText.Multiline = true;
				errorText.ScrollBars = ScrollBars.Both;
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
			} finally {
				StatusBarService.SetMessage(String.Empty);
				Application.UseWaitCursor = false;
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
		
		void ComponentAddedHandler(object sender, ComponentEventArgs args)
		{
			viewContent.PrimaryFile.MakeDirty();
			LoggingService.Debug("ComponentAddedHandler");
		}
		
		void ComponentAddingHandler(object sender, ComponentEventArgs args)
		{
			LoggingService.Debug("ComponentAddingHandler");
		}

		void ComponentChangedHandler(object sender, ComponentChangedEventArgs args)
		{
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
