// <file>
//     <copyright see="prj:///Doc/copyright.txt"/>
//     <license see="prj:///Doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

using Hornung.ResourceToolkit.ResourceFileContent;

namespace Hornung.ResourceToolkit.Gui
{
	/// <summary>
	/// Displays unused resource keys in a list and allows the user to delete them.
	/// </summary>
	public class UnusedResourceKeysViewContent : AbstractViewContent, IClipboardHandler
	{
		readonly ICollection<KeyValuePair<string, string>> unusedKeys;
		Panel panel;
		ListView listView;
		
		public override System.Windows.Forms.Control Control {
			get {
				return this.panel;
			}
		}
		
		/// <summary>
		/// Gets the ListView control that shows the unused resource keys.
		/// </summary>
		public System.Windows.Forms.ListView ListView {
			get {
				return this.listView;
			}
		}
		
		/// <summary>
		/// Gets a collection of key/value pairs where the values are the resource file names and the keys are the unused resource keys.
		/// </summary>
		public ICollection<KeyValuePair<string, string>> UnusedKeys {
			get {
				return this.unusedKeys;
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="UnusedResourceKeysViewContent"/> class.
		/// </summary>
		/// <param name="unusedKeys">A collection of key/value pairs where the values are the resource file names and the keys are the unused resource keys.</param>
		public UnusedResourceKeysViewContent(ICollection<KeyValuePair<string, string>> unusedKeys)
			: base(StringParser.Parse("${res:Hornung.ResourceToolkit.UnusedResourceKeys.Title}"))
		{
			LoggingService.Debug("ResourceToolkit: Creating new UnusedResourceKeysViewContent");
			
			if (unusedKeys == null) {
				throw new ArgumentNullException("unusedKeys");
			}
			this.unusedKeys = unusedKeys;
			
			this.panel = new Panel();
			this.panel.SuspendLayout();
			this.panel.Dock = DockStyle.Fill;
			
			this.listView = new ListView();
			this.ListView.Columns.Add(StringParser.Parse("${res:Hornung.ResourceToolkit.FileName}"), 60);
			this.ListView.Columns.Add(StringParser.Parse("${res:Hornung.ResourceToolkit.Key}"), 140);
			this.ListView.Columns.Add(StringParser.Parse("${res:Hornung.ResourceToolkit.Value}"), 140);
			this.ListView.CheckBoxes = true;
			this.ListView.View = View.Details;
			this.ListView.FullRowSelect = true;
			this.ListView.ShowItemToolTips = true;
			this.ListView.ListViewItemSorter = new ResourceListViewItemComparer();
			this.ListView.Dock = DockStyle.Fill;
			this.ListView.Resize += delegate {
				if (this.ListView != null && !this.ListView.IsDisposed && !this.ListView.Disposing && this.ListView.Columns.Count >= 3) {
					this.ListView.Columns[0].Width = Convert.ToInt32(this.ListView.Width * 0.20);
					this.ListView.Columns[1].Width = Convert.ToInt32(this.ListView.Width * 0.45);
					this.ListView.Columns[2].Width = Convert.ToInt32(this.ListView.Width * 0.30);
				}
			};
			this.ListView.HandleCreated += this.ListViewHandleCreated;
			this.ListView.ContextMenuStrip = MenuService.CreateContextMenu(this, "/AddIns/ResourceToolkit/ViewContent/UnusedResourceKeys/ListViewContextMenu");
			
			ToolStrip toolStrip = ToolbarService.CreateToolStrip(this, "/AddIns/ResourceToolkit/ViewContent/UnusedResourceKeys/Toolbar");
			toolStrip.Dock = DockStyle.Top;
			toolStrip.Stretch = true;
			
			this.panel.Controls.Add(this.ListView);
			this.panel.Controls.Add(toolStrip);
			this.panel.ResumeLayout();
		}
		
		void ListViewHandleCreated(object sender, EventArgs e)
		{
			this.ListView.HandleCreated -= this.ListViewHandleCreated;
			this.ListView.BeginInvoke(new Action<ICollection<KeyValuePair<string, string>>>(this.FillListView), this.UnusedKeys);
		}
		
		public override void Dispose()
		{
			this.panel.Controls.Clear();
			this.ListView.Dispose();
			this.listView = null;
			this.panel.Dispose();
			this.panel = null;
			base.Dispose();
		}
		
		// ********************************************************************************************************************************
		
		class ResourceListViewItemComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				ListViewItem a = (ListViewItem)x;
				ListViewItem b = (ListViewItem)y;
				return String.Compare(String.Concat(a.Text, a.SubItems[1].Text), String.Concat(b.Text, b.SubItems[1].Text));
			}
		}
		
