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
	/// Compares ListViewItems by the text of a specific column.
	/// </summary>
	public class ListViewTextColumnComparer : AbstractListViewSubItemComparer
	{
		readonly IComparer<string> stringComparer;
		
		/// <summary>
		/// Creates a new instance of the <see cref="ListViewTextColumnComparer"/> class
		/// with a culture-independent and case-sensitive string comparer.
		/// </summary>
		public ListViewTextColumnComparer()
			: this(StringComparer.InvariantCulture)
		{
		}
		
		/// <summary>
		/// Creates a new instance of the <see cref="ListViewTextColumnComparer"/> class
		/// with the specified string comparer.
		/// </summary>
		/// <param name="stringComparer">The string comparer used to compare the item texts.</param>
		public ListViewTextColumnComparer(IComparer<string> stringComparer)
		{
			if (stringComparer == null) {
				throw new ArgumentNullException("stringComparer");
			}
			this.stringComparer = stringComparer;
		}
		
		protected override int Compare(ListViewItem.ListViewSubItem lhs, ListViewItem.ListViewSubItem rhs)
		{
			return this.stringComparer.Compare(lhs.Text, rhs.Text);
		}
	}
}
