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

	/// <summary>
	/// Helper class for <see cref="IModelCollection.CollectionChanged"/> implementations.
	/// This is necessary because <c>Delegate.Combine</c> does not work with
	/// co-/contravariant delegates.
	/// </summary>
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
		
		public bool ContainsHandlers {
			get {
				return _handlers.Count > 0;
			}
		}
	}
}


