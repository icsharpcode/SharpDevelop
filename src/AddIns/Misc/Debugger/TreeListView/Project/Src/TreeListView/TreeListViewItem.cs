using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.InteropServices.APIs;
using System.Diagnostics;

namespace System.Windows.Forms
{
	#region TreeListViewItemBoundsPortion
	/// <summary>
	/// Specifies a portion of the tree list view item from which to retrieve the bounding rectangle
	/// </summary>
	[Serializable]
	public enum TreeListViewItemBoundsPortion
	{
		/// <summary>
		/// The bounding rectangle of the entire item, including the icon, the item text, and the subitem text (if displayed), should be retrieved
		/// </summary>
		Entire = (int)ItemBoundsPortion.Entire,
		/// <summary>
		/// The bounding rectangle of the icon or small icon should be retrieved
		/// </summary>
		Icon = (int)ItemBoundsPortion.Icon,
		/// <summary>
		/// The bounding rectangle specified by the Entire value without the subitems
		/// </summary>
		ItemOnly = (int)ItemBoundsPortion.ItemOnly,
		/// <summary>
		/// The bounding rectangle of the item text should be retrieved
		/// </summary>
		Label = (int)ItemBoundsPortion.Label,
		/// <summary>
		/// The bounding rectangle of the item plus minus
		/// </summary>
		PlusMinus = 4
	}
	#endregion
	/// <summary>
	/// Represents an item in a TreeListView control
	/// </summary>
	public class TreeListViewItem : ListViewItem
	{
		#region Private delegates
		private delegate void ChangeChildrenCheckStateRecursivelyHandler(CheckState state);
		private delegate TreeListViewItemCollection GetCollectionHandler();
		private delegate string GetStringHandler();
		private delegate bool GetBoolHandler();
		private delegate int GetIntHandler();
		private delegate TreeListViewItem GetTreeListViewItemHandler();
		#endregion

		#region Events
		/// <summary>
		/// TreeListViewItemHandler delegate
		/// </summary>
		public delegate void TreeListViewItemHanlder(object sender);
		/// <summary>
		/// TreeListViewItemCheckedHandler delegate
		/// </summary>
		public delegate void TreeListViewItemCheckedHandler(object sender, bool ischecked);
		/// <summary>
		/// Occurs after the tree node is collapsed
		/// </summary>
		public event TreeListViewItemHanlder AfterCollapse;
		/// <summary>
		/// Occurs after the tree node is expanded
		/// </summary>
		public event TreeListViewItemHanlder AfterExpand;
		#endregion
		#region Properties
			#region NextVisibleItem
			/// <summary>
			/// Gets the next visible item in the TreeListView
			/// </summary>
			public TreeListViewItem NextVisibleItem
			{
				get
				{
					if(!IsInATreeListView || !Visible) return null;
					ListView listview = (ListView) TreeListView;
					if(Index >= listview.Items.Count-1) return null;
					return (TreeListViewItem)listview.Items[Index+1];
				}
			}
			#endregion
			#region PrevVisibleItem
			/// <summary>
			/// Gets the previous visible item in the TreeListView
			/// </summary>
			public TreeListViewItem PrevVisibleItem
			{
				get
				{
					if(!IsInATreeListView || !Visible) return null;
					ListView listview = (ListView) TreeListView;
					if(Index < 1) return null;
					return (TreeListViewItem)listview.Items[Index-1];
				}
			}
			#endregion

