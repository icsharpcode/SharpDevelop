// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace Hornung.ResourceToolkit.Gui
{
	/// <summary>
	/// Displays unused resource keys in a list and allows the user to delete them.
	/// </summary>
	public class UnusedResourceKeysViewContent : AbstractViewContent, IClipboardHandler, IFilterHost<ResourceItem>
	{
		readonly ICollection<ResourceItem> unusedKeys;
		Panel panel;
		ListView listView;
		ToolStrip toolStrip;
		
		public override object Control {
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
		public ICollection<ResourceItem> UnusedKeys {
			get {
				return this.unusedKeys;
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="UnusedResourceKeysViewContent"/> class.
		/// </summary>
		/// <param name="unusedKeys">A collection of <see cref="ResourceItem"/> classes that represent the unused resource keys to display.</param>
		public UnusedResourceKeysViewContent(ICollection<ResourceItem> unusedKeys)
		{
			LoggingService.Debug("ResourceToolkit: Creating new UnusedResourceKeysViewContent");
			
			SetLocalizedTitle("${res:Hornung.ResourceToolkit.UnusedResourceKeys.Title}");
			
			if (unusedKeys == null) {
				throw new ArgumentNullException("unusedKeys");
			}
			this.unusedKeys = unusedKeys;
			
			this.panel = new Panel();
			this.panel.SuspendLayout();
			this.panel.Dock = DockStyle.Fill;
			
			this.listView = new ListView();
			this.ListView.Columns.Add(StringParser.Parse("${res:Global.FileName}"), 60);
			this.ListView.Columns.Add(StringParser.Parse("${res:Hornung.ResourceToolkit.Key}"), 140);
			this.ListView.Columns.Add(StringParser.Parse("${res:Hornung.ResourceToolkit.Value}"), 140);
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
			
			this.toolStrip = ToolbarService.CreateToolStrip(this, "/AddIns/ResourceToolkit/ViewContent/UnusedResourceKeys/Toolbar");
			this.toolStrip.Dock = DockStyle.Top;
			this.toolStrip.Stretch = true;
			this.toolStrip.VisibleChanged += this.ToolStripVisibleChanged;
			
			this.panel.Controls.Add(this.ListView);
			this.panel.Controls.Add(toolStrip);
			this.panel.ResumeLayout();
		}
		
		void ListViewHandleCreated(object sender, EventArgs e)
		{
			this.ListView.HandleCreated -= this.ListViewHandleCreated;
			this.FillListView();
		}
		
		void ToolStripVisibleChanged(object sender, EventArgs e)
		{
			if (this.toolStrip.Visible) {
				this.toolStrip.VisibleChanged -= this.ToolStripVisibleChanged;
				ToolbarService.UpdateToolbar(this.toolStrip);
			}
		}
		
		public override void Dispose()
		{
			this.panel.Controls.Clear();
			if (this.toolStrip != null) {
				this.toolStrip.Dispose();
				this.toolStrip = null;
			}
			if (this.listView != null) {
				this.listView.Dispose();
				this.listView = null;
			}
			if (this.panel != null) {
				this.panel.Dispose();
				this.panel = null;
			}
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
		
		bool fillListViewQueued;
		
		/// <summary>
		/// Fills the list view with all unused resource keys that match the current filter after processing the message queue.
		/// </summary>
		public void FillListView()
		{
			if (!this.fillListViewQueued) {
				this.fillListViewQueued = true;
				this.ListView.BeginInvoke(new MethodInvoker(this.FillListViewInternal));
			}
		}
		
		/// <summary>
		/// Fills the list view with all unused resource keys that match the current filter.
		/// </summary>
		void FillListViewInternal()
		{
			Application.DoEvents();
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			
			try {
				
				this.ListView.Items.Clear();
				this.ListView.Groups.Clear();
				// Suspend sorting to improve performance
				System.Collections.IComparer comparer = this.ListView.ListViewItemSorter;
				this.ListView.ListViewItemSorter = null;
				this.ListView.BeginUpdate();
				
				Dictionary<string, ListViewGroup> fileGroups = new Dictionary<string, ListViewGroup>();
				
				// Create the ListViewItems.
				foreach (ResourceItem entry in this.UnusedKeys) {
					
					// Skip if any filter rejects this item.
					if (!this.ItemMatchesCurrentFilter(entry)) {
						continue;
					}
					
					IResourceFileContent c = ResourceFileContentRegistry.GetResourceFileContent(entry.FileName);
					object o;
					
					// only add the file name to save space
					// and show the full path as tooltip
					ListViewItem item = new ListViewItem(Path.GetFileName(entry.FileName));
					item.ToolTipText = entry.FileName;
					item.SubItems.Add(entry.Key);
					if (c.TryGetValue(entry.Key, out o)) {
						item.SubItems.Add((o ?? (object)"<<null>>").ToString());
					} else {
						throw new InvalidOperationException("The key '"+entry.Key+"' in file '"+entry.FileName+"' does not exist although it was reported as unused.");
					}
					
					// Use ListViewGroups to group by file names
					ListViewGroup grp;
					if (!fileGroups.TryGetValue(entry.FileName, out grp)) {
						grp = new ListViewGroup(entry.FileName);
						fileGroups.Add(entry.FileName, grp);
						this.ListView.Groups.Add(grp);
					}
					grp.Items.Add(item);
					
					this.ListView.Items.Add(item);
				}
				
				this.ListView.ListViewItemSorter = comparer;
				this.ListView.EndUpdate();
				
			} finally {
				this.fillListViewQueued = false;
				Cursor.Current = oldCursor;
			}
		}
		
		#region Filter
		
		readonly List<IFilter<ResourceItem>> filters = new List<IFilter<ResourceItem>>();
		
		/// <summary>
		/// Registers a new filter with the filter host, if the filter is not already registered,
		/// or signals that the filter condition of the specified filter has changed.
		/// </summary>
		/// <param name="filter">The filter to be registered.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="filter"/> parameter is <c>null</c>.</exception>
		public void RegisterFilter(IFilter<ResourceItem> filter)
		{
			if (filter == null) {
				throw new ArgumentNullException("filter");
			}
			
			if (!this.filters.Contains(filter)) {
				this.filters.Add(filter);
			}
			
			this.FillListView();
		}
		
		/// <summary>
		/// Removes the specified filter from the filter host, if it is currently registered there.
		/// </summary>
		/// <param name="filter">The filter to be removed.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="filter"/> parameter is <c>null</c>.</exception>
		public void UnregisterFilter(IFilter<ResourceItem> filter)
		{
			if (filter == null) {
				throw new ArgumentNullException("filter");
			}
			
			this.filters.Remove(filter);
			
			this.FillListView();
		}
		
		/// <summary>
		/// Determines whether the specified resource should be included in the list
		/// according to the current filter.
		/// </summary>
		/// <returns><c>true</c>, if the resource should be included in the list view, otherwise <c>false</c>.</returns>
		bool ItemMatchesCurrentFilter(ResourceItem item)
		{
			foreach (IFilter<ResourceItem> filter in this.filters) {
				if (!filter.IsMatch(item)) {
					return false;
				}
			}
			return true;
		}
		
		#endregion
		
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
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body")]
		public void Delete()
		{
			if (this.ListView.SelectedItems.Count > 0) {
				
				bool ok;
				
				if (this.ListView.SelectedItems.Count == 1) {
					ok = MessageService.AskQuestion(
						StringParser.Parse("${res:Hornung.ResourceToolkit.DeleteSingleResourceKeyQuestion}",
						                   new StringTagPair("Key", this.ListView.SelectedItems[0].SubItems[1].Text), 
						                   new StringTagPair("FileName", this.ListView.SelectedItems[0].Group.Header))
					);
				} else {
					ok = MessageService.AskQuestion(StringParser.Parse("${res:Hornung.ResourceToolkit.DeleteAllSelectedResourceKeysQuestion}", new StringTagPair("Count", this.ListView.SelectedItems.Count.ToString(CultureInfo.CurrentCulture))));
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
			try {
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
			} catch (Exception ex) {
				MessageService.ShowWarningFormatted("${res:Hornung.ResourceToolkit.ErrorProcessingResourceFile}" + Environment.NewLine + ex.Message, fileName);
				return;
			}
			
			foreach (KeyValuePair<string, IResourceFileContent> entry in ResourceFileContentRegistry.GetLocalizedContents(fileName)) {
				LoggingService.Debug("ResourceToolkit: Looking in localized resource file: '"+entry.Value.FileName+"'");
				try {
					if (entry.Value.ContainsKey(key)) {
						LoggingService.Debug("ResourceToolkit:   -> Key found, removing.");
						entry.Value.RemoveKey(key);
					}
				} catch (Exception ex) {
					MessageService.ShowWarningFormatted("${res:Hornung.ResourceToolkit.ErrorProcessingResourceFile}" + Environment.NewLine + ex.Message, entry.Value.FileName);
				}
			}
		}
		
	}
}
