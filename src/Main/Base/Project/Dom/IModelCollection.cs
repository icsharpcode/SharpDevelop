// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Event handler for the <see cref="IModelCollection{T}.CollectionChanged"/> event.
	/// </summary>
	/// <remarks>
	/// We don't use the classic 'EventArgs' model for this event, because a EventArgs-class couldn't be covariant.
	/// </remarks>
	public delegate void ModelCollectionChangedEventHandler<in T>(IReadOnlyCollection<T> removedItems, IReadOnlyCollection<T> addedItems);
	
	public class ModelCollectionChangedEvent<T>
	{
		List<ModelCollectionChangedEventHandler<T>> _handlers = new List<ModelCollectionChangedEventHandler<T>>();
		
		public void AddHandler(ModelCollectionChangedEventHandler<T> handler)
		{
			if (handler != null)
				_handlers.Add(handler);
		}
		
		public void RemoveHandler(ModelCollectionChangedEventHandler<T> handler)
		{
			_handlers.Remove(handler);
		}
		
		public void Fire(IReadOnlyCollection<T> removedItems, IReadOnlyCollection<T> addedItems)
		{
			foreach (var handler in _handlers.ToArray()) {
				handler(removedItems, addedItems);
			}
		}
		
		public bool ContainsHandlers
		{
			get {
				return _handlers.Count > 0;
			}
		}
	}
	
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