			#region Checked properties
			/// <summary>
			/// Gets or sets a value indicating whether the item is checked.
			/// </summary>
			public new bool Checked 
			{
				get
				{
					try
					{
						return (base.Checked);
					}
					catch
					{
						return false;
					}
				}
				set 
				{
					if(IsInATreeListView)
						if(TreeListView.InvokeRequired)
							throw(new Exception("Invoke required"));
					try
					{
						// Check downwards recursively
						if(ListView != null &&
							ListView._checkDirection == CheckDirection.Downwards &&
							_items.Count > 0)
						{
							foreach(TreeListViewItem childItem in _items)
								childItem.Checked = value;
						}
						if(base.Checked == value) return;
						base.Checked = value;
					}
					catch{} 
				}
			}
			#endregion
			#region CheckStatus
			/// <summary>
			/// Gets the check state of this item
			/// </summary>
			public CheckState CheckStatus
			{
				get
				{
					if(_items.Count <= 0)
					{
						if(this.Checked)
							return CheckState.Checked;
						else
							return CheckState.Unchecked;
					}
					else
					{
						bool allChecked = true; 
						bool allUnChecked = true; 

						TreeListViewItem[] items = Items.ToArray(); 
						foreach(TreeListViewItem item in items) 
						{ 
							if (item.CheckStatus == CheckState.Indeterminate) 
								return CheckState.Indeterminate; 
							else if (item.CheckStatus == CheckState.Checked) 
								allUnChecked = false; 
							else 
								allChecked = false; 
						} 

						Debug.Assert(!(allChecked && allUnChecked)); 
						if (allChecked) 
							return CheckState.Checked; 
						else if (allUnChecked) 
							return CheckState.Unchecked; 
						else 
							return CheckState.Indeterminate; 
					}
				}
			}
			#endregion

			#region ParentsInHierarch
			/// <summary>
			/// Gets a collection of the parent of this item
			/// </summary>
			[Browsable(false)]
			public TreeListViewItem[] ParentsInHierarch
			{
				get
				{
					TreeListViewItemCollection items = GetParentsInHierarch();
					return(items.ToArray());
				}
			}
			private TreeListViewItemCollection GetParentsInHierarch()
			{
				TreeListViewItemCollection temp = Parent != null ?
					Parent.GetParentsInHierarch() : new TreeListViewItemCollection();
				if(Parent != null) temp.Add(Parent);
				return temp;
			}
			#endregion
			#region FullPath
			/// <summary>
			/// Gets the fullpath of an item (Parents.Text + \ + this.Text)
			/// </summary>
			[Browsable(false)]
			public string FullPath
			{
				get
				{
					if(Parent != null)
					{
						string pathSeparator = IsInATreeListView ? TreeListView.PathSeparator : "\\";
						string strPath = Parent.FullPath + pathSeparator + Text;
						return(strPath.Replace(pathSeparator + pathSeparator, pathSeparator));
					}
					else
						return(Text);
				}
			}
			#endregion
			#region Text
			/// <summary>
			/// Get or Set the Text property
			/// </summary>
			new public string Text
			{
				get
				{
					return(base.Text);
				}
				set
				{
					base.Text = value;
					TreeListViewItemCollection collection = Container;
					if(collection != null) collection.Sort(false);}
			}
			#endregion
			#region Container
			/// <summary>
			/// Get the collection that contains this item
			/// </summary>
			public TreeListViewItemCollection Container
			{
				get
				{
					if(Parent != null) return(Parent.Items);
					if(IsInATreeListView) return(TreeListView.Items);
					return(null);
				}
			}
			#endregion
			#region IsInATreeListView
			internal bool IsInATreeListView
			{
				get
				{
					return(TreeListView != null);
				}
			}
			#endregion
			#region LastChildIndexInListView
			/// <summary>
			/// Get the biggest index in the listview of the visible childs of this item
			/// including this item
			/// </summary>
			[Browsable(false)]
			public int LastChildIndexInListView
			{
				get
				{
					if(!IsInATreeListView)
						throw(new Exception("No ListView control"));
					int index = this.Index, temp;
					foreach(TreeListViewItem item in Items)
						if(item.Visible)
						{
							temp = item.LastChildIndexInListView;
							if(temp > index) index = temp;
						}
					return(index);
				}
			}
			#endregion
			#region ChildrenCount
			/// <summary>
			/// Get the children count recursively
			/// </summary>
			[Browsable(false)]
			public int ChildrenCount
			{
				get
				{
					TreeListViewItem[] items = _items.ToArray();
					int count = items.Length;
					foreach(TreeListViewItem item in items) count += item.ChildrenCount;
					return(count);
				}
			}
			#endregion
			#region IsExpanded
			private bool _isexpanded;
			/// <summary>
			/// Returns true if this item is expanded
			/// </summary>
			public bool IsExpanded
			{
				get
				{
					return(_isexpanded);
				}
				set
				{
					if(_isexpanded == value) return;
					if(value) Expand();
					else Collapse();
				}
			}
			#endregion
			#region Level
			/// <summary>
			/// Get the level of the item in the treelistview
			/// </summary>
			[Browsable(false)]
			public int Level
			{
				get
				{
					return(Parent == null ? 0 : Parent.Level + 1);
				}
			}
			#endregion
			#region Items
			private TreeListViewItemCollection _items;
			/// <summary>
			/// Get the items contained in this item
			/// </summary>
			public TreeListViewItemCollection Items
			{
				get
				{
					return(_items);
				}
			}
			#endregion
			#region Parent
			private TreeListViewItem _parent;
			/// <summary>
			/// Get the parent of this item
			/// </summary>
			public TreeListViewItem Parent
			{
				get
				{
					return(_parent);
				}
			}
			#endregion
			#region TreeListView
			/// <summary>
			/// Gets the TreeListView containing this item
			/// </summary>
			public new TreeListView ListView
			{
				get
				{
					if(base.ListView != null) return((TreeListView) base.ListView);
					if(Parent != null) return(Parent.ListView);
					return(null);
				}
			}
			/// <summary>
			/// Gets the TreeListView containing this item
			/// </summary>
			public TreeListView TreeListView
			{
				get
				{
					return (TreeListView) ListView;
				}
			}
			#endregion
			#region Visible
			/// <summary>
			/// Returns true if this item is visible in the TreeListView
			/// </summary>
			public bool Visible
			{
				get
				{
					return(base.Index > -1);
				}
			}
			#endregion
		#endregion

