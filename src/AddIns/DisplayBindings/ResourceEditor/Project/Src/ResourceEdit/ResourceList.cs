// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
		ColumnHeader comment  = new ColumnHeader();
		
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
			
			comment.Text = ResourceService.GetString("ResourceEditor.ResourceEdit.CommentColumn");
			comment.Width = 300;
			
			Columns.AddRange(new ColumnHeader[] {name, type, content, comment});
			
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
			                                	null,
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
		
		public void LoadFile(FileName filename, Stream stream)
		{
			resources.Clear();
			metadata.Clear();
			switch (Path.GetExtension(filename).ToLowerInvariant()) {
				case ".resx":
					ResXResourceReader rx = new ResXResourceReader(stream);
					ITypeResolutionService typeResolver = null;
					rx.BasePath = Path.GetDirectoryName(filename);
					rx.UseResXDataNodes = true;
					IDictionaryEnumerator n = rx.GetEnumerator();
					while (n.MoveNext()) {
						if (!resources.ContainsKey(n.Key.ToString())) {
							ResXDataNode node = (ResXDataNode)n.Value;
							resources.Add(n.Key.ToString(), new ResourceItem(node.Name, node.GetValue(typeResolver), node.Comment));
						}
					}
					
					n = rx.GetMetadataEnumerator();
					while (n.MoveNext()) {
						if (!metadata.ContainsKey(n.Key.ToString())) {
							ResXDataNode node = (ResXDataNode)n.Value;
							metadata.Add(n.Key.ToString(), new ResourceItem(node.Name, node.GetValue(typeResolver)));
						}
					}
					
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
		
		public void SaveFile(FileName filename, Stream stream)
		{
			switch (Path.GetExtension(filename).ToLowerInvariant()) {
				case ".resx":
					// write XML resource
					ResXResourceWriter rxw = new ResXResourceWriter(stream, t => ResXConverter.ConvertTypeName(t, filename));
					foreach (KeyValuePair<string, ResourceItem> entry in resources) {
						if (entry.Value != null) {
							ResourceItem item = entry.Value;
							rxw.AddResource(item.ToResXDataNode(t => ResXConverter.ConvertTypeName(t, filename)));
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
				default:
					// write default resource
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
		
		public void SetCommentValue(string resourceName, string commentValue)
		{
			ResourceItem item = ((ResourceItem)Resources[resourceName]);
			item.Comment = commentValue;
			SelectedItems[0].SubItems[3].Text = item.Comment;
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
				
				ListViewItem lv = new ListViewItem(new String[] {item.Name, type, tmp, item.Comment}, item.ImageIndex);
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
