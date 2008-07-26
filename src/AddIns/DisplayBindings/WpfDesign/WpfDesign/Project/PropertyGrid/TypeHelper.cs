using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ICSharpCode.WpfDesign.PropertyGrid
{
	public static class TypeHelper
	{
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

		public static IEnumerable<PropertyDescriptor> GetAvailableProperties(Type forType)
		{
			foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(forType)) {
				if (!p.IsBrowsable) continue;
				if (p.IsReadOnly) continue;
				if (p.Name.Contains(".")) continue;
				yield return p;
			}
		}

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
