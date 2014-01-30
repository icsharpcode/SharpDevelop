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
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.ListViewSorting
{
	/// <summary>
	/// Provides the ability to compare two ListViewItems by the
	/// content of ListViewSubItems of a specific column.
	/// The text content is converted to the type T by parsing
	/// and the parsed value is compared using its IComparable implementation.
	/// Values that are not parseable are considered less than other values
	/// and are compared by their text value.
	/// </summary>
	public abstract class AbstractListViewParseableColumnComparer<T>
		: AbstractListViewSubItemComparer
		where T : IComparable<T>
	{
		protected override int Compare(ListViewItem.ListViewSubItem lhs, ListViewItem.ListViewSubItem rhs)
		{
			T lhsValue, rhsValue;
			
			bool lhsValid = this.TryParse(lhs.Text, out lhsValue);
			bool rhsValid = this.TryParse(rhs.Text, out rhsValue);
			
			if (lhsValid) {
				
				if (rhsValid) {
					// both are valid -> compare
					return lhsValue.CompareTo(rhsValue);
				} else {
					// only left is valid -> left is greater than right
					return 1;
				}
				
			} else {
				
				if (rhsValid) {
					// only right is valid -> left is less than right
					return -1;
				} else {
					// both are invalid -> compare the text values
					return String.Compare(lhs.Text, rhs.Text, StringComparison.InvariantCulture);
				}
				
			}
		}
		
		/// <summary>
		/// Tries to convert a text value to a value of type T.
		/// </summary>
		/// <param name="textValue">The text value to convert.</param>
		/// <param name="parsedValue">Receives the converted value if successful. When <c>true</c> is returned, this parameter must not be set to <c>null</c> by the method.</param>
		/// <returns><c>true</c> if the text value has been converted successfully, otherwise <c>false</c>.</returns>
		protected abstract bool TryParse(string textValue, out T parsedValue);
	}
}
