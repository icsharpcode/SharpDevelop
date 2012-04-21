// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets.ListViewSorting;

namespace ResourceEditor
{
	/// <summary>
	/// This class allows viewing and editing of windows resource files
	/// both in XML as in normal format.
	/// </summary>
	public class ResourceList : ListView
	{
		ColumnHeader name     = new ColumnHeader();
		ColumnHeader type     = new ColumnHeader();
		ColumnHeader content  = new ColumnHeader();
		
		Dictionary<string, ResourceItem> resources = new Dictionary<string, ResourceItem>();
		Dictionary<string, ResourceItem> metadata = new Dictionary<string, ResourceItem>();
		ImageList images = new ImageList();
		
		bool writeProtected = false;
		int editListViewItemIndex = -1;
		
		ListViewItemSorter sorter;
		
		public event EventHandler Changed;
		
		public bool WriteProtected
		{
			get {
				return writeProtected;
			}
			set {
				writeProtected = value;
			}
		}
		
		public Dictionary<string, ResourceItem> Resources
		{
			get {
				return resources;
			}
		}
		
		public PrintDocument PrintDocument
		{
			get {
				return null;
			}
		}
		
		public bool IsEditing {
			get {
				return editListViewItemIndex != -1;
			}
		}
		
		public ResourceList(ResourceEditorControl editor)
		{
			name.Text     = ResourceService.GetString("Global.Name");
			name.Width    = 250;
			
			type.Text     = ResourceService.GetString("ResourceEditor.ResourceEdit.TypeColumn");
			type.Width    = 170;
			
			content.Text  = ResourceService.GetString("ResourceEditor.ResourceEdit.ContentColumn");
			content.Width = 300;
			
			Columns.AddRange(new ColumnHeader[] {name, type, content});
			
			FullRowSelect = true;
			AutoArrange   = true;
			Alignment     = ListViewAlignment.Left;
			View          = View.Details;
			GridLines     = true;
			LabelEdit     = true;
			Dock          = DockStyle.Fill;
			HideSelection = false;
			
			BorderStyle   = System.Windows.Forms.BorderStyle.None;
			
			images.Images.Add(WinFormsResourceService.GetIcon("Icons.16x16.ResourceEditor.string"));
			images.Images.Add(WinFormsResourceService.GetIcon("Icons.16x16.ResourceEditor.bmp"));
			images.Images.Add(WinFormsResourceService.GetIcon("Icons.16x16.ResourceEditor.icon"));
			images.Images.Add(WinFormsResourceService.GetIcon("Icons.16x16.ResourceEditor.cursor"));
			images.Images.Add(WinFormsResourceService.GetIcon("Icons.16x16.ResourceEditor.bin"));
			images.Images.Add(WinFormsResourceService.GetIcon("Icons.16x16.ResourceEditor.obj"));
			SmallImageList = images;
			
			// Set up sorting:
			// User can sort the list by name and by type,
			// whereas sorting by type also implicitly sorts by name.
			IListViewItemComparer textComparer = new ListViewTextColumnComparer();
			IListViewItemComparer typeNameComparer = new ListViewMultipleColumnsComparer(textComparer, 1, textComparer, 0);
			sorter = new ListViewItemSorter(this,
			                                new IListViewItemComparer[] {
			                                	textComparer,
			                                	typeNameComparer,
			                                	null
			                                });
			sorter.SortColumnIndex = 0;
			sorter.SortOrder = SortOrder.Ascending;
			
			ContextMenuStrip = MenuService.CreateContextMenu(editor, "/SharpDevelop/ResourceEditor/ResourceList/ContextMenu");
		}
		