		/// <summary>
		/// Fills the list view with the specified resource keys.
		/// </summary>
		/// <param name="resources">A collection of key/value pairs where the values are the resource file names and the keys are the resource keys.</param>
		public void FillListView(ICollection<KeyValuePair<string, string>> resources)
		{
			Application.DoEvents();
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try {
				
				this.ListView.Items.Clear();
				this.ListView.Groups.Clear();
				this.ListView.BeginUpdate();
				
				Dictionary<string, ListViewGroup> fileGroups = new Dictionary<string, ListViewGroup>();
				
				// Create the ListViewItems.
				foreach (KeyValuePair<string, string> entry in resources) {
					IResourceFileContent c = ResourceFileContentRegistry.GetResourceFileContent(entry.Value);
					object o;
					
					// only add the file name to save space
					// and show the full path as tooltip
					ListViewItem item = new ListViewItem(Path.GetFileName(entry.Value));
					item.ToolTipText = entry.Value;
					item.SubItems.Add(entry.Key);
					if (c.TryGetValue(entry.Key, out o)) {
						item.SubItems.Add((o ?? (object)"<<null>>").ToString());
					} else {
						throw new InvalidOperationException("The key '"+entry.Key+"' in file '"+entry.Value+"' does not exist although it was reported as unused.");
					}
					
					// Use ListViewGroups to group by file names
					ListViewGroup grp;
					if (!fileGroups.TryGetValue(entry.Value, out grp)) {
						grp = new ListViewGroup(entry.Value);
						fileGroups.Add(entry.Value, grp);
						this.ListView.Groups.Add(grp);
					}
					grp.Items.Add(item);
					
					this.ListView.Items.Add(item);
				}
				
				this.ListView.EndUpdate();
				
			} finally {
				Cursor.Current = oldCursor;
			}
		}
		
		// ********************************************************************************************************************************
		
		#region IClipboardHandler implementation
		
		public bool EnableCut {
			get {
				return false;
			}
		}
		
		public bool EnableCopy {
			get {
				return false;
			}
		}
		
		public bool EnablePaste {
			get {
				return false;
			}
		}
		
		public bool EnableDelete {
			get {
				return this.ListView.SelectedItems.Count > 0;
			}
		}
		
		public bool EnableSelectAll {
			get {
				return this.ListView.Items.Count > 0;
			}
		}
		
		public void Cut()
		{
			throw new NotImplementedException();
		}
		
		public void Copy()
		{
			throw new NotImplementedException();
		}
		
		public void Paste()
		{
			throw new NotImplementedException();
		}
		
		public void Delete()
		{
			if (this.ListView.SelectedItems.Count > 0) {
				
				bool ok;
				
				if (this.ListView.SelectedItems.Count == 1) {
					ok = MessageService.AskQuestion(StringParser.Parse("${res:Hornung.ResourceToolkit.DeleteSingleResourceKeyQuestion}", new string[,] { {"Key", this.ListView.SelectedItems[0].SubItems[1].Text}, {"FileName", this.ListView.SelectedItems[0].Group.Header} }));
				} else {
					ok = MessageService.AskQuestion(StringParser.Parse("${res:Hornung.ResourceToolkit.DeleteAllSelectedResourceKeysQuestion}", new string[,] { {"Count", this.ListView.SelectedItems.Count.ToString(CultureInfo.CurrentCulture)} }));
				}
				
				if (ok) {
					this.DeleteResources(this.ListView.SelectedItems);
				}
				
			}
		}
		
		public void SelectAll()
		{
			foreach (ListViewItem item in this.ListView.Items) {
				item.Selected = true;
			}
		}
		
		#endregion
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Deletes the resource keys represented by the specified ListViewItems.
		/// </summary>
		public void DeleteResources(System.Collections.IEnumerable itemsToDelete)
		{
			Application.DoEvents();
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try {
				
				// The collection must not be modified during the enumeration.
				// -> Save the items that should be deleted in a separate list.
				List<ListViewItem> items = new List<ListViewItem>();
				foreach (ListViewItem item in itemsToDelete) {
					DeleteResourceKey(item.Group.Header, item.SubItems[1].Text);
					items.Add(item);
				}
				
				items.ForEach(this.ListView.Items.Remove);
				
			} finally {
				Cursor.Current = oldCursor;
			}
		}
		
		/// <summary>
		/// Deletes the specified resource key in the resource file and all dependent localized
		/// resource files permanently.
		/// </summary>
		/// <param name="fileName">The master resource file that contains the key to be deleted.</param>
		/// <param name="key">The key to be deleted.</param>
		protected static void DeleteResourceKey(string fileName, string key)
		{
			IResourceFileContent content = ResourceFileContentRegistry.GetResourceFileContent(fileName);
			if (content != null) {
				if (content.ContainsKey(key)) {
					LoggingService.Debug("ResourceToolkit: Remove key '"+key+"' from resource file '"+fileName+"'");
					content.RemoveKey(key);
				} else {
					MessageService.ShowWarningFormatted("${res:Hornung.ResourceToolkit.KeyNotFoundWarning}", key, fileName);
				}
			} else {
				MessageService.ShowWarning("ResoureToolkit: Could not get ResourceFileContent for '"+fileName+"' key +'"+key+"'.");
			}
			
			foreach (KeyValuePair<string, IResourceFileContent> entry in ResourceFileContentRegistry.GetLocalizedContents(fileName)) {
				LoggingService.Debug("ResourceToolkit: Looking in localized resource file: '"+entry.Value.FileName+"'");
				if (entry.Value.ContainsKey(key)) {
					LoggingService.Debug("ResourceToolkit:   -> Key found, removing.");
					entry.Value.RemoveKey(key);
				}
			}
		}
		
	}
}
