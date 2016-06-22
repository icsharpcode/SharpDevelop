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
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Widgets.Resources;

namespace ICSharpCode.SharpDevelop.Widgets.ListViewSorting
{
	/// <summary>
	/// Sorts items in a ListView.
	/// The user can select the column to sort by clicking on the column header.
	/// </summary>
	public sealed class ListViewItemSorter : IComparer, IDisposable
	{
		readonly ListView listView;
		readonly IListViewItemComparer[] itemComparers;
		
		int sortColumnIndex;
		SortOrder sortOrder = SortOrder.None;
		
		#region Properties
		
		/// <summary>
		/// Gets the ListView this ListViewItemSorter is associated with.
		/// </summary>
		public ListView ListView {
			get { return listView; }
		}
		
		/// <summary>
		/// Gets or sets the index of the list view column that is currently sorted.
		/// Assigning a value to this property causes the ListView to be re-sorted.
		/// </summary>
		public int SortColumnIndex {
			get { return sortColumnIndex; }
			set {
				sortColumnIndex = value;
				this.SetColumnHeaderIcons();
				this.ListView.Sort();
			}
		}
		
		/// <summary>
		/// Gets or sets the current sort order.
		/// Assigning a value to this property causes the ListView to be re-sorted.
		/// </summary>
		public SortOrder SortOrder {
			get { return sortOrder; }
			set {
				sortOrder = value;
				this.SetColumnHeaderIcons();
				this.ListView.Sort();
			}
		}
		
		#endregion
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewItemSorter"/> class
		/// that can sort all columns by their text content
		/// and attaches the new instance to the specified ListView.
		/// </summary>
		/// <param name="listView">The ListView that should be sorted and monitored for column click events.</param>
		/// <remarks>
		/// Note that the ListViewItemSorter constructor adds two bitmaps to the
		/// SmallImageList of the ListView (an empty ImageList will be created
		/// first if none is assigned).
		/// </remarks>
		public ListViewItemSorter(ListView listView)
			: this(listView, null)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewItemSorter"/> class
		/// that uses the specified comparers to sort columns
		/// and attaches the new instance to the specified ListView.
		/// </summary>
		/// <param name="listView">The ListView that should be sorted and monitored for column click events.</param>
		/// <param name="itemComparers">An array of objects that implement IListViewItemComparer.
		/// The index of the comparer in the array corresponds to the index of the list view column that it sorts.
		/// If an element is <c>null</c>, the corresponding column cannot be sorted.
		/// If this parameter is <c>null</c>, all columns can be sorted by their text content.</param>
		/// <remarks>
		/// Note that the ListViewItemSorter constructor adds two bitmaps to the
		/// SmallImageList of the ListView (an empty ImageList will be created
		/// first if none is assigned).
		/// </remarks>
		public ListViewItemSorter(ListView listView, IListViewItemComparer[] itemComparers)
		{
			if (listView == null) {
				throw new ArgumentNullException("listView");
			}
			this.listView = listView;
			
			if (itemComparers == null) {
				
				IListViewItemComparer defaultComparer = new ListViewTextColumnComparer();
				this.itemComparers = new IListViewItemComparer[this.ListView.Columns.Count];
				for (int i = 0; i < this.itemComparers.Length; i++) {
					this.itemComparers[i] = defaultComparer;
				}
				
			} else {
				
				this.itemComparers = itemComparers;
				
			}
			
			if (this.ListView.SmallImageList == null) {
				this.ListView.SmallImageList = new ImageList();
			}
			this.ListView.SmallImageList.Images.Add("SortAscending",  BitmapResources.GetBitmap("Icons.16x16.SortAscending.png"));
			this.ListView.SmallImageList.Images.Add("SortDescending", BitmapResources.GetBitmap("Icons.16x16.SortDescending.png"));
			
			this.ListView.ColumnClick += this.ListViewColumnClick;
			this.ListView.ListViewItemSorter = this;
		}
		
		/// <summary>
		/// Cleans up all used resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
		}
		
		void Dispose(bool disposing)
		{
			if (disposing) {
				this.ListView.ListViewItemSorter = null;
				this.ListView.ColumnClick -= this.ListViewColumnClick;
			}
		}
		
		void SetColumnHeaderIcons()
		{
			for (int i = 0; i < this.ListView.Columns.Count; i++) {
				if (i == this.SortColumnIndex && this.SortOrder != SortOrder.None) {
					if (this.SortOrder == SortOrder.Ascending) {
						this.ListView.Columns[i].ImageKey = "SortAscending";
					} else {
						this.ListView.Columns[i].ImageKey = "SortDescending";
					}
				} else {
					this.ListView.Columns[i].ImageKey = null;
				}
			}
		}
		
		void ListViewColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (e.Column >= this.itemComparers.Length || this.itemComparers[e.Column] == null) {
				return;
			}
			
			if (this.sortColumnIndex == e.Column) {
				
				// Reverse the current sort direction for this column.
				if (this.SortOrder == SortOrder.Ascending) {
					this.SortOrder = SortOrder.Descending;
				} else {
					this.SortOrder = SortOrder.Ascending;
				}
				
			} else {
				
				// assign to the field to prevent re-sorting twice
				this.sortOrder = SortOrder.Ascending;
				this.SortColumnIndex = e.Column;
				
			}
		}
		
		public int Compare(object x, object y)
		{
			if (this.SortOrder == SortOrder.None ||
			    this.SortColumnIndex < 0 || this.SortColumnIndex >= this.itemComparers.Length ||
			    this.itemComparers[this.SortColumnIndex] == null) {
				return 0;
			}
			
			int compareResult = this.itemComparers[this.SortColumnIndex].Compare((ListViewItem)x, (ListViewItem)y, this.SortColumnIndex);
			
			if (this.SortOrder == SortOrder.Ascending) {
				return compareResult;
			} else {
				return -compareResult;
			}
		}
	}
}
