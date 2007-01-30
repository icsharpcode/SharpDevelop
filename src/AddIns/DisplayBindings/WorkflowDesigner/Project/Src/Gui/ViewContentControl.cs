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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
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
			
			this.loader = loader;

			try {
				
				LoadDesigner();
				
				if (designSurface != null && Controls.Count == 0) {
					Control designer = designSurface.View as Control;
					designer.Dock = DockStyle.Fill;
					Controls.Add(designer);
					DesignerHost.Activate();
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
