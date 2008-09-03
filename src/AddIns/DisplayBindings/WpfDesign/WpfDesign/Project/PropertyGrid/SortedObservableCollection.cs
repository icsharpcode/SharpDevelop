using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ICSharpCode.WpfDesign.PropertyGrid
{
	public class SortedObservableCollection<T, K> : ObservableCollection<T>
	{
		public SortedObservableCollection(Func<T, K> keySelector)
		{
			this.keySelector = keySelector;
			this.comparer = Comparer<K>.Default;
		}

		Func<T, K> keySelector;
		IComparer<K> comparer;

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

	public class PropertyNodeCollection : SortedObservableCollection<PropertyNode, string>
	{
		public PropertyNodeCollection() : base(n => n.Name)
		{
		}
	}
}
