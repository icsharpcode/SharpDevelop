// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Sorts the list view that contains code coverage sequence
	/// points.
	/// </summary>
	public class SequencePointListViewSorter : IComparer, IDisposable
	{
		ListView listView;
		int column = -1;
		SortOrder sortOrder = SortOrder.None;
		
		const int VisitCountColumn = 0;
		const int SequencePointLineColumn = 1;
		const int SequencePointStartColumnColumn = 2;
		const int SequencePointEndLineColumn = 3;
		const int SequencePointEndColumnColumn = 4;
		
		public SequencePointListViewSorter(ListView listView)
		{
			this.listView = listView;
			listView.ListViewItemSorter = this;
			listView.ColumnClick += ListViewColumnClick;
		}
		
		public void Dispose()
		{
			if (listView != null) {
				listView.ColumnClick -= ListViewColumnClick;
			}
		}
		
		/// <summary>
		/// Compares two list view items and sorts them according
		/// to the currently sorted column.
		/// </summary>
		public int Compare(object x, object y)
		{
			CodeCoverageSequencePoint lhs = null;
			CodeCoverageSequencePoint rhs = null;
			
			ListViewItem item = x as ListViewItem;
			if (item != null) {
				lhs = item.Tag as CodeCoverageSequencePoint;
			}
			
			item = y as ListViewItem;
			if (item != null) {
				rhs = item.Tag as CodeCoverageSequencePoint;
			}
			
			if (lhs != null && rhs != null) {
				return Compare(lhs, rhs);
			}
			return 0;
		}
		
		/// <summary>
		/// Sorts the list view by the specified column.
		/// </summary>
		public void Sort(int column)
		{
			if (this.column == column) {
				ToggleSortOrder();
			} else {
				sortOrder = SortOrder.Ascending;
			}
			this.column = column;
			listView.Sort();
		}
		
		/// <summary>
		/// Compares two code coverage sequence points based on the
		/// currently sorted column and sort order.
		/// </summary>
		int Compare(CodeCoverageSequencePoint x, CodeCoverageSequencePoint y)
		{
			int result = 0;
			switch (column) {
				case VisitCountColumn:
					result = x.VisitCount - y.VisitCount;
					break;
				case SequencePointLineColumn:
					result = x.Line - y.Line;
					break;
				case SequencePointStartColumnColumn:
					result = x.Column - y.Column;
					break;
				case SequencePointEndLineColumn:
					result = x.EndLine - y.EndLine;
					break;
				case SequencePointEndColumnColumn:
					result = x.EndColumn - y.EndColumn;
					break;
			}
			
			// Sort by secondary sort column?
			if (result == 0 && column != SequencePointLineColumn) {
				result = x.Line - y.Line;
			}
			
			if (sortOrder == SortOrder.Descending) {
				return -result;
			}
			return result;
		}
		
		/// <summary>
		/// Switches the sort order from ascending to descending
		/// and vice versa.
		/// </summary>
		void ToggleSortOrder()
		{
			if (sortOrder == SortOrder.Ascending) {
				sortOrder = SortOrder.Descending;
			} else {
				sortOrder = SortOrder.Ascending;
			}
		}
		
		/// <summary>
		/// User clicked a column header so sort that column.
		/// </summary>
		void ListViewColumnClick(object source, ColumnClickEventArgs e)
		{
			Sort(e.Column);
		}
	}
}
