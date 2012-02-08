// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.CodeQualityAnalysis.Utility
{
	public class DoubleKeyDictionary<K, T, V> :
        IEnumerable<DoubleKeyPairValue<K, T, V>>,
        IEquatable<DoubleKeyDictionary<K, T, V>> 
	{
        private Dictionary<T, V> innerDictionary;

        public DoubleKeyDictionary()
        {
            OuterDictionary = new Dictionary<K, Dictionary<T, V>>();
        }

        private Dictionary<K, Dictionary<T, V>> OuterDictionary { get; set; }

        public void Add(K key1, T key2, V value)
        {
            if (OuterDictionary.ContainsKey(key1)) {
        		if (innerDictionary.ContainsKey(key2)) {
                    OuterDictionary[key1][key2] = value;
        		}
                else {
                    innerDictionary = OuterDictionary[key1];
                    innerDictionary.Add(key2, value);
                    OuterDictionary[key1] = innerDictionary;
                }
            }
            else {
                innerDictionary = new Dictionary<T, V>();
                innerDictionary[key2] = value;
                OuterDictionary.Add(key1, innerDictionary);
            }
        }

        public V this[K index1, T index2]
        {
            get 
            {
            	Dictionary<T, V> value1;
            	OuterDictionary.TryGetValue(index1, out value1);
            	if (value1 == null)
            		return default(V);
            	
            	V value2;
            	value1.TryGetValue(index2, out value2);
            	if (value2 == null)
            		return default(V);
            	
            	return value2;
            }
            
            set 
            {
                Add(index1, index2, value);
            }
        }

        #region IEnumerable<DoubleKeyPairValue<K,T,V>> Members

        public IEnumerator<DoubleKeyPairValue<K, T, V>> GetEnumerator()
        {
            foreach (KeyValuePair<K, Dictionary<T, V>> outer in OuterDictionary)
                foreach (KeyValuePair<T, V> inner in outer.Value)
                    yield return new DoubleKeyPairValue<K, T, V>(outer.Key, inner.Key, inner.Value);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEquatable<DoubleKeyDictionary<K,T,V>> Members

        public bool Equals(DoubleKeyDictionary<K, T, V> other) 
        {
            if (OuterDictionary.Keys.Count != other.OuterDictionary.Keys.Count)
                return false;

            bool isEqual = true;

            foreach (KeyValuePair<K, Dictionary<T, V>> innerItems in OuterDictionary) {
                if (!other.OuterDictionary.ContainsKey(innerItems.Key))
                    isEqual = false;

                if (!isEqual)
                    break;

                // here we can be sure that the key is in both lists, 
                // but we need to check the contents of the inner dictionary
                Dictionary<T, V> otherInnerDictionary = other.OuterDictionary[innerItems.Key];
                foreach (KeyValuePair<T, V> innerValue in innerItems.Value) {
                    if (!otherInnerDictionary.ContainsValue(innerValue.Value))
                        isEqual = false;
                    if (!otherInnerDictionary.ContainsKey(innerValue.Key))
                        isEqual = false;
                }

                if (!isEqual)
                    break;
            }

            return isEqual;
        }

        #endregion
    }

    public class DoubleKeyPairValue<K, T, V> 
    {
    	public K Key1 { get; set; }
        public T Key2 { get; set; }
        public V Value { get; set; }
    	
        public DoubleKeyPairValue(K key1, T key2, V value) {
            Key1 = key1;
            Key2 = key2;
            Value = value;
        }
    	
        public override string ToString()
        {
            return Key1.ToString() + " - " + Key2.ToString() + " - " + Value.ToString();
        }
    }
}
