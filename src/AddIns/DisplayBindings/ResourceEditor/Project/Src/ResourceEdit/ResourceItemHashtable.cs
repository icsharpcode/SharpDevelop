using System;
using System.Collections;

namespace ResourceEditor
{
	public class ResourceItemHashtable : IDictionary, ICollection, IEnumerable, ICloneable
		{
			protected Hashtable innerHash;
			
			#region "Constructors"
			public  ResourceItemHashtable()
			{
				innerHash = new Hashtable();
			}
			
			public ResourceItemHashtable(ResourceItemHashtable original)
			{
				innerHash = new Hashtable (original.innerHash);
			}
			
			public ResourceItemHashtable(IDictionary dictionary)
			{
				innerHash = new Hashtable (dictionary);
			}
			
			public ResourceItemHashtable(int capacity)
			{
				innerHash = new Hashtable(capacity);
			}
			
			public ResourceItemHashtable(IDictionary dictionary, float loadFactor)
			{
				innerHash = new Hashtable(dictionary, loadFactor);
			}
			
			public ResourceItemHashtable(IHashCodeProvider codeProvider, IComparer comparer)
			{
				innerHash = new Hashtable (codeProvider, comparer);
			}
			
			public ResourceItemHashtable(int capacity, int loadFactor)
			{
				innerHash = new Hashtable(capacity, loadFactor);
			}
			
			public ResourceItemHashtable(IDictionary dictionary, IHashCodeProvider codeProvider, IComparer comparer)
			{
				innerHash = new Hashtable (dictionary, codeProvider, comparer);
			}
			
			public ResourceItemHashtable(int capacity, IHashCodeProvider codeProvider, IComparer comparer)
			{
				innerHash = new Hashtable (capacity, codeProvider, comparer);
			}
			
			public ResourceItemHashtable(IDictionary dictionary, float loadFactor, IHashCodeProvider codeProvider, IComparer comparer)
			{
				innerHash = new Hashtable (dictionary, loadFactor, codeProvider, comparer);
			}
			
			public ResourceItemHashtable(int capacity, float loadFactor, IHashCodeProvider codeProvider, IComparer comparer)
			{
				innerHash = new Hashtable (capacity, loadFactor, codeProvider, comparer);
			}
			#endregion
	
			#region Implementation of IDictionary
			public ResourceItemHashtableEnumerator GetEnumerator()
			{
				return new ResourceItemHashtableEnumerator(this);
			}
			
			System.Collections.IDictionaryEnumerator IDictionary.GetEnumerator()
			{
				return new ResourceItemHashtableEnumerator(this);
			}
			
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
			
			public void Remove(string key)
			{
				innerHash.Remove (key);
			}
			
			void IDictionary.Remove(object key)
			{
				Remove ((string)key);
			}
			
			public bool Contains(string key)
			{
				return innerHash.Contains(key);
			}
			
			bool IDictionary.Contains(object key)
			{
				return Contains((string)key);
			}
			
			public void Clear()
			{
				innerHash.Clear();		
			}
			
			public void Add(string key, ResourceItem value)
			{
				innerHash.Add (key, value);
			}
			
			void IDictionary.Add(object key, object value)
			{
				Add ((string)key, (ResourceItem)value);
			}
			
			public bool IsReadOnly
			{
				get
				{
					return innerHash.IsReadOnly;
				}
			}
			
			public ResourceItem this[string key]
			{
				get
				{
					return (ResourceItem) innerHash[key];
				}
				set
				{
					innerHash[key] = value;
				}
			}
			
			object IDictionary.this[object key]
			{
				get
				{
					return this[(string)key];
				}
				set
				{
					this[(string)key] = (ResourceItem)value;
				}
			}
	        	
			public System.Collections.ICollection Values
			{
				get
				{
					return innerHash.Values;
				}
			}
			
			public System.Collections.ICollection Keys
			{
				get
				{
					return innerHash.Keys;
				}
			}
			
			public bool IsFixedSize
			{
				get
				{
					return innerHash.IsFixedSize;
				}
			}
			#endregion
			
			#region Implementation of ICollection
			public void CopyTo(System.Array array, int index)
			{
				innerHash.CopyTo (array, index);
			}
			
			public bool IsSynchronized
			{
				get
				{
					return innerHash.IsSynchronized;
				}
			}
			
			public int Count
			{
				get
				{
					return innerHash.Count;
				}
			}
			
			public object SyncRoot
			{
				get
				{
					return innerHash.SyncRoot;
				}
			}
			#endregion
			
			#region Implementation of ICloneable
			public ResourceItemHashtable Clone()
			{
				ResourceItemHashtable clone = new ResourceItemHashtable();
				clone.innerHash = (Hashtable) innerHash.Clone();
				
				return clone;
			}
			
			object ICloneable.Clone()
			{
				return Clone();
			}
			#endregion
			
			#region "HashTable Methods"
			public bool ContainsKey (string key)
			{
				return innerHash.ContainsKey(key);
			}
			
			public bool ContainsValue (ResourceItem value)
			{
				return innerHash.ContainsValue(value);
			}
			
			public static ResourceItemHashtable Synchronized(ResourceItemHashtable nonSync)
			{
				ResourceItemHashtable sync = new ResourceItemHashtable();
				sync.innerHash = Hashtable.Synchronized(nonSync.innerHash);
	
				return sync;
			}
			#endregion
	
			internal Hashtable InnerHash
			{
				get
				{
					return innerHash;
				}
			}
		}
		
		public class ResourceItemHashtableEnumerator : IDictionaryEnumerator
		{
			private IDictionaryEnumerator innerEnumerator;
			
			internal ResourceItemHashtableEnumerator (ResourceItemHashtable enumerable)
			{
				innerEnumerator = enumerable.InnerHash.GetEnumerator();
			}
			
			#region Implementation of IDictionaryEnumerator
			public string Key
			{
				get
				{
					return (string)innerEnumerator.Key;
				}
			}
			
			object IDictionaryEnumerator.Key
			{
				get
				{
					return Key;
				}
			}
			
			public ResourceItem Value
			{
				get
				{
					return (ResourceItem)innerEnumerator.Value;
				}
			}
			
			object IDictionaryEnumerator.Value
			{
				get
				{
					return Value;
				}
			}
			
			public System.Collections.DictionaryEntry Entry
			{
				get
				{
					return innerEnumerator.Entry;
				}
			}
			#endregion
			
			#region Implementation of IEnumerator
			public void Reset()
			{
				innerEnumerator.Reset();
			}
			
			public bool MoveNext()
			{
				return innerEnumerator.MoveNext();
			}
			
			public object Current
			{
				get
				{
					return innerEnumerator.Current;
				}
			}
			#endregion
		}
}