		#region Constructors
		/// <summary>
		/// Create a new instance of a TreeListViewItem
		/// </summary>
		public TreeListViewItem()
		{
			_items = new TreeListViewItemCollection(this);
		}
		/// <summary>
		/// Create a new instance of a TreeListViewItem
		/// </summary>
		public TreeListViewItem(string value) : this()
		{
			this.Text = value;
		}
		/// <summary>
		/// Create a new instance of a TreeListViewItem
		/// </summary>
		public TreeListViewItem(string value, int imageindex) : this(value)
		{
			this.ImageIndex = imageindex;
		}
		#endregion
		
		#region Functions
		internal void GetCheckedItems(ref TreeListViewItemCollection items)
		{
			if(Checked) items.Add(this);
			foreach(TreeListViewItem item in Items)
				item.GetCheckedItems(ref items);
		}
		/// <summary>
		/// Places the subitem into edit mode
		/// </summary>
		/// <param name="column">Number of the subitem to edit</param>
		public void BeginEdit(int column)
		{
			if(TreeListView == null)
				throw(new Exception("The item is not associated with a TreeListView"));
			if(!TreeListView.Visible)
				throw(new Exception("The item is not visible"));
			if(column + 1 > TreeListView.Columns.Count)
				throw(new Exception("The column is greater the number of columns in the TreeListView"));
			TreeListView.Focus();
			Focused = true;
			TreeListView._lastitemclicked = new EditItemInformations(this, column, this.SubItems[column].Text);
			base.BeginEdit();
		}
		/// <summary>
		/// Places the item into edit mode
		/// </summary>
		new public void BeginEdit()
		{
			BeginEdit(0);
		}
		/// <summary>
		/// Asks the associated TreeListView control to redraw this item
		/// </summary>
		public void Redraw()
		{
			if(ListView == null || !Visible) return;
			try
			{
				APIsUser32.SendMessage(
					ListView.Handle,
					(int)APIsEnums.ListViewMessages.REDRAWITEMS,
					Index, Index);}
			catch{}
		}
		/// <summary>
		/// Retrieves the specified portion of the bounding rectangle for the item
		/// </summary>
		/// <param name="portion">One of the TreeListViewItemBoundsPortion values that represents a portion of the item for which to retrieve the bounding rectangle</param>
		/// <returns>A Rectangle that represents the bounding rectangle for the specified portion of the item</returns>
		public Rectangle GetBounds(TreeListViewItemBoundsPortion portion)
		{
			switch((int)portion)
			{
				case (int) TreeListViewItemBoundsPortion.PlusMinus:
					if(TreeListView == null)
						throw(new Exception("This item is not associated with a TreeListView control"));
					Point pos = base.GetBounds(ItemBoundsPortion.Entire).Location;
					Point position = new Point(
						Level*SystemInformation.SmallIconSize.Width + 1 + pos.X,
						TreeListView.GetItemRect(Index, ItemBoundsPortion.Entire).Top + 1);
					return new Rectangle(position, TreeListView.ShowPlusMinus ? SystemInformation.SmallIconSize : new Size(0, 0));
				default:
					ItemBoundsPortion lviPortion = (ItemBoundsPortion)(int) portion;
					return base.GetBounds(lviPortion);
			}
		}
		internal void SetParent(TreeListViewItem parent)
		{
			_parent = parent;
		}
		/// <summary>
		/// Remove this item from its associated collection
		/// </summary>
		public new void Remove()
		{
			if(ListView != null)
				if(ListView.InvokeRequired)
					throw(new Exception("Invoke required"));
			TreeListViewItemCollection collection = this.Container;
			if(collection != null) collection.Remove(this);
		}
		/// <summary>
		/// Check if this node is one of the parents of an item (recursively)
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool IsAParentOf(TreeListViewItem item)
		{
			TreeListViewItem[] parents = item.ParentsInHierarch;
			foreach(TreeListViewItem parent in parents)
				if(parent == this) return(true);
			return(false);
		}
		/// <summary>
		/// Ensure that the node is visible (expand parents and scroll listview so that the item is visible)
		/// </summary>
		new public void EnsureVisible()
		{
			if(!Visible)
			{
				if(IsInATreeListView)
					TreeListView.BeginUpdate();
				if(ListView != null)
					ListView.Invoke(new MethodInvoker(ExpandParents));
				else ExpandParents();
				if(TreeListView != null)
					TreeListView.EndUpdate();
			}
			base.EnsureVisible();
		}
		internal void ExpandParents()
		{
			if(IsInATreeListView)
				Debug.Assert(!ListView.InvokeRequired);
			if(Parent != null)
			{
				if(!Parent.IsExpanded) Parent.ExpandInternal();
				Parent.ExpandParents();
			}
		}
		#endregion
		#region Indentation
		/// <summary>
		/// Set the indentation using the level of this item
		/// </summary>
		/// <returns>True if successfull, false otherwise</returns>
		public bool SetIndentation()
		{
			if(!IsInATreeListView) return false;
			bool res = true;
			APIsStructs.LV_ITEM lvi = new APIsStructs.LV_ITEM();
			lvi.iItem = Index;
			lvi.iIndent = Level;
			if(TreeListView.ShowPlusMinus) lvi.iIndent++;
			lvi.mask = APIsEnums.ListViewItemFlags.INDENT;
			try
			{
				APIsUser32.SendMessage(
					ListView.Handle,
					APIsEnums.ListViewMessages.SETITEM,
					0,
					ref lvi);
			}
			catch
			{
				res = false;
			}
			return res;
		}
		/// <summary>
		/// Refresh indentation of this item and of its children (recursively)
		/// </summary>
		/// <param name="recursively">Recursively</param>
		public void RefreshIndentation(bool recursively)
		{
			if(ListView == null) return;
			if(ListView.InvokeRequired)
				throw(new Exception("Invoke Required"));
			if(!this.Visible) return;
			SetIndentation();
			if(recursively)
			{
				try
				{
					foreach(TreeListViewItem item in this.Items)
						item.RefreshIndentation(true);
				}
				catch{}
			}
		}
		#endregion
		#region Expand
		/// <summary>
		/// Expand
		/// </summary>
		public void Expand()
		{
			if(IsInATreeListView)
				if (ListView.InvokeRequired)
					throw(new Exception("Invoke Required"));
			if(TreeListView != null) TreeListView.BeginUpdate();
			ExpandInternal();
			if(TreeListView != null) TreeListView.EndUpdate();
		}
		internal void ExpandInternal()
		{
			if(IsInATreeListView)
				if (ListView.InvokeRequired)
					throw(new Exception("Invoke Required"));

			TreeListViewItem selItem = null;
			if(TreeListView != null) selItem = TreeListView.FocusedItem;

			// Must set ListView.checkDirection to CheckDirection.None.
			// Forbid recursively checking.
			CheckDirection oldDirection = CheckDirection.All;
			if(ListView != null)
			{
				oldDirection = ListView._checkDirection;
				ListView._checkDirection = CheckDirection.None;
			}

			// The item wasn't expanded -> raise an event
			if(Visible && !_isexpanded && ListView != null)
			{
				TreeListViewCancelEventArgs e = new TreeListViewCancelEventArgs(
					this, TreeListViewAction.Expand);
				ListView.RaiseBeforeExpand(e);
				if(e.Cancel) return;
			}

			if(Visible)
				for(int i = Items.Count - 1 ; i >= 0 ;i--)
				{
					TreeListViewItem item = this.Items[i];
					if(!item.Visible)
					{
						ListView LView = this.ListView;
						LView.Items.Insert(
							this.Index + 1, item);
						item.SetIndentation();
					}
					if(item.IsExpanded) item.Expand(); 
				} 
			// The item wasn't expanded -> raise an event
			if(Visible && !_isexpanded && IsInATreeListView)
			{
				this._isexpanded = true;
				TreeListViewEventArgs e = new TreeListViewEventArgs(
					this, TreeListViewAction.Expand);
				ListView.RaiseAfterExpand(e);
				if (AfterExpand != null) AfterExpand(this);
			}
			this._isexpanded = true;

			// Reset ListView.checkDirection
			if(IsInATreeListView)
				ListView._checkDirection = oldDirection;
			if(TreeListView != null && selItem != null)
				if(selItem.Visible)
					selItem.Focused = true;
		}
		/// <summary>
		/// Expand all sub nodes
		/// </summary>
		public void ExpandAll()
		{
			if(IsInATreeListView)
				if(ListView.InvokeRequired)
					throw(new Exception("Invoke Required"));
			if(TreeListView != null) TreeListView.BeginUpdate();
			ExpandAllInternal();
			if(TreeListView != null) TreeListView.EndUpdate();
		}
		internal void ExpandAllInternal()
		{
			if(IsInATreeListView)
				if(ListView.InvokeRequired)
					throw(new Exception("Invoke Required"));
			ExpandInternal();
			// Expand canceled -> stop expandall for the children of this item
			if(!IsExpanded) return;
			for(int i = 0 ; i < Items.Count ; i++)
				Items[i].ExpandAllInternal();
		}
		#endregion
		#region Collapse
		/// <summary>
		/// Collapse
		/// </summary>
		public void Collapse()
		{
			if(IsInATreeListView)
				if(ListView.InvokeRequired)
					throw(new Exception("Invoke Required"));
			if(TreeListView != null) TreeListView.BeginUpdate();
			CollapseInternal();
			if(TreeListView != null) TreeListView.EndUpdate();
		}
		internal void CollapseInternal()
		{
			if(IsInATreeListView)
				if(ListView.InvokeRequired)
					throw(new Exception("Invoke Required"));
			TreeListViewItem selItem = null;
			if(TreeListView != null) selItem = TreeListView.FocusedItem;
			// The item was expanded -> raise an event
			if(Visible && _isexpanded && ListView != null)
			{
				TreeListViewCancelEventArgs e = new TreeListViewCancelEventArgs(
					this, TreeListViewAction.Collapse);
				ListView.RaiseBeforeCollapse(e);
				if(e.Cancel) return;
			}

			// Collapse
			if(this.Visible)
				foreach(TreeListViewItem item in Items)
						item.Hide();
			
			// The item was expanded -> raise an event
			if(Visible && _isexpanded && IsInATreeListView)
			{
				this._isexpanded = false;
				TreeListViewEventArgs e = new TreeListViewEventArgs(
					this, TreeListViewAction.Collapse);
				ListView.RaiseAfterCollapse(e);
				if(AfterCollapse != null) AfterCollapse(this);
			}
			this._isexpanded = false;
			if(IsInATreeListView && selItem != null)
			{
				if(selItem.Visible)
					selItem.Focused = true;
				else
				{
					ListView listview = (ListView) TreeListView;
					listview.SelectedItems.Clear();
					TreeListViewItem[] items = selItem.ParentsInHierarch;
					for(int i = items.Length - 1; i >= 0; i--)
						if(items[i].Visible)
						{
							items[i].Focused = true;
							break;
						}
				}
			}
		}
		/// <summary>
		/// Collapse all sub nodes
		/// </summary>
		public void CollapseAll()
		{
			if(IsInATreeListView)
				if(ListView.InvokeRequired)
					throw(new Exception("Invoke Required"));
			if(TreeListView != null) TreeListView.BeginUpdate();
			CollapseAllInternal();
			if(TreeListView != null) TreeListView.EndUpdate();
		}
		internal void CollapseAllInternal()
		{
			if(IsInATreeListView)
				if(ListView.InvokeRequired)
					throw(new Exception("Invoke Required"));
			foreach(TreeListViewItem item in this.Items)
				item.CollapseAllInternal();
			CollapseInternal();
		}
		/// <summary>
		/// Hide this node (remove from TreeListView but
		/// not from associated Parent items)
		/// </summary>
		internal void Hide()
		{
			if(IsInATreeListView)
				if(ListView.InvokeRequired)
					throw(new Exception("Invoke Required"));
			foreach(TreeListViewItem item in Items) item.Hide();
			if(Visible) base.Remove();
			Selected = false;
		}
		#endregion

