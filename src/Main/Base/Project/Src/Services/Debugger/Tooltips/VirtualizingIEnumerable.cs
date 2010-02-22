// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.SharpDevelop.Debugging
{
	/// <summary>
	/// A wrapper around IEnumerable&lt;T&gt; with AddNextItems method for pulling additional items 
	/// from underlying IEnumerable&lt;T&gt;.
	/// Can be used as source for <see cref="LazyItemsControl" />.
	/// </summary>
	public class VirtualizingIEnumerable<T> : ObservableCollection<T>
	{
		private IEnumerator<T> originalSourceEnumerator;

		public VirtualizingIEnumerable(IEnumerable<T> originalSource)
		{
			if (originalSource == null)
				throw new ArgumentNullException("originalSource");

			this.originalSourceEnumerator = originalSource.GetEnumerator();
		}

		private bool hasNext = true;
		/// <summary>
		/// False if all items from underlying IEnumerable have already been added.
		/// </summary>
		public bool HasNext
		{
			get
			{
				return this.hasNext;
			}
		}

		/// <summary>
		/// Requests next <paramref name="count"/> items from underlying IEnumerable source and adds them to the collection.
		/// </summary>
		public void AddNextItems(int count)
		{
			for (int i = 0; i < count; i++) {
				if (!originalSourceEnumerator.MoveNext()) {
					this.hasNext = false;
					break;
				}
				this.Add(originalSourceEnumerator.Current);
			}
		}
	}
}
