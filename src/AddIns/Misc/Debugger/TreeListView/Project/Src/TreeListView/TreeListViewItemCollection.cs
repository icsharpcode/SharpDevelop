using System;
using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
	/// <summary>
	/// Represents the collection of items in a TreeListView control or in a TreeListViewItem
	/// </summary>
	public class TreeListViewItemCollection : CollectionBase
	{
		/// <summary>
		/// Comparer for TreeListViewItem
		/// </summary>
		public class TreeListViewItemCollectionComparer : ITreeListViewItemComparer
		{
			#region Order Property
			private SortOrder _sortorder = SortOrder.Ascending;
			/// <summary>
			/// Sort order
			/// </summary>
			public SortOrder SortOrder
			{
				get{return(_sortorder);}
				set{_sortorder = value;}
			}
			private int _column;
			/// <summary>
			/// Column for the comparison
			/// </summary>
			public int Column
			{
				get{return(_column);}
				set{_column = value;}
			}
			#endregion
			#region Constructor
			/// <summary>
			/// Create a new instance of  Comparer
			/// </summary>
			public TreeListViewItemCollectionComparer() : this(SortOrder.Ascending, 0)
			{}
			/// <summary>
			/// Create a new instance of  Comparer
			/// </summary>
			/// <param name="order"></param>
			public TreeListViewItemCollectionComparer(SortOrder order) : this(order, 0)
			{
				SortOrder = order;
			}
			/// <summary>
			/// Create a new instance of  Comparer
			/// </summary>
			/// <param name="order"></param>
			/// <param name="column"></param>
			public TreeListViewItemCollectionComparer(SortOrder order, int column)
			{
				SortOrder = order;
				_column = column;
			}
			#endregion
			#region Compare
			/// <summary>
			/// Compare two TreeListViewItems
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(object x, object y)
			{
				TreeListViewItem a = (TreeListViewItem) x;
				TreeListViewItem b = (TreeListViewItem) y;
				int res = 1;
				if(Column < a.SubItems.Count && Column < b.SubItems.Count)
					res = string.CompareOrdinal(a.SubItems[Column].Text.ToUpper(), b.SubItems[Column].Text.ToUpper());
				switch(SortOrder)
				{
					case SortOrder.Ascending:
						return(res);
					case SortOrder.Descending:
						return(-res);
					default:
						return(1);
				}
			}
			#endregion
		}
		#region Private delegates
		private delegate void VoidHandlerSortOrder(SortOrder value);
		private delegate void VoidHandler();
		private delegate void ChangeChildrenCheckStateRecursivelyHandler(CheckState state);
		private delegate TreeListViewItemCollection GetCollectionHandler();
		private delegate string GetStringHandler();
		private delegate int GetIntHandler();
		#endregion

		#region Properties
			#region Sortable
			private bool _sortable = true;
			/// <summary>
			/// Gets or sets a value indicating whether the items of the collection can be reorder while the sort function is being called
			/// </summary>
			public bool Sortable
			{
				get
				{
					return _sortable;
				}
				set
				{
					_sortable = value;
				}
			}
			#endregion
			#region SortOrder
			/// <summary>
			/// Get or set the new sortorder (apply automatically the sort function
			/// if the sortorder value is changed)
			/// </summary>
			public SortOrder SortOrder
			{
				get{return(Comparer.SortOrder);}
				set{Comparer.SortOrder = value;
					Sort(false);}
			}
			#endregion
			#region Comparer
			private ITreeListViewItemComparer _comparer = new TreeListViewItemCollectionComparer(SortOrder.Ascending);
			/// <summary>
			/// Gets the comparer used in the Sort and Add functions
			/// </summary>
			public ITreeListViewItemComparer Comparer
			{
				get{return(_comparer);}
				set{_comparer = value;}
			}
			#endregion
			#region SortOrderRecursively
			/// <summary>
			/// Set the new sortorder (apply automatically the sort function
			/// if the sortorder value is changed) for each collection recursively
			/// </summary>
			public SortOrder SortOrderRecursively
			{
				set
				{
					if(TreeListView != null)
						if(TreeListView.InvokeRequired)
						{
							TreeListView.Invoke(new VoidHandlerSortOrder(SetSortOrderRecursively), new object[] {value});
							return;
						}
					SetSortOrderRecursively(value);
				}
			}
			private void SetSortOrderRecursively(SortOrder value)
			{
				SortOrder = value;
				foreach(TreeListViewItem item in this)
					item.Items.SortOrderRecursively = value;
			}
			internal SortOrder SortOrderRecursivelyWithoutSort
			{
				set{if(TreeListView != null)
						if(TreeListView.InvokeRequired)
							throw(new Exception("Invoke Required"));
					Comparer.SortOrder = value;
					foreach(TreeListViewItem item in this)
						item.Items.SortOrderRecursivelyWithoutSort = value;}
			}
			#endregion
			#region Owner
			private TreeListView _owner;
			/// <summary>
			/// TreeListView control that directly contains this collection
			/// </summary>
			public TreeListView Owner{get{return(_owner);}}
			#endregion
			#region Parent
			private TreeListViewItem _parent;
			/// <summary>
			/// TreeListViewItem that contains this collection
			/// </summary>
			public TreeListViewItem Parent{get{return(_parent);}}
			#endregion
			#region TreeListView
			/// <summary>
			/// Returns the TreeListView set in Owner or in Parent
			/// </summary>
			private TreeListView TreeListView
			{
				get
				{
					if(Owner != null) return(Owner);
					if(Parent != null) return(Parent.ListView);
					return(null);
				}
			}
			#endregion
			#region this[]
			/// <summary>
			/// Get an item in the collection
			/// </summary>
			public virtual TreeListViewItem this[int index]
			{
				get
				{
					return((TreeListViewItem) this.List[index]);}
			}
			#endregion
		#endregion
		#region Events, Delegates, and internal calls
			#region Events
			/// <summary>
			/// Occurs when an item is added to the collection
			/// </summary>
			[Description("Occurs when an item is added to the collection.")]
			public TreeListViewEventHandler ItemAdded;
			/// <summary>
			/// Occurs when an item is removed from the collection
			/// </summary>
			[Description("Occurs when an item is removed from the collection.")]
			public TreeListViewEventHandler ItemRemoved;
			#endregion
			#region On???
			/// <summary>
			/// Raises the ItemAdded event
			/// </summary>
			/// <param name="e"></param>
			protected virtual void OnItemAdded(TreeListViewEventArgs e)
			{
				if(ItemAdded != null) ItemAdded(this, e);
			}
			/// <summary>
			/// Raises the ItemRemoved event
			/// </summary>
			/// <param name="e"></param>
			protected virtual void OnItemRemoved(TreeListViewEventArgs e)
			{
				if(ItemRemoved != null) ItemRemoved(this, e);
			}
			#endregion
		#endregion

		#region Constructors
		/// <summary>
		/// Create a collection in the root of a TreeListView (first level items)
		/// </summary>
		/// <param name="owner"></param>
		public TreeListViewItemCollection(TreeListView owner)
		{
			_owner = owner;
		}
		/// <summary>
		/// Create a collection within a TreeListViewItem
		/// </summary>
		/// <param name="parent"></param>
		public TreeListViewItemCollection(TreeListViewItem parent)
		{
			_parent = parent;
		}
		/// <summary>
		/// Create a free TreeListViewItemCollection (items will not be
		/// displayed in a TreeListView
		/// </summary>
		public TreeListViewItemCollection()
		{
		}
		#endregion

		#region Sort Functions
		/// <summary>
		/// Transforms the collection to an array
		/// </summary>
		public TreeListViewItem[] ToArray()
		{
			if(TreeListView != null)
				if(TreeListView.InvokeRequired)
					throw(new Exception("Invoke required"));
			int size = this.Count;
			TreeListViewItem[] eltsArray = new TreeListViewItem[size];
			for(int i = 0 ; i < size ; i++)
				eltsArray[i] = this[i];
			return(eltsArray);
		}
		/// <summary>
		/// Sort the items in this collection (recursively or not)
		/// </summary>
		/// <param name="recursively">Recursively</param>
		public void Sort(bool recursively)
		{
			if(TreeListView != null) TreeListView.BeginUpdate();
			SortInternal(recursively);
			if(TreeListView != null) TreeListView.EndUpdate();
		}
		internal void SortInternal(bool recursively)
		{
			if(TreeListView != null)
				if(TreeListView.InvokeRequired)
					throw(new Exception("Invoke required"));
			if(_sortable)
			{
				// Gets an array of the items
				TreeListViewItem[] thisarray = ToArray();
				// Removes the items
				ClearInternal();
				// Adds the items
				foreach(TreeListViewItem item in thisarray)
					Add(item);
			}
			if(recursively)
				foreach(TreeListViewItem item in this)
					item.Items.SortInternal(true);
		}
		#endregion
		#region Add Function
		/// <summary>
		/// Returns true if this collection contains an item
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual bool Contains(TreeListViewItem item)
		{
			if(TreeListView != null)
				if(TreeListView.InvokeRequired)
					throw(new Exception("Invoke required"));
			bool res = false;
			foreach(TreeListViewItem elt in this)
				if(item == elt)
				{
					res = true;
					break;
				}
			return(res);
		}
		private bool ListViewContains(TreeListViewItem item)
		{
			if(TreeListView == null) return(false);
			if(TreeListView.InvokeRequired)
				throw(new Exception("Invoke required"));
			ListView listview = (ListView) TreeListView;
			ListViewItem listviewitem = (ListViewItem) item;
			try{
				foreach(ListViewItem temp in listview.Items)
					if(temp == listviewitem) return(true);}
			catch{}
			return(false);
		}
		/// <summary>
		/// Adds an item in the collection and in the TreeListView
		/// </summary>
		/// <param name="item"></param>
		/// <returns>Index of the item in the collection</returns>
		public virtual int Add(TreeListViewItem item)
		{
			if(TreeListView != null)
				if(TreeListView.InvokeRequired)
					throw(new Exception("Invoke required"));
			// Do not add the item if the collection owns a TreeListView recursively
			// and the item already owns a TreeListView
			if(TreeListView != null && item.ListView != null)
				throw(new Exception("The Item is already in a TreeListView"));
			int index = GetInsertCollectionIndex(item);
			if(index == -1) return(-1);
			if(Parent != null) item.SetParent(Parent);
			item.Items.Comparer = this.Comparer;
			int treelistviewindex = GetInsertTreeListViewIndex(item, index);
			// Insert in the ListView
			if(treelistviewindex > -1)
			{
				ListView listview = (ListView) TreeListView;
				listview.Items.Insert(treelistviewindex, (ListViewItem) item);
				if(item.IsExpanded) item.Expand();
				item.SetIndentation();
			}
			// Insert in this collection
			if(index > -1) List.Insert(index, item);
			if(index > -1) OnItemAdded(new TreeListViewEventArgs(item, TreeListViewAction.Unknown));
			if(Count == 1 && TreeListView != null && Parent != null)
				if(Parent.Visible) Parent.Redraw();
			return(index);
		}
		/// <summary>
		/// Adds an item in the collection and in the TreeListView
		/// </summary>
		/// <param name="value"></param>
		/// <param name="imageindex"></param>
		/// <returns></returns>
		public virtual TreeListViewItem Add(string value, int imageindex)
		{
			TreeListViewItem item = new TreeListViewItem(value, imageindex);
			Add(item);
			return(item);
		}
		/// <summary>
		/// Adds an item in the collection and in the TreeListView
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public virtual TreeListViewItem Add(string value)
		{
			TreeListViewItem item = new TreeListViewItem(value);
			Add(item);
			return(item);
		}
		/// <summary>
		/// Adds a collection to this collection
		/// </summary>
		/// <param name="collection"></param>
		public void AddRange(TreeListViewItemCollection collection)
		{
			if(TreeListView != null) TreeListView.BeginUpdate();
			AddRangeInternal(collection);
			if(TreeListView != null) TreeListView.BeginUpdate();
		}
		internal void AddRangeInternal(TreeListViewItemCollection collection)
		{
			foreach(TreeListViewItem item in collection)
				Add(item);
		}
		#endregion
		#region Remove & Clear Functions
		/// <summary>
		/// Removes each node of the collection
		/// </summary>
		public new void Clear()
		{
			TreeListView treelistview = this.TreeListView;
			if(treelistview != null) treelistview.BeginUpdate();
			ClearInternal();
			if(treelistview != null) treelistview.EndUpdate();
		}
		internal void ClearInternal()
		{
			if(TreeListView != null)
				if(TreeListView.InvokeRequired)
					throw(new Exception("Invoke required"));
			while(this.Count > 0) this.RemoveAtInternal(0);
		}
		/// <summary>
		/// Remove an item from the collection and the TreeListView
		/// </summary>
		/// <param name="item"></param>
		public virtual void Remove(TreeListViewItem item)
		{
			TreeListView treelistview = this.TreeListView;
			if(treelistview != null) treelistview.BeginUpdate();
			RemoveInternal(item);
			if(treelistview != null) treelistview.EndUpdate();
		}
		internal void RemoveInternal(TreeListViewItem item)
		{
			if(TreeListView != null)
				if(TreeListView.InvokeRequired)
					throw(new Exception("Invoke required"));
			int index = GetIndexOf(item);
			if(index == -1) return;
			RemoveAtInternal(index);
		}
		/// <summary>
		/// Gets the index of an item in the collection
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int GetIndexOf(TreeListViewItem item)
		{
			if(TreeListView != null)
				if(TreeListView.InvokeRequired)
					throw(new Exception("Invoke required"));
			int index = -1;
			for(int i = 0 ; i < this.Count ; i++)
				if(this[i] == item) {index = i; break;}
			return(index);
		}
		/// <summary>
		/// Remove an item from the collection and the TreeListView
		/// </summary>
		/// <param name="index"></param>
		public new void RemoveAt(int index)
		{
			TreeListView treelistview = this.TreeListView;
			if(treelistview != null) treelistview.BeginUpdate();
			RemoveAtInternal(index);
			if(treelistview != null) treelistview.EndUpdate();
		}
		internal void RemoveAtInternal(int index)
		{
			if(TreeListView != null)
				if(TreeListView.InvokeRequired)
					throw(new Exception("Invoke required"));
			TreeListViewItem item = this[index];
			if(this[index].Visible && this.TreeListView != null) item.Hide();
			List.RemoveAt(index);
			item.SetParent(null);
			// Redraw parent if no more children
			if(Count == 0 && TreeListView != null && Parent != null)
				Parent.Redraw();
			// Redraw new last item
			if(Count > 0 && TreeListView != null && index == Count)
				this[index-1].Redraw();
			OnItemRemoved(new TreeListViewEventArgs(item, TreeListViewAction.Unknown));
		}
		#endregion
		#region Internal Functions
		private int GetInsertTreeListViewIndex(TreeListViewItem item, int collectionindex)
		{
			if(TreeListView == null) return(-1);
			if(TreeListView.InvokeRequired)
				throw(new Exception("Invoke required"));
			if(Owner != null)
				{
					int a = 0;
					a++;
				}
			int index = -1;
			// First level item (no parent)
			if(Owner != null && collectionindex != -1)
			{
				if(collectionindex == 0) index = 0;
				else index =
						 this[collectionindex - 1].LastChildIndexInListView + 1;
			}
			else if(Parent != null && collectionindex != -1)
			{
				if(!Parent.Visible || !Parent.IsExpanded) index = -1;
				else
				{
					if(collectionindex == 0) index = Parent.Index + 1;
					else index =
							 Parent.Items[collectionindex - 1].LastChildIndexInListView + 1;
				}
			}
			return(index);
		}
		private int GetInsertCollectionIndex(TreeListViewItem item)
		{
			if(TreeListView != null)
				if(TreeListView.InvokeRequired)
					throw(new Exception("Invoke required"));
			int index = -1;
			if(!_sortable) index = Count;
			else if(!Contains(item) && !ListViewContains(item))
			{
				switch(SortOrder)
				{
						// No sortorder -> at the end of the collection
					case System.Windows.Forms.SortOrder.None:
						index = this.Count;
						break;
					default:
						for(int i = 0 ; i < this.Count ; i++)
						{
							// Change the index for the compare if the order is descending
							int indexcompare = i;
							int comp = Comparer.Compare(item, this[indexcompare]);
							if(comp <= 0)
							{
								index = indexcompare;
								break;
							}
						}
						index = index == -1 ? this.Count : index;
						break;
				}
			}
			return(index);
		}
		#endregion
	}
}