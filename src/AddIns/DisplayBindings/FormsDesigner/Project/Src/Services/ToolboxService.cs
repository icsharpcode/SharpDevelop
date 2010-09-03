// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Drawing.Design;

using ICSharpCode.Core;

namespace ICSharpCode.FormsDesigner.Services
{
	public delegate void ToolboxEventHandler(object sender, ToolboxEventArgs tea);
	
	public class ToolboxEventArgs : EventArgs
	{
		ToolboxItem   item     = null;
		string        category = null;
		IDesignerHost host     = null;
		
		public ToolboxEventArgs(ToolboxItem item, string category, IDesignerHost host)
		{
			this.item     = item;
			this.category = category;
			this.host     = host;
		}
		
		public ToolboxItem Item {
			get {
				return item;
			}
		}
		
		public string Category {
			get {
				return category;
			}
		}
		
		public IDesignerHost Host {
			get {
				return host;
			}
		}
	}
	
	/// <summary>
	/// Provides access to the toolbox in the development environment.
	/// </summary>
	/// <remarks>
	/// Provides designers with the ability to configure what tools
	/// are available on the toolbox.
	/// </remarks>
	public class ToolboxService : IToolboxService
	{
		static readonly string ALL_HOSTS      = "_all_hosts_";
		static readonly string ALL_CATEGORIES = "_all_categories_";
		
		IDictionary toolboxByCategory = new ListDictionary();
		IDictionary toolboxByHost     = new ListDictionary();
		ArrayList toolboxItems        = new ArrayList();
		
		IDictionary creators          = new HybridDictionary();
		IDictionary creatorsByHost    = new ListDictionary();
		
		string selectedCategory  = null;
		ToolboxItem selectedItem = null;
		
		// Constructor
		public ToolboxService()
		{
			IList list = new ArrayList();
			toolboxByCategory.Add(ALL_CATEGORIES, list);
			
			list = new ArrayList();
			toolboxByHost.Add(ALL_HOSTS, list);
		}
		
		// Properties
		
		/// <summary>
		/// Gets the names of all the tool categories currently on the toolbox.
		/// </summary>
		/// <value>
		/// A <see cref="System.Drawing.Design.CategoryNameCollection">
		/// containing the tool categories.
		/// </value>
		public CategoryNameCollection CategoryNames {
			get {
				string[] names = new string[toolboxByCategory.Count];
				toolboxByCategory.Keys.CopyTo(names, 0);
				return new CategoryNameCollection(names);
			}
		}
		
		/// <summary>
		/// Gets or sets the name of the currently selected tool category
		/// from the toolbox.
		/// </summary>
		/// <value>
		/// The name of the currently selected category.
		/// </value>
		/// <remarks>
		/// The implementation of this property's "set" accessor fires the
		/// events <see cref="SelectedCategoryChanging"> and
		/// <see cref="SelectedCategoryChanged">.
		/// </remarks>
		public string SelectedCategory {
			get {
				return selectedCategory;
			}
			set {
				if (value != selectedCategory) {
					FireSelectedCategoryChanging();
					selectedCategory = value;
					FireSelectedCategoryChanged();
				}
			}
		}
		
		// Methods
		/// <summary>
		/// Adds a new toolbox item creator.
		/// </summary>
		/// <param name="creator">
		/// A <see cref="System.Drawing.Design.ToolboxItemCreatorCallback">
		/// that can create a component when the toolbox item
		/// is invoked. </param>
		/// <param name="format">
		/// The data format this creator responds to. If a creator responds
		/// to more than one format, call AddCreator more than once. It is
		/// an error to add more than one creator for the same format.
		/// </param>
		/// <remarks>
		/// A toolbox item creator is used to handle data formats other than
		/// the standard native format of the toolbox service. Typically, the
		/// standard toolbox service format should be used, because it provides
		/// ample opportunity for customization in an object-oriented way.
		/// However, there are times when a legacy data format may need to be
		/// supported. A toolbox item creator is the mechanism by which these
		/// legacy data formats may be converted into toolbox items.
		/// </remarks>
		public void AddCreator(ToolboxItemCreatorCallback creator, string format)
		{
			AddCreator(creator, format, null);
		}
		
