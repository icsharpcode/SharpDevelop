/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 03.10.2007
 * Zeit: 17:19
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing.Design;

using ICSharpCode.Core;

namespace ICSharpCode.Reports.Addin
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
	/// Description of ToolboxService.
	/// </summary>
	public class ToolboxService:IToolboxService
	{
		ArrayList toolboxItems = new ArrayList();
		ToolboxItem selectedItem;
		
		public ToolboxService()
		{
		}
		
		public CategoryNameCollection CategoryNames {
			get {
				return new CategoryNameCollection(new string [] {"Reporting"});;
			}
		}
		
		public string SelectedCategory {
			get {
				System.Console.WriteLine("ToolboxSerivce:SelectedCategory");
				return String.Empty;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public void AddCreator(ToolboxItemCreatorCallback creator, string format)
		{
			this.AddCreator(creator,format,null);
		}
		
		public void AddCreator(ToolboxItemCreatorCallback creator, string format, IDesignerHost host)
		{
//			System.Console.WriteLine(" AddCreator for");
//			System.Console.WriteLine("\t {0} / {1} / {2}",creator.ToString(),format,host.ToString());
		}
		
		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, IDesignerHost host)
		{
			AddItemToToolbox(toolboxItem,null,host);
		}
		
		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, IDesignerHost host)
		{
			AddItemToToolbox(toolboxItem, category, host);
		}
		
		public void AddToolboxItem(ToolboxItem toolboxItem)
		{
			AddItemToToolbox(toolboxItem,null,null);
		}
		
		public void AddToolboxItem(ToolboxItem toolboxItem, string category)
		{
			AddItemToToolbox(toolboxItem,category,null);
		}
		
		void AddItemToToolbox(ToolboxItem toolboxItem, string category, IDesignerHost host)
		{
			toolboxItems.Add(toolboxItem);
			System.Console.WriteLine("{0}",toolboxItems.Count);
			FireToolboxItemAdded(toolboxItem, category, host);
		}
		
		public ToolboxItem DeserializeToolboxItem(object serializedObject)
		{
			System.Console.WriteLine("DeserializeToolboxItem throw exception");
			throw new NotImplementedException();
		}
		
		public ToolboxItem DeserializeToolboxItem(object serializedObject, IDesignerHost host)
		{
			ToolboxItem item = (ToolboxItem) ((System.Windows.Forms.IDataObject)serializedObject).GetData(typeof(ToolboxItem));
			return item;
		}
		
		public ToolboxItem GetSelectedToolboxItem()
		{
			return selectedItem;
		}
		
		public ToolboxItem GetSelectedToolboxItem(IDesignerHost host)
		{
			return this.selectedItem;
		}
		
		public ToolboxItemCollection GetToolboxItems()
		{
			ToolboxItem[] items = new ToolboxItem[toolboxItems.Count];
			toolboxItems.CopyTo(items);
			return new ToolboxItemCollection(items);
		}
		
		public ToolboxItemCollection GetToolboxItems(IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItemCollection GetToolboxItems(string category)
		{
			System.Console.WriteLine("ddddd");
			throw new NotImplementedException();
		}
		
		public ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public bool IsSupported(object serializedObject, IDesignerHost host)
		{
			return true;
		}
		
		public bool IsSupported(object serializedObject, ICollection filterAttributes)
		{
			return true;
		}
		
		public bool IsToolboxItem(object serializedObject)
		{
			System.Console.WriteLine("gggggg");
			if (serializedObject is System.Windows.Forms.IDataObject) {
				if (((System.Windows.Forms.IDataObject)serializedObject).GetDataPresent(typeof(ToolboxItem))) {
					return true;
				}
			}
			return false;
		}
		
		public bool IsToolboxItem(object serializedObject, IDesignerHost host)
		{
			System.Console.WriteLine("Toolbox:IsToolboxItem");
			// needed for Toolbox drag & drop
			if (serializedObject is System.Windows.Forms.IDataObject) {
				if (((System.Windows.Forms.IDataObject)serializedObject).GetDataPresent(typeof(ToolboxItem))) {
					ToolboxItem item = (ToolboxItem) ((System.Windows.Forms.IDataObject)serializedObject).GetData(typeof(ToolboxItem));
//					if (host != null) {
//						ArrayList list = (ArrayList)toolboxByHost[host];
//						if (list != null && list.Contains(item)) {
//							return true;
//						}
//						list = (ArrayList)toolboxByHost[ALL_HOSTS];
//						if (list != null && list.Contains(item)) {
					return true;
//						}
//					}
				}
			}
			return false;
		}
		
		public void Refresh()
		{
			System.Console.WriteLine("Toolbox:Refresh()");
			throw new NotImplementedException();
		}
		
		public void RemoveCreator(string format)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveCreator(string format, IDesignerHost host)
		{
			System.Console.WriteLine("Toolbox:removeCreator");
			throw new NotImplementedException();
		}
		
		public void RemoveToolboxItem(ToolboxItem toolboxItem)
		{
			this.toolboxItems.Remove(toolboxItem);
			FireToolboxItemRemoved(toolboxItem, null, null);
		}
		
		public void RemoveToolboxItem(ToolboxItem toolboxItem, string category)
		{
			System.Console.WriteLine("RemoveToolboxItem");
			toolboxItems.Remove(toolboxItem);
			FireToolboxItemRemoved(toolboxItem, null, null);
		}
		
		public void SelectedToolboxItemUsed()
		{
			FireSelectedItemUsed();
		}
		
		public object SerializeToolboxItem(ToolboxItem toolboxItem)
		{
			System.Console.WriteLine("nnnn");
			throw new NotImplementedException();
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
		
		#region EvenHelpers
		/*
		void FireSelectedCategoryChanging()
		{
			if (SelectedCategoryChanging != null) {
				SelectedCategoryChanging(this, EventArgs.Empty);
			}
		}
		*/
		
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
		
		/*
		void FireSelectedCategoryChanged()
		{
			if (SelectedCategoryChanged != null) {
				SelectedCategoryChanged(this, EventArgs.Empty);
			}
		}
		*/
		
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
//		public event EventHandler SelectedCategoryChanging;
//		public event EventHandler SelectedCategoryChanged;
		public event EventHandler SelectedItemChanging;
		public event EventHandler SelectedItemChanged;
		public event EventHandler SelectedItemUsed;
		public event ToolboxEventHandler ToolboxItemAdded;
		public event ToolboxEventHandler ToolboxItemRemoved;
		#endregion
	}
}
