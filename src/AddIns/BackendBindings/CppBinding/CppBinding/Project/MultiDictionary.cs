// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ICSharpCode.CppBinding.Project
{
	public interface IMultiDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
	{
		void Add(TKey key, TValue value);
		bool Contains(TKey key, TValue value);
		bool ContainsKey(TKey key);
		bool Remove(TKey key, TValue value);
		IList<TValue> this[TKey key] { get; }
	}

	/// <summary>
	/// A dictionary that allows multiple pairs with the same key.
	/// </summary>
	public class MultiDictionary<TKey, TValue> : IMultiDictionary<TKey,TValue>
	{
		public MultiDictionary()
			: this(new Dictionary<TKey, IList<TValue>>())
		{ }

		public MultiDictionary(IDictionary<TKey, IList<TValue>> innerDictionary)
		{
			if (innerDictionary == null)
				throw new ArgumentNullException("innerDictionary");
			dict = innerDictionary;
			count = CountElements(dict);
		}
		IDictionary<TKey, IList<TValue>> dict;
		int count;

		public void Add(TKey key, TValue value)
		{
			IList<TValue> valueList;
			if (!dict.TryGetValue(key, out valueList))
			{
				valueList = new List<TValue>();
				dict.Add(key, valueList);
			}
			valueList.Add(value);
			count++;
		}

		public bool Contains(TKey key, TValue value)
		{
			IList<TValue> valueList;
			if (!dict.TryGetValue(key, out valueList))
				return false;
			return valueList.Contains(value);
		}

		public bool ContainsKey(TKey key)
		{
			return dict.ContainsKey(key);
		}

		public bool Remove(TKey key, TValue value)
		{
			IList<TValue> valueList;
			if (!dict.TryGetValue(key, out valueList))
				return false;
			return valueList.Remove(value);
		}

		public IList<TValue> this[TKey key]
		{
			get
			{
				return new ReadOnlyCollection<TValue>(dict[key]);
			}
		}

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			dict.Clear();
			count = 0;
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return Contains(item.Key, item.Value);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException("arrayIndex");
			if (array.Rank != 1)
				throw new ArgumentException("Array is multidimensional", "array");
			if (arrayIndex + count >= array.Length)
				throw new ArgumentException("Array is to small", "array");

			foreach (KeyValuePair<TKey, IList<TValue>> item in dict)
				foreach (TValue value in item.Value)
					array[arrayIndex++] = new KeyValuePair<TKey, TValue>(item.Key, value);
		}

		public int Count
		{
			get { return count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key, item.Value);
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			foreach (KeyValuePair<TKey, IList<TValue>> item in dict)
				foreach (TValue value in item.Value)
					yield return new KeyValuePair<TKey, TValue>(item.Key, value);
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		static int CountElements(IDictionary<TKey, IList<TValue>> dict)
		{
			int count = 0;
			foreach (KeyValuePair<TKey, IList<TValue>> item in dict)
				count += item.Value.Count;
			return count;
		}
	}
}
