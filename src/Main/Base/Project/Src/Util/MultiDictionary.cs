// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory;
using Mono.Collections.Generic;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// A dictionary that allows multiple pairs with the same key.
	/// </summary>
	public class MultiDictionary<TKey, TValue> : ILookup<TKey, TValue>
	{
		Dictionary<TKey, List<TValue>> dict;

		public MultiDictionary()
		{
		}

		public MultiDictionary(IEqualityComparer<TKey> comparer)
		{
			dict = new Dictionary<TKey, List<TValue>>(comparer);
		}

		public void Add(TKey key, TValue value)
		{
			List<TValue> valueList;
			if (!dict.TryGetValue(key, out valueList)) {
				valueList = new List<TValue>();
				dict.Add(key, valueList);
			}
			valueList.Add(value);
		}

		public bool Remove(TKey key, TValue value)
		{
			List<TValue> valueList;
			if (dict.TryGetValue(key, out valueList)) {
				if (valueList.Remove(value)) {
					if (valueList.Count == 0)
						dict.Remove(key);
					return true;
				}
			}
			return false;
		}

		public void Clear()
		{
			dict.Clear();
		}

		public IEnumerable<TValue> this[TKey key] {
			get {
				List<TValue> list;
				if (dict.TryGetValue(key, out list))
					return list.AsReadOnly();
				else
					return new TValue[0];
			}
		}

		public int Count {
			get { return dict.Count; }
		}

		IEnumerable<TValue> ILookup<TKey, TValue>.this[TKey key] {
			get { return this[key]; }
		}

		bool ILookup<TKey, TValue>.Contains(TKey key)
		{
			return dict.ContainsKey(key);
		}

		public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
		{
			foreach (var pair in dict)
				yield return new Grouping(pair.Key, pair.Value);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		sealed class Grouping : IGrouping<TKey, TValue>
		{
			readonly TKey key;
			readonly List<TValue> values;

			public Grouping(TKey key, List<TValue> values)
			{
				this.key = key;
				this.values = values;
			}

			public TKey Key {
				get { return key; }
			}

			public IEnumerator<TValue> GetEnumerator()
			{
				return values.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return values.GetEnumerator();
			}
		}
	}
}