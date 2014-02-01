// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
