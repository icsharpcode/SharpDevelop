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
using System.Collections.ObjectModel;

namespace ICSharpCode.WpfDesign.PropertyGrid
{
	/// <summary>
	/// Extends ObservableCollection{T} with an AddSorted method to insert items in a sorted collection.
	/// </summary>
	public class SortedObservableCollection<T, TKey> : ObservableCollection<T>
	{
		/// <summary>
		/// Creates a new SortedObservableCollection instance.
		/// </summary>
		/// <param name="keySelector">The function to select the sorting key.</param>
		public SortedObservableCollection(Func<T, TKey> keySelector)
		{
			this.keySelector = keySelector;
			this.comparer = Comparer<TKey>.Default;
		}

		Func<T, TKey> keySelector;
		IComparer<TKey> comparer;

		/// <summary>
		/// Adds an item to a sorted collection.
		/// </summary>
		public void AddSorted(T item)
		{
			int i = 0;
			int j = Count - 1;

			while (i <= j) {
				int n = (i + j) / 2;
				int c = comparer.Compare(keySelector(item), keySelector(this[n]));

				if (c == 0) { i = n; break; }
				if (c > 0) i = n + 1;
				else j = n - 1;
			}

			Insert(i, item);
		}
	}
	
	/// <summary>
	/// A SortedObservableCollection{PropertyNode, string} that sorts by the PropertyNode's Name.
	/// </summary>
	public class PropertyNodeCollection : SortedObservableCollection<PropertyNode, string>
	{
		/// <summary>
		/// Creates a new PropertyNodeCollection instance.
		/// </summary>
		public PropertyNodeCollection() : base(n => n.Name)
		{
		}
	}
}
