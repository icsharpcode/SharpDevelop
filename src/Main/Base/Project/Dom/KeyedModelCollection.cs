// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)using System;

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



