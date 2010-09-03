// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Utils
{
	public static class IEnumerableExtensions
	{
		/// <summary>
		/// Builds Dictionary for quickly searching a collection.
		/// The keys must be unique.
		/// </summary>
		/// <param name="collection">Collection for which to build Dictionary.</param>
		/// <param name="keySelector">Function returning key by which to index.</param>
		public static Dictionary<K, V> MakeDictionary<K, V>(this IEnumerable<V> collection, Func<V, K> keySelector)
		{
			Dictionary<K, V> dictionary = new Dictionary<K, V>();
			foreach (V item in collection)
			{
				K key = keySelector(item);
				if (dictionary.ContainsKey(key)) throw new InvalidOperationException("MakeDictionary: key " + key + " seen twice");
				dictionary[key] = item;
			}
			return dictionary;
		}

		/// <summary>
		/// Builds Lookup for quickly searching a collection.
		/// </summary>
		/// <param name="collection">Collection for which to build Dictionary.</param>
		/// <param name="keySelector">Function returning key by which to index.</param>
		public static Lookup<K, V> MakeLookup<K, V>(this IEnumerable<V> collection, Func<V, K> keySelector)
		{
			Lookup<K, V> lookup = new Lookup<K, V>();
			foreach (V item in collection)
			{
				lookup.Add(keySelector(item), item);
			}
			return lookup;
		}
	}
}
