// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ICSharpCode.WpfDesign.Designer
{
	// Static helpers that should become extension methods in the future
	static class Linq
	{
		public static T[] ToArray<T>(ICollection<T> collection)
		{
			T[] arr = new T[collection.Count];
			collection.CopyTo(arr, 0);
			return arr;
		}
		
		public static PropertyDescriptor[] ToArray(PropertyDescriptorCollection collection)
		{
			PropertyDescriptor[] arr = new PropertyDescriptor[collection.Count];
			collection.CopyTo(arr, 0);
			return arr;
		}
		
		/// <summary>
		/// Returns a sorted copy of the collection.
		/// </summary>
		public static ICollection<T> Sort<T>(ICollection<T> collection, Comparison<T> comparison)
		{
			T[] arr = ToArray(collection);
			Array.Sort(arr, comparison);
			return arr;
		}
	}
}
