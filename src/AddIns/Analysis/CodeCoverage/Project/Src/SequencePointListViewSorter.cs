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
