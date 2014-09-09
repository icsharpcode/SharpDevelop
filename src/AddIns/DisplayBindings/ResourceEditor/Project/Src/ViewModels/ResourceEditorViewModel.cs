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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;
using ResourceEditor.Views;

namespace ResourceEditor.ViewModels
{
	public class ChangedDirtyStateEventArgs : EventArgs
	{
		public ChangedDirtyStateEventArgs(bool isDirty)
		{
			this.IsDirty = isDirty;
		}
		
		public bool IsDirty {
			get;
			private set;
		}
	}
	
	/// <summary>
	/// Main view model for resource editor.
	/// </summary>
	public class ResourceEditorViewModel : DependencyObject
	{
		readonly ObservableCollection<ResourceItem> resourceItems;
		readonly HashSet<string> resourceItemNames;
		readonly ObservableCollection<ResourceItem> metadataItems;
		readonly Dictionary<ResourceItemEditorType, IResourceItemView> itemViews;
		bool longUpdateRunning;
		ResourceItem editedResourceItem;
		string originalNameOfEditedItem;
		
		IResourceEditorView view;
		
		public event EventHandler<ChangedDirtyStateEventArgs> DirtyStateChanged;
		
		public static readonly DependencyProperty SearchTermProperty =
			DependencyProperty.Register("SearchTerm", typeof(string), typeof(ResourceEditorViewModel),
				new FrameworkPropertyMetadata());
		
		public string SearchTerm {
			get { return (string)GetValue(SearchTermProperty); }
			set { SetValue(SearchTermProperty, value); }
		}
		
		public IList SelectedItems {
			get {
				if (view != null) {
					return view.SelectedItems;
				}
				
				return null;
			}
		}
		
		public ResourceEditorViewModel()
		{
			resourceItems = new ObservableCollection<ResourceItem>();
			resourceItemNames = new HashSet<string>();
			metadataItems = new ObservableCollection<ResourceItem>();
			itemViews = new Dictionary<ResourceItemEditorType, IResourceItemView>();
		}
		
		public ObservableCollection<ResourceItem> ResourceItems {
			get {
				return resourceItems;
			}
		}
		
		public IEnumerable<ResourceItem> GetSelectedItems()
		{
			return SelectedItems.OfType<ResourceItem>() ?? new ResourceItem[0];
		}
		
		/// <summary>
		/// Checks whether a resource name is existing in currently open file.
		/// </summary>
		/// <param name="name">Resource name to check.</param>
		/// <returns><c>True</c> if name exists, <c>false</c> otherwise.</returns>
		public bool ContainsResourceName(string name)
		{
			return resourceItemNames.Contains(name);
		}
		
		public void AddItemView(ResourceItemEditorType itemType, IResourceItemView view)
		{
			itemViews[itemType] = view;
		}
		
