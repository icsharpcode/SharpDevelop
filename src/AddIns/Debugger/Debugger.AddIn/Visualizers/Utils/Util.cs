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
		
		public static string Repeat(char c, int count)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(c, count);
			return sb.ToString();
		}
		
		/// <summary>
		/// Gets value from Dictionary. Returns null if not found.
		/// </summary>
		public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : class
		{
			TValue outValue;
			if (dictionary.TryGetValue(key, out outValue)) {
				return outValue;
			}
			return null;
		}
		
		/// <summary>
		/// Builds a Dictionary for quickly searching a collection. Item keys must be unique.
		/// </summary>
		/// <param name="collection">Collection for which to build Dictionary.</param>
		/// <param name="keySelector">Function returning key by which to index.</param>
		public static Dictionary<K, V> MakeDictionary<K, V>(this IEnumerable<V> collection, Func<V, K> keySelector)
		{
			Dictionary<K, V> dictionary = new Dictionary<K, V>();
			foreach (V item in collection) {
				K key = keySelector(item);
				if (dictionary.ContainsKey(key)) throw new InvalidOperationException("MakeDictionary: key " + key + " seen twice");
				dictionary[key] = item;
			}
			return dictionary;
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
