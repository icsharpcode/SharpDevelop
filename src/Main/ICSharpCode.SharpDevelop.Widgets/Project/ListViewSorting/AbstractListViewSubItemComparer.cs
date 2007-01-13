// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.ListViewSorting
{
	/// <summary>
	/// Provides the ability to compare two ListViewItems by the
	/// ListViewSubItems of a specific column.
	/// </summary>
	public abstract class AbstractListViewSubItemComparer : IListViewItemComparer
	{
		public int Compare(ListViewItem lhs, ListViewItem rhs, int column)
		{
			return this.Compare(lhs.SubItems[column], rhs.SubItems[column]);
		}
		
		/// <summary>
		/// Compares two ListViewSubItems.
		/// </summary>
		/// <returns>
		/// Less than zero if <paramref name="lhs"/> is less than <paramref name="rhs"/>.
		/// Zero if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
		/// Greater than zero if <paramref name="lhs"/> is greater than <paramref name="rhs"/>.
		/// </returns>
		/// <remarks>
		/// The implementation must always compare the specified subitems in ascending order.
		/// </remarks>
		protected abstract int Compare(ListViewItem.ListViewSubItem lhs, ListViewItem.ListViewSubItem rhs);
	}
}
