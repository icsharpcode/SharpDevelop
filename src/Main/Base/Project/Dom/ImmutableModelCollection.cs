// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// An immutable model collection.
	/// </summary>
	public class ImmutableModelCollection<T> : ReadOnlyCollection<T>, IModelCollection<T>, IMutableModelCollection<T>
	{
		public ImmutableModelCollection(IEnumerable<T> items)
			: base(items.ToList())
		{
		}
		
		event ModelCollectionChangedEventHandler<T> IModelCollection<T>.CollectionChanged { add {} remove {} }
		
		IReadOnlyCollection<T> IModelCollection<T>.CreateSnapshot()
		{
			return this;
		}
		
		void IMutableModelCollection<T>.AddRange(IEnumerable<T> items)
		{
			throw new NotSupportedException();
		}

		int IMutableModelCollection<T>.RemoveAll(System.Predicate<T> predicate)
		{
			throw new NotSupportedException();
		}

		IDisposable IMutableModelCollection<T>.BatchUpdate()
		{
			return null;
		}
	}
}
