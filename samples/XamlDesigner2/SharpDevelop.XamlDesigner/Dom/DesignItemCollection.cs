using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;

namespace SharpDevelop.XamlDesigner.Dom
{
	public class DesignItemCollection : DesignItem, IList<DesignItem>, INotifyCollectionChanged
	{
		internal DesignItemCollection(DesignContext context, Type type) : base(context, type)
		{
		}

		List<DesignItem> list = new List<DesignItem>();

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		internal void ParserAdd(DesignItem item)
		{
			list.Add(item);
		}

		#region IList<DesignItem> Members

		public int IndexOf(DesignItem item)
		{
			return list.IndexOf(item);
		}

		public void Insert(int index, DesignItem item)
		{
			list.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			list.RemoveAt(index);
		}

		public DesignItem this[int index]
		{
			get
			{
				return list[index];
			}
			set
			{
				list[index] = value;
			}
		}

		public void Add(DesignItem item)
		{
			list.Add(item);
		}

		public void Clear()
		{
			list.Clear();
		}

		public bool Contains(DesignItem item)
		{
			return list.Contains(item);
		}

		public void CopyTo(DesignItem[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(DesignItem item)
		{
			return list.Remove(item);
		}

		public IEnumerator<DesignItem> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		#endregion
	}
}
