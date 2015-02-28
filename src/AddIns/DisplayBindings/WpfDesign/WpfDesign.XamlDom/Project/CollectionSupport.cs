// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom
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
			return type != typeof(LineBreak) && (
				typeof(IList).IsAssignableFrom(type)
				|| type.IsArray
				|| typeof(IAddChild).IsAssignableFrom(type)
				|| typeof(IDictionary).IsAssignableFrom(type));
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
		
		/// <summary>
		/// Adds a value to the end of a collection.
		/// </summary>
		public static void AddToCollection(Type collectionType, object collectionInstance, XamlPropertyValue newElement)
		{
			IAddChild addChild = collectionInstance as IAddChild;
			if (addChild != null) {
				if (newElement is XamlTextValue) {
					addChild.AddText((string)newElement.GetValueFor(null));
				} else {
					addChild.AddChild(newElement.GetValueFor(null));
				}
			} else if (collectionInstance is IDictionary) {
				object val = newElement.GetValueFor(null);
				object key = newElement is XamlObject ? ((XamlObject)newElement).GetXamlAttribute("Key") : null;
				if (key == null || (key as string) == "")
				{
					if (val is Style)
						key = ((Style)val).TargetType;
				}
				if (key == null || (key as string) == "")
					key = val;
				((IDictionary)collectionInstance).Add(key, val);
			} else {
				collectionType.InvokeMember(
					"Add", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
					null, collectionInstance,
					new object[] { newElement.GetValueFor(null) },
					CultureInfo.InvariantCulture);
			}
		}
		
		/// <summary>
		/// Adds a value at the specified index in the collection.
		/// </summary>
		public static bool Insert(Type collectionType, object collectionInstance, XamlPropertyValue newElement, int index)
		{
			object value = newElement.GetValueFor(null);
			
			// Using IList, with possible Add instead of Insert, was primarily added as a workaround
			// for a peculiarity (or bug) with collections inside System.Windows.Input namespace.
			// See CollectionTests.InputCollectionsPeculiarityOrBug test method for details.
			var list = collectionInstance as IList;
			if (list != null) {
				if (list.Count == index) {
					list.Add(value);
				}
				else {
					list.Insert(index, value);
				}
				return true;
			} else {
				var hasInsert = collectionType.GetMethods().Any(x => x.Name == "Insert");
			
				if (hasInsert) {
					collectionType.InvokeMember(
						"Insert", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
						null, collectionInstance,
						new object[] { index, value },
						CultureInfo.InvariantCulture);
				
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// Adds a value at the specified index in the collection. A return value indicates whether the Insert succeeded.
		/// </summary>
		/// <returns>True if the Insert succeeded, false if the collection type does not support Insert.</returns>
		internal static bool TryInsert(Type collectionType, object collectionInstance, XamlPropertyValue newElement, int index)
		{
			try {
				return Insert(collectionType, collectionInstance, newElement, index);
			} catch (MissingMethodException) {
				return false;
			}
		}
		
		static readonly Type[] RemoveAtParameters = { typeof(int) };
		
		/// <summary>
		/// Removes the item at the specified index of the collection.
		/// </summary>
		/// <returns>True if the removal succeeded, false if the collection type does not support RemoveAt.</returns>
		public static bool RemoveItemAt(Type collectionType, object collectionInstance, int index)
		{
			MethodInfo m = collectionType.GetMethod("RemoveAt", RemoveAtParameters);
			if (m != null) {
				m.Invoke(collectionInstance, new object[] { index });
				return true;
			} else {
				return false;
			}
		}
		
		/// <summary>
		/// Removes an item instance from the specified collection.
		/// </summary>
		public static void RemoveItem(Type collectionType, object collectionInstance, object item)
		{
			collectionType.InvokeMember(
				"Remove", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
				null, collectionInstance,
				new object[] { item },
				CultureInfo.InvariantCulture);
		}
		
		/// <summary>
		/// Removes an item instance from the specified collection.
		/// </summary>
		internal static void RemoveItem(Type collectionType, object collectionInstance, object item, XamlPropertyValue element)
		{
			var dictionary = collectionInstance as IDictionary;
			var xamlObject = element as XamlObject;
			
			if (dictionary != null && xamlObject != null) {
				dictionary.Remove(xamlObject.GetXamlAttribute("Key"));
			} else {
				RemoveItem(collectionType, collectionInstance, item);
			}
		}
	}
}
