// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Debugger.AddIn.Visualizers.Utils
{
	public static class Util
	{
		public static int MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, int defaultValue)
		{
			if (source.Count() == 0) {
				return defaultValue;
			}
			return source.Max(selector);
		}
		
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
