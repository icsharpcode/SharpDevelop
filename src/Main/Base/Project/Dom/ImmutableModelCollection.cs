// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// An immutable model collection.
	/// </summary>
	public class ImmutableModelCollection<T> : ReadOnlyCollection<T>, IModelCollection<T>
	{
		static ImmutableModelCollection<T> empty = new ImmutableModelCollection<T>(Enumerable.Empty<T>());
		
		/// <summary>
		/// Gets the empty model collection.
		/// </summary>
		public static ImmutableModelCollection<T> Empty {
			get { return empty; }
		}
		
		public ImmutableModelCollection(IEnumerable<T> items)
			: base(items.ToList())
		{
		}
		
		event ModelCollectionChangedEventHandler<T> IModelCollection<T>.CollectionChanged { add {} remove {} }
		
		IReadOnlyCollection<T> IModelCollection<T>.CreateSnapshot()
		{
			return this;
		}
	}
	
	class ImmutableModelCollectionImplementsMutableInterface<T> : ImmutableModelCollection<T>, IMutableModelCollection<T>
	{
		public ImmutableModelCollectionImplementsMutableInterface(IEnumerable<T> items) : base(items)
		{
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