		public IResourceEditorView View {
			get {
				return view;
			}
			set {
				if (view != null) {
					view.SelectionChanged -= View_SelectionChanged;
					view.EditingStartRequested -= View_EditingStartRequested;
					view.AddingNewItemRequested -= View_AddingNewItemRequested;
					ResourceItems.CollectionChanged -= ResourceItems_CollectionChanged;
				}
				
				view = value;
				if (view != null) {
					// Bind this model to new view
					view.DataContext = this;
					view.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, (s, e) => Cut(), (s, e) => e.CanExecute = EnableCut));
					view.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, (s, e) => Copy(), (s, e) => e.CanExecute = EnableCopy));
					view.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, (s, e) => Paste(), (s, e) => e.CanExecute = EnablePaste));
					view.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, (s, e) => Delete(), (s, e) => e.CanExecute = EnableDelete));
					view.CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, (s, e) => SelectAll(), (s, e) => e.CanExecute = EnableSelectAll));
					
					view.FilterPredicate = resourceItem => {
						if (!resourceItem.IsNew && (SearchTerm != null)) {
							bool isMatch = resourceItem.Name.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0
								|| ((resourceItem.Content ?? "").IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
								|| ((resourceItem.Comment ?? "").IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
							if (isMatch)
								resourceItem.Highlight(SearchTerm);
							return isMatch;
						}
						return true;
					};
					view.SelectionChanged += View_SelectionChanged;
					view.EditingStartRequested += View_EditingStartRequested;
					view.AddingNewItemRequested += View_AddingNewItemRequested;
					ResourceItems.CollectionChanged += ResourceItems_CollectionChanged;
				}
			}
		}
		
		void ResourceItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Remove:
					if (e.OldItems != null) {
						var oldItems = e.OldItems.OfType<ResourceItem>();
						foreach (var item in oldItems) {
							resourceItemNames.Remove(item.Name);
						}
					}
					break;
				case NotifyCollectionChangedAction.Add:
					if (e.NewItems != null) {
						var newItems = e.NewItems.OfType<ResourceItem>();
						foreach (var item in newItems) {
							if (!resourceItemNames.Contains(item.Name))
								resourceItemNames.Add(item.Name);
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					resourceItemNames.Clear();
					break;
			}
			
			// Changes in collection of ResourceItems => make dirty
			MakeDirty();
		}

		void View_SelectionChanged(object sender, EventArgs e)
		{
			if (longUpdateRunning)
				return;
			
			ResourceItem selectedItem = SelectedItems.OfType<ResourceItem>().FirstOrDefault();
			if (selectedItem != null) {
				if (itemViews.ContainsKey(selectedItem.ResourceType)) {
					var itemView = itemViews[selectedItem.ResourceType];
					itemView.ResourceItem = selectedItem;
					view.SetItemView(itemView);
				} else {
					view.SetItemView(null);
				}
			}
			
			// When selection is changed, reset any new item marked with IsNew to pass the filter
			foreach (var newItem in ResourceItems.Where(ri => ri.IsNew)) {
				newItem.IsNew = false;
			}
		}
		
		void OnChangedDirtyState(bool isDirty)
		{
			if (DirtyStateChanged != null) {
				DirtyStateChanged(this, new ChangedDirtyStateEventArgs(isDirty));
			}
		}
		
		public void MakeDirty()
		{
			OnChangedDirtyState(true);
		}
		
		public void SelectItem(ResourceItem item)
		{
			view.SelectItem(item);
		}
		
		public void StartEditing()
		{
			// Start editing currently selected item
			var firstSelectedItem = SelectedItems.OfType<ResourceItem>().FirstOrDefault();
			if (firstSelectedItem != null) {
				editedResourceItem = firstSelectedItem;
				originalNameOfEditedItem = editedResourceItem.Name;
				firstSelectedItem.IsEditing = true;
			}
		}

		void View_EditingStartRequested(object sender, EventArgs e)
		{
			StartEditing();
		}

		void View_AddingNewItemRequested(object sender, EventArgs e)
		{
			AddStringEntry();
		}
		
		public void AddStringEntry()
		{
			int count = 1;
			string newNameBase = "New string entry ";
			string newName = newNameBase + count;
			
			while (ContainsResourceName(newName)) {
				count++;
				newName = newNameBase + count;
			}
			
			var selectedItem = GetSelectedItems().FirstOrDefault();
			ResourceItem item = new ResourceItem(this, newName, "");
			item.IsNew = true;
			if (selectedItem != null)
				item.SortingCriteria = selectedItem.Name;
			else
				item.SortingCriteria = item.Name;
			ResourceItems.Add(item);
			SelectItem(item);
			StartEditing();
		}
		
		public void AddBooleanEntry()
		{
			int count = 1;
			string newNameBase = "New boolean entry ";
			string newName = newNameBase + count;
			
			while (ContainsResourceName(newName)) {
				count++;
				newName = newNameBase + count;
			}
			
			var selectedItem = GetSelectedItems().FirstOrDefault();
			ResourceItem item = new ResourceItem(this, newName, false);
			item.IsNew = true;
			if (selectedItem != null)
				item.SortingCriteria = selectedItem.Name;
			else
				item.SortingCriteria = item.Name;
			ResourceItems.Add(item);
			SelectItem(item);
			StartEditing();
		}
		
		void StartUpdate()
		{
			// When loading many items at once, temporarily unbind view from model
			view.DataContext = null;
		}
		
		void EndUpdate()
		{
			view.DataContext = this;
		}
		
		public void LoadFile(FileName filename, Stream stream)
		{
			StartUpdate();
			
			resourceItems.Clear();
			metadataItems.Clear();
			switch (Path.GetExtension(filename).ToLowerInvariant()) {
				case ".resx":
					ResXResourceReader rx = new ResXResourceReader(stream);
					ITypeResolutionService typeResolver = null;
					rx.BasePath = Path.GetDirectoryName(filename);
					rx.UseResXDataNodes = true;
					IDictionaryEnumerator n = rx.GetEnumerator();
					while (n.MoveNext()) {
						ResXDataNode node = (ResXDataNode)n.Value;
						resourceItems.Add(new ResourceItem(this, node.Name, node.GetValue(typeResolver), node.Comment));
					}
					
					n = rx.GetMetadataEnumerator();
					while (n.MoveNext()) {
						ResXDataNode node = (ResXDataNode)n.Value;
						metadataItems.Add(new ResourceItem(this, node.Name, node.GetValue(typeResolver)));
					}
					
					rx.Close();
					break;
				case ".resources":
					ResourceReader rr = null;
					try {
						rr = new ResourceReader(stream);
						foreach (DictionaryEntry entry in rr) {
							resourceItems.Add(new ResourceItem(this, entry.Key.ToString(), entry.Value));
						}
					} finally {
						if (rr != null) {
							rr.Close();
						}
					}
					break;
			}
			
			EndUpdate();
		}
		
		public void SaveFile(FileName filename, Stream stream)
		{
			switch (Path.GetExtension(filename).ToLowerInvariant()) {
				case ".resx":
					// write XML resource
					ResXResourceWriter rxw = new ResXResourceWriter(stream, t => ResXConverter.ConvertTypeName(t, filename));
					foreach (ResourceItem entry in resourceItems) {
						if (entry != null) {
							rxw.AddResource(entry.ToResXDataNode(t => ResXConverter.ConvertTypeName(t, filename)));
						}
					}
					foreach (ResourceItem entry in metadataItems) {
						if (entry != null) {
							rxw.AddMetadata(entry.Name, entry.ResourceValue);
						}
					}
					rxw.Generate();
					rxw.Close();
					break;
				default:
					// write default resource
					ResourceWriter rw = new ResourceWriter(stream);
					foreach (ResourceItem entry in resourceItems) {
						rw.AddResource(entry.Name, entry.ResourceValue);
					}
					rw.Generate();
					rw.Close();
					break;
			}
		}
		
		#region Standard clipboard commands
		
		public bool EnableCut {
			get {
				return SelectedItems.Count > 0;
			}
		}
		
		public bool EnableCopy {
			get {
				return SelectedItems.Count > 0;
			}
		}
		
		public bool EnablePaste {
			get {
				return true;
			}
		}
		
		public bool EnableDelete {
			get {
				return SelectedItems.Count > 0;
			}
		}
		
		public bool EnableSelectAll {
			get {
				return true;
			}
		}
		
		public void Cut()
		{
			if (SelectedItems.Count == 0)
				return;
			
			Hashtable tmphash = new Hashtable();
			foreach (var selectedItem in SelectedItems.OfType<ResourceItem>().ToList()) {
				tmphash.Add(selectedItem.Name, selectedItem.ResourceValue);
				resourceItems.Remove(selectedItem);
			}
			SD.Clipboard.SetDataObject(tmphash);
		}
		
		public void Copy()
		{
			if (SelectedItems.Count == 0)
				return;
			
			Hashtable tmphash = new Hashtable();
			foreach (var selectedItem in SelectedItems.OfType<ResourceItem>().ToList()) {
				object resourceValue = GetClonedResource(selectedItem.ResourceValue);
				tmphash.Add(selectedItem.Name, resourceValue);
			}
			SD.Clipboard.SetDataObject(tmphash);
		}
		
		public void Paste()
		{
			IDataObject dob = Clipboard.GetDataObject();
			if (dob == null)
				return;
			
			if (dob.GetDataPresent(typeof(Hashtable).FullName)) {
				Hashtable tmphash = (Hashtable)dob.GetData(typeof(Hashtable));
				foreach (DictionaryEntry entry in tmphash) {
					object resourceValue = GetClonedResource(entry.Value);
					ResourceItem item;
					
					if (!resourceItemNames.Contains((string)entry.Key)) {
						item = new ResourceItem(this, entry.Key.ToString(), resourceValue);
					} else {
						int count = 1;
						string newNameBase = entry.Key + " ";
						string newName = newNameBase + count;
						
						while (resourceItemNames.Contains(newName)) {
							count++;
							newName = newNameBase + count;
						}
						item = new ResourceItem(this, newName, resourceValue);
					}
					resourceItems.Add(item);
					SelectItem(item);
				}
			}
		}

		/// <summary>
		/// Clones a resource if the <paramref name="resource"/>
		/// is cloneable.
		/// </summary>
		/// <param name="resource">A resource to clone.</param>
		/// <returns>A cloned resource if the object implements
		/// the ICloneable interface, otherwise the
		/// <paramref name="resource"/> object.</returns>
		object GetClonedResource(object resource)
		{
			object clonedResource = null;
			
			ICloneable cloneableResource = resource as ICloneable;
			if (cloneableResource != null) {
				clonedResource = cloneableResource.Clone();
			} else {
				clonedResource = resource;
			}
			
			return clonedResource;
		}
		
		public void Delete()
		{
			if (SelectedItems.Count > 0) {
				if (!SD.MessageService.AskQuestion("${res:ResourceEditor.DeleteEntry.Confirm}", "${res:ResourceEditor.DeleteEntry.Title}"))
					return;

				if (SelectedItems.Count == resourceItems.Count) {
					// Little performance improvement for the case "Select All" + "Delete"
					SelectedItems.Clear();
					resourceItems.Clear();
				} else {
					foreach (var item in SelectedItems.OfType<ResourceItem>().ToList()) {
						resourceItems.Remove(item);
					}
				}
			}
		}
		
		public void SelectAll()
		{
			longUpdateRunning = true;
			foreach (var i in resourceItems)
				SelectedItems.Add(i);
			longUpdateRunning = false;
		}
		
		#endregion
	}
}
