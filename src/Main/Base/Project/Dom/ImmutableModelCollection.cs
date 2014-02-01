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
