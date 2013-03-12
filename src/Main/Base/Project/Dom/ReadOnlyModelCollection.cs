// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A model collection implementation that is based on a ReadOnlyCollection.
	/// </summary>
	public class ReadOnlyModelCollection<T> : ReadOnlyCollection<T>, IModelCollection<T>
	{
		public ReadOnlyModelCollection(IEnumerable<T> items)
			: base(items.ToList())
		{
		}
		
		event ModelCollectionChangedEventHandler<T> IModelCollection<T>.CollectionChanged { add {} remove {} }
	}
}
