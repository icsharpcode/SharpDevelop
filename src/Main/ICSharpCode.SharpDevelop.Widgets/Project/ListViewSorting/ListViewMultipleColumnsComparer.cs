// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.ListViewSorting
{
	/// <summary>
	/// Compares ListViewItems by comparing multiple columns,
	/// using an object that implements <see cref="IListViewItemComparer"/>
	/// for every column to compare.
	/// </summary>
	public class ListViewMultipleColumnsComparer : IListViewItemComparer
	{
		readonly List<IListViewItemComparer> comparers = new List<IListViewItemComparer>(2);
		readonly List<int> columns = new List<int>(2);
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewMultipleColumnsComparer"/> class.
		/// </summary>
		/// <param name="firstComparer">The <see cref="IListViewItemComparer"/> to use to compare the first column.</param>
		/// <param name="firstColumn">The 0-based index of the first column to compare.</param>
		/// <param name="secondComparer">The <see cref="IListViewItemComparer"/> to use to compare the second column.</param>
		/// <param name="secondColumn">The 0-based index of the second column to compare.</param>
		/// <remarks>
		/// You can add more columns to compare by using the <see cref="AddComparer"/> method.
		/// </remarks>
		public ListViewMultipleColumnsComparer(IListViewItemComparer firstComparer, int firstColumn, IListViewItemComparer secondComparer, int secondColumn)
		{
			if (firstComparer == null) {
				throw new ArgumentNullException("firstComparer");
			}
			if (secondComparer == null) {
				throw new ArgumentNullException("secondComparer");
			}
			
			this.AddComparer(firstComparer, firstColumn);
			this.AddComparer(secondComparer, secondColumn);
		}
		
		/// <summary>
		/// Adds another column to compare.
		/// </summary>
		/// <param name="comparer">The <see cref="IListViewItemComparer"/> to use to compare this column.</param>
		/// <param name="column">The 0-based index of the column to compare.</param>
		public void AddComparer(IListViewItemComparer comparer, int column)
		{
			if (comparer == null) {
				throw new ArgumentNullException("comparer");
			}
			this.comparers.Add(comparer);
			this.columns.Add(column);
		}
		
		public int Compare(ListViewItem lhs, ListViewItem rhs, int column)
		{
			int compareResult;
			
			for (int i = 0; i < this.comparers.Count; i++) {
				if ((compareResult = this.comparers[i].Compare(lhs, rhs, this.columns[i])) != 0) {
					return compareResult;
				}
			}
			
			return 0;
		}
	}
}
