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
				if (p.Attributes.OfType<ObsoleteAttribute>().Count()!=0) continue;
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
				if (e.Attributes.OfType<ObsoleteAttribute>().Count()!=0) continue;
				if (e.Name.Contains(".")) continue;
				yield return e;
			}
		}
		
		private static string[] hiddenPropertiesOnWindow = new[] {"ClipToBounds"};
		
		/// <summary>
		/// Gets available properties for an object, includes attached properties also.
		/// </summary>
		public static IEnumerable<PropertyDescriptor> GetAvailableProperties(object element)
		{
			if (element.GetType().FullName == "ICSharpCode.WpfDesign.Designer.Controls.WindowClone")
			{
				foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(element))
				{
					if (!p.IsBrowsable) continue;
					if (p.IsReadOnly) continue;
					if (hiddenPropertiesOnWindow.Contains(p.Name)) continue;
					if (p.Attributes.OfType<ObsoleteAttribute>().Count() != 0) continue;
					yield return p;
				}
			}
			else
			{
				foreach(PropertyDescriptor p in TypeDescriptor.GetProperties(element)){
					if (!p.IsBrowsable) continue;
					if (p.IsReadOnly) continue;
					if (p.Attributes.OfType<ObsoleteAttribute>().Count()!=0) continue;
					yield return p;
				}
			}
		}
		
		/// <summary>
		/// Gets common properties between <paramref name="elements"/>. Includes attached properties too.
		/// </summary>
		/// <param name="elements"></param>
		/// <returns></returns>
		public static IEnumerable<PropertyDescriptor> GetCommonAvailableProperties(IEnumerable<object> elements)
		{
			foreach (var pd1 in GetAvailableProperties(elements.First())) {
				bool propertyOk = true;
				foreach (var element in elements.Skip(1)) {
					bool typeOk = false;
					foreach (var pd2 in GetAvailableProperties(element)) {
						if (pd1 == pd2) {
							typeOk = true;
							break;
						}
						
						/* Check if it is attached property.*/
						if(pd1.Name.Contains(".") && pd2.Name.Contains(".")){
							if(pd1.Name==pd2.Name){
								typeOk=true;
								break;
							}
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
		
	}
}
