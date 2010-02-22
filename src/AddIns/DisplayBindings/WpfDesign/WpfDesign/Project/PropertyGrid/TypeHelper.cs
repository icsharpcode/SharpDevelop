// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Ivan Shumilin"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ICSharpCode.WpfDesign.PropertyGrid
{
	/// <summary>
	/// Helper class with static methods to get the list of available properties/events.
	/// </summary>
	public static class TypeHelper
	{
		/// <summary>
		/// Gets the available properties common to all input types.
		/// </summary>
		/// <param name="types">List of input types. The list must have at least one element.</param>
		public static IEnumerable<PropertyDescriptor> GetCommonAvailableProperties(IEnumerable<Type> types)
		{
			foreach (var pd1 in GetAvailableProperties(types.First())) {
				bool propertyOk = true;
				foreach (var type in types.Skip(1)) {
					bool typeOk = false;
					foreach (var pd2 in GetAvailableProperties(type)) {
						if (pd1 == pd2) {
							typeOk = true;
							break;
						}
					}
					if (!typeOk) {
						propertyOk = false;
						break;
					}
				}
				if (propertyOk) yield return pd1;
			}
		}

		/// <summary>
		/// Gets the available properties for the type.
		/// </summary>
		public static IEnumerable<PropertyDescriptor> GetAvailableProperties(Type forType)
		{
			foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(forType)) {
				if (!p.IsBrowsable) continue;
				if (p.IsReadOnly) continue;
				if (p.Name.Contains(".")) continue;
				yield return p;
			}
		}

		/// <summary>
		/// Gets the available events for the type.
		/// </summary>
		public static IEnumerable<EventDescriptor> GetAvailableEvents(Type forType)
		{
			foreach (EventDescriptor e in TypeDescriptor.GetEvents(forType)) {
				if (!e.IsBrowsable) continue;
				if (e.Name.Contains(".")) continue;
				yield return e;
			}
		}
	}
}
