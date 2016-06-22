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
	public class KeyedModelCollection<TKey, TValue> : SimpleModelCollection<TValue>
	{
		Func<TValue, TKey> getKeyForValue;
		Dictionary<TKey, TValue> dict;
		
		public KeyedModelCollection(Func<TValue, TKey> getKeyForValue, IEqualityComparer<TKey> comparer = null)
		{
			if (getKeyForValue == null)
				throw new ArgumentNullException("getKeyForValue");
			this.getKeyForValue = getKeyForValue;
			this.dict = new Dictionary<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default);
		}
		
		protected override void OnAdd(TValue item)
		{
			base.OnAdd(item);
			dict.Add(getKeyForValue(item), item);
		}
		
		protected override void OnRemove(TValue item)
		{
			base.OnRemove(item);
			dict.Remove(getKeyForValue(item));
		}
		
		protected override void ValidateItem(TValue item)
		{
			base.ValidateItem(item);
			if (dict.ContainsKey(getKeyForValue(item)))
				throw new ArgumentException("An item with the same key has already been added to the collection.");
		}
		
		public TValue this[TKey key] {
			get { return dict[key]; }
		}
		
		public bool TryGetValue(TKey key, out TValue value)
		{
			return dict.TryGetValue(key, out value);
		}
	}
}
