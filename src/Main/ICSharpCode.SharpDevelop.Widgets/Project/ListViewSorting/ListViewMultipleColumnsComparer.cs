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
