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
