// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ResourceEditor
{
	public class ResourceEditorControl : UserControl, IOwnerState
	{
		ResourceList resourceList;
		Splitter splitter;
		Panel panel;
		IResourceView currentView = null;
		
		[Flags]
		public enum ListViewViewState {
			Nothing       = 0,
			ItemsSelected = 1,
		}
		
		protected ListViewViewState internalState = ListViewViewState.Nothing;

		public System.Enum InternalState {
			get {
				return internalState;
			}
		}
		
		public ResourceList ResourceList
		{
			get {
				return resourceList;
			}
		}
		
		public ResourceEditorControl()
		{
			InitializeComponent();
			resourceList.SelectedIndexChanged += new EventHandler(resourceListSelectionChanged);
		}
		
		void resourceListSelectionChanged(object sender, EventArgs e)
		{
			if(resourceList.SelectedItems.Count == 0) {
				internalState = ListViewViewState.Nothing;
				showResource(null);
			} else {
				internalState = ListViewViewState.ItemsSelected;
			}
			
			if(resourceList.SelectedItems.Count != 1) {
				return;
			}
			object key = resourceList.SelectedItems[0].Text;
			ResourceItem item = (ResourceItem)resourceList.Resources[key.ToString()];
			showResource(item);
		}
		
		void InitializeComponent()
		{
			resourceList = new ResourceList(this);
			resourceList.Dock = DockStyle.Top;
			Controls.Add(resourceList);
			
			panel = new Panel();
			panel.BackColor = SystemColors.Info;
			panel.Dock = DockStyle.Fill;
			
			splitter = new Splitter();
			splitter.Dock = DockStyle.Top;
			
			Controls.Add(panel);
			Controls.Add(splitter);
			Controls.Add(resourceList);
			
			this.Resize += new EventHandler(initializeLayout);
		}
		
		void initializeLayout(object sender, EventArgs e)
		{
			resourceList.Height = Convert.ToInt32(0.75 * Height);
		}
		
		void showView(Control viewer)
		{
			// remvoe old view if there is one
			if(panel.Controls.Count == 1) {
				Control control = panel.Controls[0];
				panel.Controls.Remove(control);
				control.Dispose();
			}
			
			if(viewer != null) {
				viewer.Dock = DockStyle.Fill;
				panel.Controls.Add(viewer);
				currentView = (IResourceView)viewer;
				currentView.WriteProtected = resourceList.WriteProtected;
				currentView.ResourceChanged += new ResourceChangedEventHandler(viewResourceChanged);
			}
		}
		
		void viewResourceChanged(object sender, ResourceEventArgs e)
		{
			resourceList.SetResourceValue(e.ResourceName, e.ResourceValue);
			
			
			
		}
		
		void showResource(ResourceItem item)
		{
			if(item == null) {
				showView(null);
				return;
			}
			if (item.ResourceValue is Icon) {
				IconView iv = new IconView(item);
				showView(iv);
			} else if(item.ResourceValue is Bitmap) {
				BitmapView bv = new BitmapView(item);
				showView(bv);
			} else if(item.ResourceValue is Cursor) {
				CursorView cv = new CursorView(item);
				showView(cv);
			} else if(item.ResourceValue is string) {
				TextView tv = new TextView(item);
				showView(tv);
			} else if(item.ResourceValue is byte[]) {
				BinaryView bv = new BinaryView(item);
				showView(bv);
			} else if(item.ResourceValue is bool) {
				BooleanView bv = new BooleanView(item);
				showView(bv);
			} else {
				showView(null);
			}
		}

	}
}
