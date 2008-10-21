using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.Xaml
{
	public static class ExtensionMethods
	{
		public static V FindOrCreate<K, V>(this Dictionary<K, V> dict, K key) where V : new()
		{
			V value;
			if (!dict.TryGetValue(key, out value)) {
				value = new V();
				dict[key] = value;
			}
			return value;
		}
	}
}