		protected override void Dispose(bool disposing)
		{
			try {
				if (disposing) {
					if (sorter != null) {
						sorter.Dispose();
						sorter = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		
		public void LoadFile(string filename, Stream stream)
		{
			resources.Clear();
			metadata.Clear();
			switch (Path.GetExtension(filename).ToLowerInvariant()) {
				case ".resx":
					ResXResourceReader rx = new ResXResourceReader(stream);
					rx.BasePath = Path.GetDirectoryName(filename);
					IDictionaryEnumerator n = rx.GetEnumerator();
					while (n.MoveNext())
						if (!resources.ContainsKey(n.Key.ToString()))
						resources.Add(n.Key.ToString(), new ResourceItem(n.Key.ToString(), n.Value));
					
					n = rx.GetMetadataEnumerator();
					while (n.MoveNext())
						if (!metadata.ContainsKey(n.Key.ToString()))
						metadata.Add(n.Key.ToString(), new ResourceItem(n.Key.ToString(), n.Value));
					
					rx.Close();
					break;
				case ".resources":
					ResourceReader rr=null;
					try {
						rr = new ResourceReader(stream);
						foreach (DictionaryEntry entry in rr) {
							if (!resources.ContainsKey(entry.Key.ToString()))
								resources.Add(entry.Key.ToString(), new ResourceItem(entry.Key.ToString(), entry.Value));
						}
					}
					finally {
						if (rr != null) {
							rr.Close();
						}
					}
					break;
			}
			InitializeListView();
		}
		
		public void SaveFile(string filename, Stream stream)
		{
			switch (Path.GetExtension(filename).ToLowerInvariant()) {
					
					// write XML resource
				case ".resx":
					ResXResourceWriter rxw = new ResXResourceWriter(stream, t => ResXConverter.ConvertTypeName(t, filename));
					foreach (KeyValuePair<string, ResourceItem> entry in resources) {
						if (entry.Value != null) {
							ResourceItem item = entry.Value;
							rxw.AddResource(item.Name, item.ResourceValue);
						}
					}
					foreach (KeyValuePair<string, ResourceItem> entry in metadata) {
						if (entry.Value != null) {
							ResourceItem item = entry.Value;
							rxw.AddMetadata(item.Name, item.ResourceValue);
						}
					}
					rxw.Generate();
					rxw.Close();
					break;
					
					// write default resource
				default:
					ResourceWriter rw = new ResourceWriter(stream);
					foreach (KeyValuePair<string, ResourceItem> entry in resources) {
						ResourceItem item = (ResourceItem)entry.Value;
						rw.AddResource(item.Name, item.ResourceValue);
					}
					rw.Generate();
					rw.Close();
					break;
			}
		}

		public void SetResourceValue(string resourceName, object resourceValue)
		{
			ResourceItem item = ((ResourceItem)Resources[resourceName]);
			item.ResourceValue = resourceValue;
			SelectedItems[0].SubItems[2].Text = item.ToString();
			OnChanged();
		}
		
		public void OnChanged()
		{
			if (Changed != null) {
				Changed(this, null);
			}
		}
		
		public void InitializeListView()
		{
			BeginUpdate();
			// Suspend sorting to improve performance
			ListViewItemSorter = null;
			Items.Clear();
			
			foreach (KeyValuePair<string, ResourceItem> entry in resources) {
				ResourceItem item = entry.Value;
				
				string tmp  = item.ToString();
				string type = item.ResourceValue == null ? "(Nothing/null)" : item.ResourceValue.GetType().FullName;
				
				ListViewItem lv = new ListViewItem(new String[] {item.Name, type, tmp}, item.ImageIndex);
				Items.Add(lv);
			}
			
			ListViewItemSorter = sorter;
			EndUpdate();
		}
		
		protected override void OnAfterLabelEdit(LabelEditEventArgs e)
		{
			editListViewItemIndex = -1;
			
			if (writeProtected) {
				e.CancelEdit = true;
				return;
			}
			string oldName = this.Items[e.Item].Text;
			string newName = e.Label;
			
			if(newName == null) {
				// no change
				return;
			}
			
			ResourceItem item = (ResourceItem)resources[oldName];
			
			if(resources.ContainsKey(newName)) {
				
				MessageService.ShowWarning("${res:ResourceEditor.ResourceList.KeyAlreadyDefinedWarning}");
				e.CancelEdit = true;
				return;
			}
			
			resources.Remove(oldName);
			item.Name = newName;
			resources.Add(newName, item);
			OnChanged();
		}
		
		protected override void OnBeforeLabelEdit(LabelEditEventArgs e)
		{
			base.OnBeforeLabelEdit(e);
			if (!e.CancelEdit) {
				editListViewItemIndex = e.Item;
			}
		}
	}
}
