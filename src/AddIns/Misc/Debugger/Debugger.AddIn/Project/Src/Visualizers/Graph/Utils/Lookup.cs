// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníèek" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Graph.Utils
{
	/// <summary>
	/// Same like Dictionary, but can store multiple values for one key.
	/// </summary>
	public class Lookup<TKey, TValue>
	{
		private Dictionary<TKey, LookupValueCollection<TValue>> _dictionary;
		
		public Lookup()
		{
			_dictionary = new Dictionary<TKey, LookupValueCollection<TValue>>();
		}
		
		public LookupValueCollection<TValue> this[TKey key]
		{
			get 
			{
				LookupValueCollection<TValue> values = null;
				if (_dictionary.TryGetValue(key, out values))
				{
					return values;
				}
				return null;
			}
		}
		
		public void Add(TKey key, TValue value)
		{
			LookupValueCollection<TValue> values = null;;
            if (!_dictionary.TryGetValue(key, out values))
            {
            	values = new LookupValueCollection<TValue>();
                 _dictionary.Add(key, values);
            }
            values.Add(value);
		}
	}
}
