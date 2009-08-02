// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníèek" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
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
