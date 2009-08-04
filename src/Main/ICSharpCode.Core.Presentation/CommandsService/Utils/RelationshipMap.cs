/*
 * Created by SharpDevelop.
 * User: Sergej Andrejev
 * Date: 7/24/2009
 * Time: 6:56 PM
 */
using System;
using System.Collections.Generic;

namespace ICSharpCode.Core.Presentation
{
	public class RelationshipMap<T1, T2>
	{
		private static Dictionary<T1, HashSet<T2>> forwardMap;
		private static Dictionary<T2, HashSet<T1>> backwardMap;
		
		private IEqualityComparer<T1> t1Comparer;
		private IEqualityComparer<T2> t2Comparer;
		
		public RelationshipMap()
		{
			forwardMap = new Dictionary<T1, HashSet<T2>>();
			backwardMap = new Dictionary<T2, HashSet<T1>>();
		}
		
		public RelationshipMap(IEqualityComparer<T1> t1Comparer, IEqualityComparer<T2> t2Comparer)
		{
			this.t1Comparer = t1Comparer;
			this.t2Comparer = t2Comparer;
			
			forwardMap = new Dictionary<T1, HashSet<T2>>(t1Comparer);
			backwardMap = new Dictionary<T2, HashSet<T1>>(t2Comparer);
		}
		
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
		
		public ICollection<T2> MapForward(T1 item) {
			HashSet<T2> mappedValues;
			forwardMap.TryGetValue(item, out mappedValues);
			
			return mappedValues ?? new HashSet<T2>();
		}
		
		public ICollection<T1> MapBackward(T2 item) {
			HashSet<T1> mappedValues;
			backwardMap.TryGetValue(item, out mappedValues);
			
			return mappedValues ?? new HashSet<T1>();
		}
		
		public bool Remove(T1 item1, T2 item2) 
		{
			var forwardBucket = MapForward(item1);
			var forwardRemoveResult = forwardBucket.Remove(item2);
			
			var backwardBucket = MapBackward(item2);
			var backwardRemoveResult = backwardBucket.Remove(item1);
			
			return forwardRemoveResult || backwardRemoveResult;
		}
		
		public void Clear()
		{
			forwardMap.Clear();
			backwardMap.Clear();
		}
	}
}
