// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Utils
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
