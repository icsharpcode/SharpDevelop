// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;

namespace ICSharpCode.SharpDevelop.Widgets.ListViewSorting
{
	/// <summary>
	/// Compares ListViewItems by the signed integer content of a specific column.
	/// </summary>
	public sealed class ListViewIntegerParseColumnComparer
		: AbstractListViewParseableColumnComparer<Int64>
	{
		protected override bool TryParse(string textValue, out Int64 parsedValue)
		{
			return Int64.TryParse(textValue, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedValue);
		}
	}
}
