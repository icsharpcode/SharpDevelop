// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Web.Services.Description;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class WebServicesView : System.Windows.Forms.UserControl
	{
		const int ServiceDescriptionImageIndex = 0;
		const int ServiceImageIndex = 1;
		const int PortImageIndex = 2;
		const int OperationImageIndex = 3;
		
		public WebServicesView()
		{
			InitializeComponent();
			AddImages();
			AddStringResources();
		}
		
		/// <summary>
		/// Removes all web services currently on display.
		/// </summary>
		public void Clear()
		{
			webServicesListView.Items.Clear();
			webServicesTreeView.Nodes.Clear();
		}
		
		public void Add(ServiceDescriptionCollection serviceDescriptions)
		{
			if (serviceDescriptions.Count == 0) {
				return;
			}

			webServicesListView.BeginUpdate();
			try {
				foreach (ServiceDescription description in serviceDescriptions) {
					Add(description);
				}
			} finally {
				webServicesListView.EndUpdate();
			}
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.webServicesTreeView = new System.Windows.Forms.TreeView();
			this.webServicesListView = new System.Windows.Forms.ListView();
			this.propertyColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.valueColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.webServicesTreeView);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.webServicesListView);
			this.splitContainer.Size = new System.Drawing.Size(471, 305);
			this.splitContainer.SplitterDistance = 156;
			this.splitContainer.TabIndex = 1;
			// 
			// webServicesTreeView
			// 
			this.webServicesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webServicesTreeView.Location = new System.Drawing.Point(0, 0);
			this.webServicesTreeView.Name = "webServicesTreeView";
			this.webServicesTreeView.Size = new System.Drawing.Size(156, 305);
			this.webServicesTreeView.TabIndex = 0;
			this.webServicesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.WebServicesTreeViewAfterSelect);
			// 
			// webServicesListView
			// 
			this.webServicesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			                                          	this.propertyColumnHeader,
			                                          	this.valueColumnHeader});
			this.webServicesListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webServicesListView.Location = new System.Drawing.Point(0, 0);
			this.webServicesListView.Name = "webServicesListView";
			this.webServicesListView.Size = new System.Drawing.Size(311, 305);
			this.webServicesListView.TabIndex = 2;
			this.webServicesListView.UseCompatibleStateImageBehavior = false;
			this.webServicesListView.View = System.Windows.Forms.View.Details;
			// 
			// propertyColumnHeader
			// 
			this.propertyColumnHeader.Text = "Property";
			this.propertyColumnHeader.Width = 120;
			// 
			// valueColumnHeader
			// 
			this.valueColumnHeader.Text = "Value";
			this.valueColumnHeader.Width = 191;
			// 
			// WebServicesView
			// 
			this.Controls.Add(this.splitContainer);
			this.Name = "WebServicesView";
			this.Size = new System.Drawing.Size(471, 305);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ColumnHeader propertyColumnHeader;
		private System.Windows.Forms.ColumnHeader valueColumnHeader;
		private System.Windows.Forms.TreeView webServicesTreeView;
		private System.Windows.Forms.ListView webServicesListView;
		private System.Windows.Forms.SplitContainer splitContainer;
		
		#endregion
		
		void WebServicesTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			ListViewItem item;
			webServicesListView.Items.Clear();
			
			if (e.Node.Tag is ServiceDescription) {
				ServiceDescription desc = (ServiceDescription)e.Node.Tag;
				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.RetrievalUriProperty}");
				item.SubItems.Add(desc.RetrievalUrl);
				webServicesListView.Items.Add(item);
			} else if(e.Node.Tag is Service) {
				Service service = (Service)e.Node.Tag;
				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.DocumentationProperty}");
				item.SubItems.Add(service.Documentation);
				webServicesListView.Items.Add(item);
			} else if(e.Node.Tag is Port) {
				Port port = (Port)e.Node.Tag;

				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.DocumentationProperty}");
				item.SubItems.Add(port.Documentation);
				webServicesListView.Items.Add(item);
				
				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.BindingProperty}");
				item.SubItems.Add(port.Binding.Name);
				webServicesListView.Items.Add(item);
				
				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ServiceNameProperty}");
				item.SubItems.Add(port.Service.Name);
				webServicesListView.Items.Add(item);
			} else if(e.Node.Tag is Operation) {
				Operation operation = (Operation)e.Node.Tag;
				
				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.DocumentationProperty}");
				item.SubItems.Add(operation.Documentation);
				webServicesListView.Items.Add(item);

				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ParametersProperty}");
				item.SubItems.Add(operation.ParameterOrderString);
				webServicesListView.Items.Add(item);
			}
		}
		
		void Add(ServiceDescription description)
		{
			TreeNode rootNode = new TreeNode(GetName(description));
			rootNode.Tag = description;
			rootNode.ImageIndex = ServiceDescriptionImageIndex;
			rootNode.SelectedImageIndex = ServiceDescriptionImageIndex;
			webServicesTreeView.Nodes.Add(rootNode);

			foreach(Service service in description.Services) {
				// Add a Service node
				TreeNode serviceNode = new TreeNode(service.Name);
				serviceNode.Tag = service;
				serviceNode.ImageIndex = ServiceImageIndex;
				serviceNode.SelectedImageIndex = ServiceImageIndex;
				rootNode.Nodes.Add(serviceNode);
				
				foreach(Port port in service.Ports) {
					TreeNode portNode = new TreeNode(port.Name);
					portNode.Tag = port;
					portNode.ImageIndex = PortImageIndex;
					portNode.SelectedImageIndex = PortImageIndex;
					serviceNode.Nodes.Add(portNode);
					
					// Get the operations
					System.Web.Services.Description.Binding binding = description.Bindings[port.Binding.Name];
					if (binding != null) {
						PortType portType = description.PortTypes[binding.Type.Name];
						if (portType != null) {
							foreach(Operation operation in portType.Operations) {
								TreeNode operationNode = new TreeNode(operation.Name);
								operationNode.Tag = operation;
								operationNode.ImageIndex = OperationImageIndex;
								operationNode.SelectedImageIndex = OperationImageIndex;
								portNode.Nodes.Add(operationNode);
							}
						}
					}
				}
			}
			webServicesTreeView.ExpandAll();
		}
		
		string GetName(ServiceDescription description)
		{
			if (description.Name != null) {
				return description.Name;
			} else if (description.RetrievalUrl != null) {
				Uri uri = new Uri(description.RetrievalUrl);
				if (uri.Segments.Length > 0) {
					return uri.Segments[uri.Segments.Length - 1];
				} else {
					return uri.Host;
				}
			}
			return String.Empty;
		}
		
		void AddImages()
		{
			try {
				ImageList imageList = new ImageList();
				imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Library"));
				imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Interface"));
				imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Class"));
				imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Method"));
				
				webServicesTreeView.ImageList = imageList;
			} catch (ResourceNotFoundException) {
				// in design mode, the core is not initialized -> the resources cannot be found
			}
		}
		
		void AddStringResources()
		{
			valueColumnHeader.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ValueColumnHeader}");
			propertyColumnHeader.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.PropertyColumnHeader}");
		}
	}
}
