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
