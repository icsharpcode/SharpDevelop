// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;

namespace Debugger.AddIn.Visualizers
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
