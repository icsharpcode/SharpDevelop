// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.AddIn.Visualizers.Common
{
	/// <summary>
	/// Can provide individial items for a lazy collection, as well as total count of items.
	/// </summary>
	public interface IListValuesProvider<T>
	{
		int GetCount();
		T GetItemAt(int index);
	}
}
