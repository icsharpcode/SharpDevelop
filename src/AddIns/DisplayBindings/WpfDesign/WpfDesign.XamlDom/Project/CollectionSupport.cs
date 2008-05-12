// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom
{
	static class CollectionSupport
	{
		public static bool IsCollectionType(Type type)
		{
			return typeof(IList).IsAssignableFrom(type)
				|| type.IsArray
				|| typeof(IAddChild).IsAssignableFrom(type)
				|| typeof(ResourceDictionary).IsAssignableFrom(type);
		}
		
		public static void AddToCollection(Type collectionType, object collectionInstance, XamlPropertyValue newElement)
		{
			IAddChild addChild = collectionInstance as IAddChild;
			if (addChild != null) {
				if (newElement is XamlTextValue) {
					addChild.AddText((string)newElement.GetValueFor(null));
				} else {
					addChild.AddChild(newElement.GetValueFor(null));
				}
			} else if (collectionInstance is ResourceDictionary) {
				object val = newElement.GetValueFor(null);
				object key = newElement is XamlObject ? ((XamlObject)newElement).GetXamlAttribute("Key") : null;
				if (key == null) {
					if (val is Style)
						key = ((Style)val).TargetType;
				}
				((ResourceDictionary)collectionInstance).Add(key, val);
			} else {
				collectionType.InvokeMember(
					"Add", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
					null, collectionInstance,
					new object[] { newElement.GetValueFor(null) },
					CultureInfo.InvariantCulture);
			}
		}
		
		public static void Insert(Type collectionType, object collectionInstance, XamlPropertyValue newElement, int index)
		{
			collectionType.InvokeMember(
				"Insert", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
				null, collectionInstance,
				new object[] { index, newElement.GetValueFor(null) },
				CultureInfo.InvariantCulture);
		}
		
		static readonly Type[] RemoveAtParameters = { typeof(int) };
		
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
		
		public static void RemoveItem(Type collectionType, object collectionInstance, object item)
		{
			collectionType.InvokeMember(
				"Remove", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
				null, collectionInstance,
				new object[] { item },
				CultureInfo.InvariantCulture);
		}
	}
}
