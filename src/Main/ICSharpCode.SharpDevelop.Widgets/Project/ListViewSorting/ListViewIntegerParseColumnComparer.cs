// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

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