		/// <summary>
		/// Adds a new toolbox item creator.
		/// </summary>
		/// <param name="creator">
		/// A <see cref="System.Drawing.Design.ToolboxItemCreatorCallback">
		/// that can create a component when the toolbox item
		/// is invoked. </param>
		/// <param name="format">
		/// The data format this creator responds to. If a creator responds
		/// to more than one format, call AddCreator more than once. It is
		/// an error to add more than one creator for the same format.
		/// </param>
		/// <param name="host">
		/// The designer host to associate with the creator. If this parameter
		/// is set to a null reference (Nothing in Visual Basic), this creator
		/// will be available to all designers. If a designer host is supplied,
		/// the creator will only be available to designers using the specified
		/// host.
		/// </param>
		/// <remarks>
		/// A toolbox item creator is used to handle data formats other than
		/// the standard native format of the toolbox service. Typically, the
		/// standard toolbox service format should be used, because it provides
		/// ample opportunity for customization in an object-oriented way.
		/// However, there are times when a legacy data format may need to be
		/// supported. A toolbox item creator is the mechanism by which these
		/// legacy data formats may be converted into toolbox items.
		/// <para>
		/// This implemetation does add the specified creator to a collection,
		/// but at this time I have no idea what to do with it!
		/// </para>
		/// </remarks>
		public void AddCreator(ToolboxItemCreatorCallback creator, string format, IDesignerHost host)
		{
			LoggingService.DebugFormatted("\tDefaultToolboxService:AddCreator({0}, {1}, {2})", creator, format, host);
			if (host == null) {
				creators.Add(format, creator);
			} else {
				IDictionary creatorsDict = (IDictionary)creatorsByHost[host];
				if (creatorsDict == null) {
					creatorsDict = new HybridDictionary();
					creatorsByHost.Add(host, creatorsDict);
				}
				creatorsDict[format] =creator;
			}
		}
		
