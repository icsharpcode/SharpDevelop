using System;

namespace System.Windows.Forms
{
	/// <summary>
	/// Collection of selected items in a TreeListView
	/// </summary>
	public class SelectedTreeListViewItemCollection : ListView.SelectedListViewItemCollection
	{
		#region Properties
		/// <summary>
		/// Gets a TreeListViewItem at the specified index
		/// </summary>
		new public TreeListViewItem this[int index]
		{
			get{return((TreeListViewItem) base[index]);}
		}
		#endregion
		#region Constructor
		/// <summary>
		/// Create a new instance of a SelectedTreeListViewItemCollection
		/// </summary>
		/// <param name="TreeListView"></param>
		public SelectedTreeListViewItemCollection(TreeListView TreeListView) : base((ListView) TreeListView)
		{
		}
		#endregion
		#region Functions
		/// <summary>
		/// Returns true if the specified item is in the collection
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(TreeListViewItem item)
		{
			return(base.Contains((ListViewItem) item));
		}
		/// <summary>
		/// Index of an item
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int IndexOf(TreeListViewItem item)
		{
			return(base.IndexOf((ListViewItem) item));
		}
		#endregion
	}
}
