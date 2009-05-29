// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Graph.Utils
{
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Gets value from Dictionary, returns null if not found.
		/// </summary>
		public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : class
		{
			TValue outValue;
			if (dictionary.TryGetValue(key, out outValue))
			{
				return outValue;
			}
			else
			{
				return null;
			}
		}
	}
}
