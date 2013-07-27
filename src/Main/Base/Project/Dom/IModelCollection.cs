// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A read-only collection that provides change notifications.
	/// </summary>
	/// <remarks>
	/// We don't use INotifyCollectionChanged because that has the annoying 'Reset' update,
	/// where it's impossible for to detect what kind of changes happened unless the event consumer
	/// maintains a copy of the list.
	/// Also, INotifyCollectionChanged isn't type-safe.
	/// </remarks>
	public interface IModelCollection<out T> : IReadOnlyCollection<T>
	{
		event ModelCollectionChangedEventHandler<T> CollectionChanged;
		
		/// <summary>
		/// Creates an immutable snapshot of the collection.
		/// </summary>
		IReadOnlyCollection<T> CreateSnapshot();
	}
	
	/// <summary>
	/// A collection that provides change notifications.
	/// </summary>
	public interface IMutableModelCollection<T> : IModelCollection<T>, ICollection<T>
	{
		/// <summary>
		/// Gets the number of items in the collection.
		/// </summary>
		/// <remarks>
		/// Disambiguates between IReadOnlyCollection.Count and ICollection.Count
		/// </remarks>
		new int Count { get; }
		
		/// <summary>
		/// Adds the specified items to the collection.
		/// </summary>
		void AddRange(IEnumerable<T> items);
		
		/// <summary>
		/// Removes all items matching the specified precidate.
		/// </summary>
		int RemoveAll(Predicate<T> predicate);
		
		/// <summary>
		/// Can be used to group several operations into a batch update.
		/// The <see cref="CollectionChanged"/> event will not fire during the batch update;
		/// instead the event will be raised a single time after the batch update finishes.
		/// </summary>
		IDisposable BatchUpdate();
	}
}
