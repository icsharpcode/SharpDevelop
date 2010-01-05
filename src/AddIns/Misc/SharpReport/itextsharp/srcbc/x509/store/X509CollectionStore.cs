using System;
using System.Collections;

namespace Org.BouncyCastle.X509.Store
{
	/**
	 * A simple collection backed store.
	 */
	internal class X509CollectionStore
		: IX509Store
	{
		private ICollection _local;

		/**
		 * Basic constructor.
		 *
		 * @param collection - initial contents for the store, this is copied.
		 */
		internal X509CollectionStore(
			ICollection collection)
		{
			_local = new ArrayList(collection);
		}

		/**
		 * Return the matches in the collection for the passed in selector.
		 *
		 * @param selector the selector to match against.
		 * @return a possibly empty collection of matching objects.
		 */
		public ICollection GetMatches(
			IX509Selector selector)
		{
			if (selector == null)
			{
				return new ArrayList(_local);
			}

			IList result = new ArrayList();
			foreach (object obj in _local)
			{
				if (selector.Match(obj))
					result.Add(obj);
			}

			return result;
		}
	}
}
