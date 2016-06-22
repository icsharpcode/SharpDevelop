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
using System.Linq;
using System.Threading;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.SharpDevelop.Util
{
	/// <summary>
	/// Helper class for adapting from one event to another with a different delegate type.
	/// Maintains thread-safety of add/remove.
	/// Also allows changing the source object; which will remove all handlers from the old source and add them to the new source.
	/// </summary>
	public class EventAdapter<TSource, TSourceDelegate, THandlerDelegate> where TSource : class where TSourceDelegate : class where THandlerDelegate : class
	{
		TSource source;
		readonly Action<TSource, TSourceDelegate> add;
		readonly Action<TSource, TSourceDelegate> remove;
		readonly Func<THandlerDelegate, TSourceDelegate> adapt;
		MultiDictionary<THandlerDelegate, TSourceDelegate> dict = new MultiDictionary<THandlerDelegate, TSourceDelegate>();
		
		public EventAdapter(TSource source, Action<TSource, TSourceDelegate> add, Action<TSource, TSourceDelegate> remove,
		                    Func<THandlerDelegate, TSourceDelegate> adapt)
		{
			if (add == null)
				throw new ArgumentNullException("add");
			if (remove == null)
				throw new ArgumentNullException("remove");
			if (adapt == null)
				throw new ArgumentNullException("adapt");
			//if (typeof(THandlerDelegate).BaseType != typeof(MulticastDelegate))
			//	throw new ArgumentException("THandlerDelegate must be a delegate type");
			this.source = source;
			this.add = add;
			this.remove = remove;
			this.adapt = adapt;
		}
		
		public void Add(THandlerDelegate newHandler)
		{
			if (newHandler == null)
				return;
			TSourceDelegate adapter = adapt(newHandler);
			lock (dict) {
				dict.Add(newHandler, adapter);
				if (source != null)
					add(source, adapter);
			}
		}
		
		public void Remove(THandlerDelegate oldHandler)
		{
			if (oldHandler == null)
				return;
			TSourceDelegate adapter;
			lock (dict) {
				adapter = dict[oldHandler].FirstOrDefault();
				if (adapter == null)
					return;
				dict.Remove(oldHandler, adapter);
				if (source != null)
					remove(source, adapter);
			}
		}
		
		public TSource Source {
			get { return source; }
			set {
				lock (dict) {
					if (source == value)
						return;
					if (source != null) {
						foreach (var adapter in dict.Values)
							remove(source, adapter);
					}
					source = value;
					if (source != null) {
						foreach (var adapter in dict.Values)
							add(source, adapter);
					}
				}
			}
		}
	}
}