		void AddItemToToolbox(ToolboxItem toolboxItem, string category, IDesignerHost host)
		{
			toolboxItems.Add(toolboxItem);
			
			if (host != null) {
				IList list = (IList)toolboxByHost[host];
				if (list == null) {
					list = new ArrayList();
					toolboxByHost.Add(host, list);
				}
				list.Add(toolboxItem);
			} else {
				IList list = (IList)toolboxByHost[ALL_HOSTS];
				list.Add(toolboxItem);
			}
			
			if (category != null) {
				IList list = (IList)toolboxByCategory[category];
				if (list == null) {
					list = new ArrayList();
					toolboxByCategory.Add(category, list);
				}
				list.Add(toolboxItem);
			} else {
				IList list = (IList)toolboxByCategory[ALL_CATEGORIES];
				list.Add(toolboxItem);
			}
			
			FireToolboxItemAdded(toolboxItem, category, host);
		}
		
		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, IDesignerHost host)
		{
			AddItemToToolbox(toolboxItem, category, host);
		}
		
		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, IDesignerHost host)
		{
			this.AddLinkedToolboxItem(toolboxItem, null, host);
		}
		
		public void AddToolboxItem(ToolboxItem toolboxItem)
		{
			this.AddItemToToolbox(toolboxItem, null, null);
		}
		
		public void AddToolboxItem(ToolboxItem toolboxItem, string category)
		{
			this.AddItemToToolbox(toolboxItem, category, null);
		}
		
		public ToolboxItem DeserializeToolboxItem(object serializedObject)
		{
			return DeserializeToolboxItem(serializedObject, null);
		}
		
		public ToolboxItem DeserializeToolboxItem(object serializedObject, IDesignerHost host)
		{
			LoggingService.DebugFormatted("DeserializeToolboxItem {0} host {1}", serializedObject, host);
			if (serializedObject is System.Windows.Forms.IDataObject) {
				if (((System.Windows.Forms.IDataObject)serializedObject).GetDataPresent(typeof(ToolboxItem))) {
					ToolboxItem item = (ToolboxItem) ((System.Windows.Forms.IDataObject)serializedObject).GetData(typeof(ToolboxItem));
					
					ArrayList list;
					if (host != null) {
						list = (ArrayList)toolboxByHost[host];
						if (list != null && list.Contains(item)) {
							LoggingService.Warn(item.TypeName);
							return item;
						}
					}
					list = (ArrayList)toolboxByHost[ALL_HOSTS];
					if (list != null && list.Contains(item)) {
						return item;
					}
				}
			}
			LoggingService.WarnFormatted("DeserializeToolboxItem {0} host {1} return null", serializedObject, host);
			return null;
		}
		
		public ToolboxItem GetSelectedToolboxItem()
		{
			return selectedItem;
		}
		
		public ToolboxItem GetSelectedToolboxItem(IDesignerHost host)
		{
			IList list = (IList)toolboxByHost[host];
			if (list != null && list.Contains(selectedItem)) {
				return selectedItem;
			}
			
			list = (IList)toolboxByHost[ALL_HOSTS];
			if (list.Contains(selectedItem)) {
				return selectedItem;
			}
			return null;
		}
		
		public ToolboxItemCollection GetToolboxItems()
		{
			LoggingService.Debug("ToolboxService: GetToolboxItems");
			ToolboxItem[] items = new ToolboxItem[toolboxItems.Count];
			toolboxItems.CopyTo(items);
			return new ToolboxItemCollection(items);
		}
		
		public ToolboxItemCollection GetToolboxItems(string category)
		{
			LoggingService.Debug("ToolboxService: GetToolboxItems category " + category);
			if (category == null) {
				category = ALL_CATEGORIES;
			}
			
			ArrayList list = (ArrayList)toolboxByCategory[category];
			list.Add((ArrayList)toolboxByCategory[ALL_CATEGORIES]);
			ToolboxItem[] items = new ToolboxItem[list.Count];
			toolboxItems.CopyTo(items);
			return new ToolboxItemCollection(items);
		}
		
		public ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host)
		{
			LoggingService.DebugFormatted("ToolboxService: GetToolboxItems category {0} host {1}", category, host);
			if (category == null) {
				category = ALL_CATEGORIES;
			}
			
			ArrayList hList = null;
			
			if (host == null) {
				hList = (ArrayList)toolboxByHost[ALL_HOSTS];
			} else {
				hList = (ArrayList)toolboxByHost[host];
			}
			
			ArrayList cList = (ArrayList)toolboxByCategory[category];
			ArrayList list = new ArrayList();
			
			foreach (ToolboxItem item in hList) {
				if (cList.Contains(item)) {
					list.Add(item);
				}
			}
			
			ToolboxItem[] items = new ToolboxItem[list.Count];
			toolboxItems.CopyTo(items);
			return new ToolboxItemCollection(items);
		}
		
		public ToolboxItemCollection GetToolboxItems(IDesignerHost host)
		{
			ArrayList hList = null;
			
			if(host == null) {
				hList = (ArrayList)toolboxByHost[ALL_HOSTS];
			} else {
				hList = (ArrayList)toolboxByHost[host];
			}
			ArrayList list = (ArrayList)toolboxByHost[host];
			list.Add((ArrayList)toolboxByHost[ALL_HOSTS]);
			ToolboxItem[] items = new ToolboxItem[list.Count];
			toolboxItems.CopyTo(items);
			return new ToolboxItemCollection(items);
		}
		
		public bool IsSupported(object serializedObject, ICollection filterAttributes)
		{
			return true;
		}
		
		public bool IsSupported(object serializedObject, IDesignerHost host)
		{
			return true;
		}
		
		public bool IsToolboxItem(object serializedObject)
		{
			if (serializedObject is System.Windows.Forms.IDataObject) {
				if (((System.Windows.Forms.IDataObject)serializedObject).GetDataPresent(typeof(ToolboxItem))) {
					return true;
				}
			}
			return false;
		}
		
		public bool IsToolboxItem(object serializedObject, IDesignerHost host)
		{
			// needed for Toolbox drag & drop
			if (serializedObject is System.Windows.Forms.IDataObject) {
				if (((System.Windows.Forms.IDataObject)serializedObject).GetDataPresent(typeof(ToolboxItem))) {
					ToolboxItem item = (ToolboxItem) ((System.Windows.Forms.IDataObject)serializedObject).GetData(typeof(ToolboxItem));
					if (host != null) {
						ArrayList list = (ArrayList)toolboxByHost[host];
						if (list != null && list.Contains(item)) {
							return true;
						}
						list = (ArrayList)toolboxByHost[ALL_HOSTS];
						if (list != null && list.Contains(item)) {
							return true;
						}
					}
				}
			}
			return false;
		}
		
		public void Refresh()
		{
			//System.Console.WriteLine("\tDefaultToolboxService:Refresh()");
		}
		
		public void RemoveCreator(string format)
		{
			RemoveCreator(format, null);
		}
		
		public void RemoveCreator(string format, IDesignerHost host)
		{
			if (host == null) {
				creators.Remove(format);
			} else {
				HybridDictionary creatorsDict = creatorsByHost[host] as HybridDictionary;
				if (creatorsDict != null) {
					creatorsDict.Remove(format);
					if (creatorsDict.Count == 0)
						creatorsByHost.Remove(host);
				}
			}
		}
		
		public void RemoveToolboxItem(ToolboxItem toolboxItem)
		{
			toolboxItems.Remove(toolboxItem);
			ArrayList list = (ArrayList)toolboxByCategory[ALL_CATEGORIES];
			list.Remove(toolboxItem);
			list = (ArrayList)toolboxByHost[ALL_HOSTS];
			list.Remove(toolboxItem);
			FireToolboxItemRemoved(toolboxItem, null, null);
		}
		
		public void RemoveToolboxItem(ToolboxItem toolboxItem, string category)
		{
			toolboxItems.Remove(toolboxItem);
			ArrayList list = (ArrayList)toolboxByCategory[category];
			list.Remove(toolboxItem);
			FireToolboxItemRemoved(toolboxItem, category, null);
		}
		
		public void SelectedToolboxItemUsed()
		{
			FireSelectedItemUsed();
		}
		
		public object SerializeToolboxItem(ToolboxItem toolboxItem)
		{
			return null;
		}
		
		public bool SetCursor()
		{
			if (selectedItem == null) {
				return false;
			}
			if (selectedItem.DisplayName == "Pointer") {
				return false;
			}
			return true;
		}
		
		public void SetSelectedToolboxItem(ToolboxItem toolboxItem)
		{
			if (toolboxItem != selectedItem) {
				FireSelectedItemChanging();
				selectedItem = toolboxItem;
				FireSelectedItemChanged();
			}
		}
		
		// Event helpers
		void FireSelectedCategoryChanging()
		{
			if (SelectedCategoryChanging != null) {
				SelectedCategoryChanging(this, EventArgs.Empty);
			}
		}
		
		void FireSelectedItemChanged()
		{
			if (SelectedItemChanged != null) {
				SelectedItemChanged(this, EventArgs.Empty);
			}
		}
		
		void FireSelectedItemChanging()
		{
			if (SelectedItemChanging != null) {
				SelectedItemChanging(this, EventArgs.Empty);
			}
		}
		
		void FireSelectedCategoryChanged()
		{
			if (SelectedCategoryChanged != null) {
				SelectedCategoryChanged(this, EventArgs.Empty);
			}
		}
		
		void FireSelectedItemUsed()
		{
			if (SelectedItemUsed != null) {
				SelectedItemUsed(this, EventArgs.Empty);
			}
		}
		
		void FireToolboxItemAdded(ToolboxItem item, string category, IDesignerHost host)
		{
			if (ToolboxItemAdded != null) {
				ToolboxItemAdded(this, new ToolboxEventArgs(item, category, host));
			}
		}
		
		void FireToolboxItemRemoved(ToolboxItem item, string category, IDesignerHost host)
		{
			if (ToolboxItemAdded != null) {
				ToolboxItemRemoved(this, new ToolboxEventArgs(item, category, host));
			}
		}
		
		// Events
		public event EventHandler SelectedCategoryChanging;
		public event EventHandler SelectedCategoryChanged;
		public event EventHandler SelectedItemChanging;
		public event EventHandler SelectedItemChanged;
		public event EventHandler SelectedItemUsed;
		public event ToolboxEventHandler ToolboxItemAdded;
		public event ToolboxEventHandler ToolboxItemRemoved;
	}
}
