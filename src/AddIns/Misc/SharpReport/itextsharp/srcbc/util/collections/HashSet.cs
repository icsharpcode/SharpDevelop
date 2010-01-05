using System;
using System.Collections;

namespace Org.BouncyCastle.Utilities.Collections
{
	public class HashSet
		: ISet
	{
		private readonly Hashtable impl = new Hashtable();

		public HashSet()
		{
		}

		public HashSet(ISet s)
		{
			foreach (object o in s)
			{
				Add(o);
			}
		}

		public void Add(object o)
		{
			impl[o] = null;
		}

		public bool Contains(object o)
		{
			return impl.ContainsKey(o);
		}

		public void CopyTo(Array array, int index)
		{
			impl.Keys.CopyTo(array, index);
		}

		public int Count
		{
			get { return impl.Count; }
		}

		public IEnumerator GetEnumerator()
		{
			return impl.Keys.GetEnumerator();
		}

		public bool IsSynchronized
		{
			get { return impl.IsSynchronized; }
		}

		public void Remove(object o)
		{
			impl.Remove(o);
		}

		public object SyncRoot
		{
			get { return impl.SyncRoot; }
		}
	}
}
