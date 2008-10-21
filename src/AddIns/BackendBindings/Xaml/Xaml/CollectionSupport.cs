// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3509 $</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace ICSharpCode.Xaml
{
	/// <summary>
	/// Static class containing helper methods to work with collections (like the XamlParser does)
	/// </summary>
	public static class CollectionSupport
	{
		/// <summary>
		/// Gets if the type is considered a collection in XAML.
		/// </summary>
		public static bool IsCollectionType(Type type)
		{
			return typeof(IList).IsAssignableFrom(type)
				|| type.IsArray
				|| typeof(IAddChild).IsAssignableFrom(type)
				|| typeof(ResourceDictionary).IsAssignableFrom(type);
		}

		/// <summary>
		/// Gets if the collection type <paramref name="col"/> can accepts items of type
		/// <paramref name="item"/>.
		/// </summary>
		public static bool CanCollectionAdd(Type col, Type item)
		{
			var e = col.GetInterface("IEnumerable`1");
			if (e != null && e.IsGenericType) {
				var a = e.GetGenericArguments()[0];
				return a.IsAssignableFrom(item);
			}
			return true;
		}

		/// <summary>
		/// Gets if the collection type <paramref name="col"/> can accept the specified items.
		/// </summary>
		public static bool CanCollectionAdd(Type col, IEnumerable items)
		{
			foreach (var item in items) {
				if (!CanCollectionAdd(col, item.GetType())) return false;
			}
			return true;
		}
	}
}
