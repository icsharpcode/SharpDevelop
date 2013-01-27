// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Utils
{
	/// <summary> Dictionary that can store multiple values for each key.</summary>
	public class Multimap<TKey, TValue>
	{
		/// <summary>Wrapped dictionary</summary>
		private Dictionary<TKey, IList<TValue>> dictionary;
		
		public Multimap()
		{
			dictionary = new Dictionary<TKey, IList<TValue>>();
		}
		
		public IList<TValue> this[TKey key]
		{
			get {
				IList<TValue> values = null;
				if (dictionary.TryGetValue(key, out values)) {
					return values;
				}
				return null;
			}
		}
		
		public void Add(TKey key, TValue value)
		{
			IList<TValue> values = null;
			if (!dictionary.TryGetValue(key, out values)) {
				values = new List<TValue>();
				dictionary.Add(key, values);
			}
			values.Add(value);
		}
	}
}