		#region DrawPlusMinus
		internal void DrawPlusMinus()
		{
			if(!IsInATreeListView) return;
			if(TreeListView._updating) return;
			Graphics g = Graphics.FromHwnd(TreeListView.Handle);
			DrawPlusMinus(g);
			g.Dispose();
		}
		internal void DrawPlusMinus(Graphics g)
		{
			if(!IsInATreeListView) return;
			if(TreeListView._updating) return;
			Debug.Assert(!TreeListView.InvokeRequired);
			if(Items.Count == 0 || TreeListView.Columns.Count == 0) return;
			Drawing.Imaging.ImageAttributes attr = new Drawing.Imaging.ImageAttributes();
			attr.SetColorKey(Color.Transparent, Color.Transparent);
			if(TreeListView.Columns[0].Width > (Level + 1) * SystemInformation.SmallIconSize.Width)
				g.DrawImage(
					TreeListView.plusMinusImageList.Images[IsExpanded ? 1 : 0],
					GetBounds(TreeListViewItemBoundsPortion.PlusMinus),
					0, 0, SystemInformation.SmallIconSize.Width, SystemInformation.SmallIconSize.Height,
					GraphicsUnit.Pixel, attr);
			attr.Dispose();
		}
		#endregion
		#region DrawPlusMinusLines
		internal void DrawPlusMinusLines()
		{
			if(!IsInATreeListView) return;
			if(TreeListView._updating) return;
			Graphics g = Graphics.FromHwnd(TreeListView.Handle);
			DrawPlusMinusLines(g);
			g.Dispose();
		}
		internal void DrawPlusMinusLines(Graphics g)
		{
			if(!IsInATreeListView) return;
			if(TreeListView._updating) return;
			Debug.Assert(!TreeListView.InvokeRequired);
			if(!TreeListView.ShowPlusMinus || TreeListView.Columns.Count == 0) return;
			int itemLevel = Level;
			Rectangle plusminusRect = GetBounds(TreeListViewItemBoundsPortion.PlusMinus);
			Rectangle entireRect = GetBounds(TreeListViewItemBoundsPortion.Entire);
			Drawing.Drawing2D.HatchBrush hb = new Drawing.Drawing2D.HatchBrush(Drawing.Drawing2D.HatchStyle.Percent50, TreeListView.PlusMinusLineColor, BackColor);
			Pen pen = new Pen(hb);
			Point point1, point2;
			#region Vertical line
			point1 = new Point(
				plusminusRect.Right - SystemInformation.SmallIconSize.Width / 2 - 1,
				entireRect.Top);
			point2 = new Point( point1.X, entireRect.Bottom);
			// If ListView has no items that have the same level before this item
			if(!HasLevelBeforeItem(itemLevel)) point1.Y += SystemInformation.SmallIconSize.Height / 2;
			// If ListView has no items that have the same level after this item
			if(!HasLevelAfterItem(itemLevel)) point2.Y -= SystemInformation.SmallIconSize.Height / 2 + 1;
			if(TreeListView.Columns[0].Width > (Level + 1) * SystemInformation.SmallIconSize.Width)
				g.DrawLine(pen, point1, point2);
			#endregion
			#region Horizontal line
			point1 = new Point(
				plusminusRect.Right - SystemInformation.SmallIconSize.Width / 2 - 1,
				GetBounds(TreeListViewItemBoundsPortion.Entire).Top + SystemInformation.SmallIconSize.Height /2);
			point2 = new Point(plusminusRect.Right + 1, point1.Y);
			if(TreeListView.Columns[0].Width > (Level + 1) * SystemInformation.SmallIconSize.Width)
				g.DrawLine(pen, point1, point2);
			#endregion
			#region Lower Level lines
			for(int level = Level - 1; level > -1; level--)
				if(HasLevelAfterItem(level))
				{
					point1 = new Point(
						SystemInformation.SmallIconSize.Width * (2*level + 1) / 2 + entireRect.X,
						entireRect.Top);
					point2 = new Point(
						point1.X, entireRect.Bottom);
					if(TreeListView.Columns[0].Width > (level + 1) * SystemInformation.SmallIconSize.Width)
						g.DrawLine(pen, point1, point2);
				}
			#endregion
			pen.Dispose();
			hb.Dispose();
		}
		internal bool HasLevelAfterItem(int level)
		{
			if(TreeListView == null) return false;
			Debug.Assert(!TreeListView.InvokeRequired);
			int lev = Level, tempLevel;
			ListView listview = (ListView) TreeListView;
			for(int i = Index + 1; i < listview.Items.Count; i++)
			{
				tempLevel = ((TreeListViewItem)listview.Items[i]).Level;
				if(tempLevel == level) return true;
				if(tempLevel < level) return false;
			}
			return false;
		}
		internal bool HasLevelBeforeItem(int level)
		{
			if(TreeListView == null) return false;
			Debug.Assert(!TreeListView.InvokeRequired);
			int lev = Level, tempLevel;
			ListView listview = (ListView) TreeListView;
			for(int i = Index - 1; i > -1; i--)
			{
				tempLevel = ((TreeListViewItem)listview.Items[i]).Level;
				if(tempLevel <= level) return true;
			}
			return false;
		}
		#endregion
		#region DrawFocusCues
		internal void DrawFocusCues()
		{
			if(!IsInATreeListView) return;
			if(TreeListView._updating) return;
			if(TreeListView.HideSelection && !TreeListView.Focused) return;
			Graphics g = Graphics.FromHwnd(TreeListView.Handle);
			if(Visible)
			{
				Rectangle entireitemrect = GetBounds(ItemBoundsPortion.Entire);
				if(entireitemrect.Bottom > entireitemrect.Height * 1.5f)
				{
					Rectangle labelitemrect = GetBounds(ItemBoundsPortion.Label);
					Rectangle itemonlyrect = GetBounds(ItemBoundsPortion.ItemOnly);
					Rectangle selecteditemrect = new Rectangle(
						labelitemrect.Left,
						labelitemrect.Top,
						TreeListView.FullRowSelect ? entireitemrect.Width - labelitemrect.Left - 1 : itemonlyrect.Width - SystemInformation.SmallIconSize.Width - 1,
						labelitemrect.Height - 1);
					Pen pen = new Pen(TreeListView.Focused && Selected ? Color.Blue : ColorUtil.CalculateColor(SystemColors.Highlight, SystemColors.Window, 130));
					for(int i = 1; i < TreeListView.Columns.Count; i++)
					{
						Rectangle rect = TreeListView.GetSubItemRect(Index, i);
						if(rect.X < selecteditemrect.X)
							selecteditemrect = new Rectangle(
								rect.X, selecteditemrect.Y,
								selecteditemrect.Width + (selecteditemrect.X-rect.X),
								selecteditemrect.Height);
					}
					g.DrawRectangle(new Pen(ColorUtil.VSNetSelectionColor), selecteditemrect);
					// Fill the item (in CommCtl V6, the selection area is not always the same :
					// label only or first column). I decided to always draw the entire column...
					if(!TreeListView.FullRowSelect)
						g.FillRectangle(
							new SolidBrush(BackColor),
							itemonlyrect.Right-1, itemonlyrect.Top,
							labelitemrect.Width - itemonlyrect.Width + SystemInformation.SmallIconSize.Width + 1, selecteditemrect.Height + 1);
					bool draw = true;
					if(PrevVisibleItem != null)
						if(PrevVisibleItem.Selected) draw = false;
					// Draw upper line if previous item is not selected
					if(draw) g.DrawLine(pen, selecteditemrect.Left, selecteditemrect.Top, selecteditemrect.Right, selecteditemrect.Top);
					g.DrawLine(pen, selecteditemrect.Left, selecteditemrect.Top, selecteditemrect.Left, selecteditemrect.Bottom);
					draw = true;
					if(NextVisibleItem != null)
						if(NextVisibleItem.Selected) draw = false;
					// Draw lower line if net item is not selected
					if(draw) g.DrawLine(pen, selecteditemrect.Left, selecteditemrect.Bottom, selecteditemrect.Right, selecteditemrect.Bottom);
					g.DrawLine(pen, selecteditemrect.Right, selecteditemrect.Top, selecteditemrect.Right, selecteditemrect.Bottom);
					// If FullRowSelect is false and multiselect is enabled, the items don't have the same width
					if(!TreeListView.FullRowSelect && NextVisibleItem != null)
						if(NextVisibleItem.Selected)
						{
							int nextItemWidth = NextVisibleItem.GetBounds(TreeListViewItemBoundsPortion.ItemOnly).Width;
							if(nextItemWidth != itemonlyrect.Width)
							{
								g.DrawLine(
									pen,
									selecteditemrect.Right,
									selecteditemrect.Bottom,
									selecteditemrect.Right - (itemonlyrect.Width-nextItemWidth),
									selecteditemrect.Bottom);
							}
						}
					pen.Dispose();
				}
			}
			g.Dispose();
		}
		#endregion
		#region DrawIntermediateState
		internal void DrawIntermediateState()
		{
			if(!IsInATreeListView) return;
			if(TreeListView._updating) return;
			Graphics g = Graphics.FromHwnd(TreeListView.Handle);
			DrawIntermediateState(g);
			g.Dispose();
		}
		internal void DrawIntermediateState(Graphics g)
		{
			if(!IsInATreeListView) return;
			if(TreeListView._updating) return;
			Debug.Assert(!TreeListView.InvokeRequired);
			if(TreeListView.CheckBoxes != CheckBoxesTypes.Recursive || TreeListView.Columns.Count == 0) return;
			if(CheckStatus == CheckState.Indeterminate)
			{
				Rectangle rect = GetBounds(ItemBoundsPortion.Icon);
				Rectangle r = TreeListView._comctl32Version >= 6 ?
					new Rectangle(rect.Left - 14, rect.Top + 5, rect.Height-10, rect.Height-10) :
					new Rectangle(rect.Left - 11, rect.Top + 5, rect.Height-10, rect.Height-10);
				Brush brush = new Drawing.Drawing2D.LinearGradientBrush(r, Color.Gray, Color.LightBlue, 45, false);
				if(TreeListView.Columns[0].Width > (Level + (TreeListView.ShowPlusMinus?2:1)) * SystemInformation.SmallIconSize.Width)
					g.FillRectangle(brush, r);
				brush.Dispose();
			}
		}
		#endregion
	}
}