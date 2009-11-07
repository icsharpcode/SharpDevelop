using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Handles many-to-many relationship between instances of T1 and T2 type
	/// </summary>
	public class RelationshipMap<T1, T2>
	{
		private static Dictionary<T1, HashSet<T2>> forwardMap;
		private static Dictionary<T2, HashSet<T1>> backwardMap;
		
		private IEqualityComparer<T1> t1Comparer;
		private IEqualityComparer<T2> t2Comparer;
		
		/// <summary>
		/// Creates new instance of <see cref="RelationshipMap" />
		/// </summary>
		public RelationshipMap()
		{
			forwardMap = new Dictionary<T1, HashSet<T2>>();
			backwardMap = new Dictionary<T2, HashSet<T1>>();
		}
		
		/// <summary>
		/// Gets a thread-safe dictionary of forward conversions
		/// </summary>
		public IDictionary<T1, ICollection<T2>> ForwardMap 
		{
			get {
				var dict = new Dictionary<T1, ICollection<T2>>();
				foreach(var pair in forwardMap) {
					dict.Add(pair.Key, new List<T2>(pair.Value));
				}
				
				return dict;
			}
		}
		
		/// <summary>
		/// Gets a thread-safe dictionary of backward conversions
		/// </summary>
		public IDictionary<T2, ICollection<T1>> BackwardMap 
		{
			get {
				var dict = new Dictionary<T2, ICollection<T1>>();
				foreach(var pair in backwardMap) {
					dict.Add(pair.Key, new List<T1>(pair.Value));
				}
				
				return dict;
			}
		}
		
		/// <summary>
		/// Creates new instance of <see cref="RelationshipMap" />
		/// </summary>
		/// <param name="t1Comparer">T1 comparer</param>
		/// <param name="t2Comparer">T2 comparer</param>
		public RelationshipMap(IEqualityComparer<T1> t1Comparer, IEqualityComparer<T2> t2Comparer)
		{
			this.t1Comparer = t1Comparer;
			this.t2Comparer = t2Comparer;
			
			forwardMap = new Dictionary<T1, HashSet<T2>>(t1Comparer);
			backwardMap = new Dictionary<T2, HashSet<T1>>(t2Comparer);
		}
		
		/// <summary>
		/// Add new two-way association between item of type T1 and item of type T2
		/// </summary>
		/// <param name="item1">Item of type T1</param>
		/// <param name="item2">Item of type T2</param>
		/// <returns>Returns <code>true</code> in case relationship was successfully created; otherwise returns <code>false</code></returns>
		public bool Add(T1 item1, T2 item2) 
		{
			if(!forwardMap.ContainsKey(item1)){
				forwardMap.Add(item1, new HashSet<T2>(t2Comparer));
			}
			
			if(!backwardMap.ContainsKey(item2)) {
				backwardMap.Add(item2, new HashSet<T1>(t1Comparer));
			}
			
			var forwardRemoveResult = forwardMap[item1].Add(item2);
			var backwardRemoveResult = backwardMap[item2].Add(item1);
			
			return forwardRemoveResult || backwardRemoveResult;
			//return forwardRemoveResult;
		}
		
		/// <summary>
		/// Selects all items mapped to provided item
		/// </summary>
		/// <param name="item">Mapping item</param>
		/// <returns>All items mapped to provided item</returns>
		public ICollection<T2> MapForward(T1 item) {
			HashSet<T2> mappedValues;
			forwardMap.TryGetValue(item, out mappedValues);
			
			return mappedValues ?? new HashSet<T2>();
		}
		
		/// <summary>
		/// Select all items mapped backward to provided item
		/// </summary>
		/// <param name="item">Mapping item</param>
		/// <returns>All items mapped to provided item</returns>
		public ICollection<T1> MapBackward(T2 item) {
			HashSet<T1> mappedValues;
			backwardMap.TryGetValue(item, out mappedValues);
			
			return mappedValues ?? new HashSet<T1>();
		}
		
		/// <summary>
		/// Removes association between item of type T1 and item of type T2
		/// </summary>
		/// <param name="item1">Item of type T1</param>
		/// <param name="item2">Item of type T2</param>
		/// <returns>Returns <code>true</code> if association was successfully removed; otherwise <code>false</code></returns>
		public bool Remove(T1 item1, T2 item2) 
		{
			var forwardBucket = MapForward(item1);
			var forwardRemoveResult = forwardBucket.Remove(item2);
			
			var backwardBucket = MapBackward(item2);
			var backwardRemoveResult = backwardBucket.Remove(item1);
			
			return forwardRemoveResult || backwardRemoveResult;
		}
		
		/// <summary>
		/// Remove all associations from collection
		/// </summary>
		public void Clear()
		{
			forwardMap.Clear();
			backwardMap.Clear();
		}
		
	}
}
