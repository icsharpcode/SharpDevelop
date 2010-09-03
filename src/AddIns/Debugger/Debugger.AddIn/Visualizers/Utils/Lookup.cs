// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Utils
{
	/// <summary>
	/// Same like Dictionary, but can store multiple values for one key.
	/// </summary>
	public class Lookup<TKey, TValue>
	{
		/// <summary>Wrapped dictionary</summary>
		private Dictionary<TKey, LookupValueCollection<TValue>> dictionary;
		
		public Lookup()
		{
			dictionary = new Dictionary<TKey, LookupValueCollection<TValue>>();
		}
		
		public LookupValueCollection<TValue> this[TKey key]
		{
			get
			{
				LookupValueCollection<TValue> values = null;
				if (dictionary.TryGetValue(key, out values))
				{
					return values;
				}
				return null;
			}
		}
		
		public void Add(TKey key, TValue value)
		{
			LookupValueCollection<TValue> values = null;
			if (!dictionary.TryGetValue(key, out values))
			{
				values = new LookupValueCollection<TValue>();
				dictionary.Add(key, values);
			}
			values.Add(value);
		}
	}
}
