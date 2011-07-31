// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Utils
{
	/// <summary>
	/// ListHelper wraps System.Collection.Generic.List methods to return the original list,
	/// instead of returning 'void', so we can write eg. list.Sorted().First()
	/// </summary>
	public static class ListHelper
	{
		public static List<T> Sorted<T>(this List<T> list, IComparer<T> comparer)
		{
			list.Sort(comparer);
			return list;
		}
		
		public static List<T> Sorted<T>(this List<T> list)
		{
			list.Sort();
			return list;
		}
		
		public static List<T> SingleItemList<T>(this T singleItem)
		{
			var newList = new List<T>();
			newList.Add(singleItem);
			return newList;
		}
	}
}
