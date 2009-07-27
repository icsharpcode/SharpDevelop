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
		private static Dictionary<T1, HashSet<T2>> forwardMap = new Dictionary<T1, HashSet<T2>>();
		private static Dictionary<T2, HashSet<T1>> backwardMap = new Dictionary<T2, HashSet<T1>>();
		
		public bool Add(T1 item1, T2 item2) 
		{
			if(!forwardMap.ContainsKey(item1)){
				forwardMap.Add(item1, new HashSet<T2>());
			}
			
			if(!backwardMap.ContainsKey(item2)) {
				backwardMap.Add(item2, new HashSet<T1>());
			}
			
			var forwardRemoveResult = forwardMap[item1].Add(item2);
			var backwardRemoveResult = backwardMap[item2].Add(item1);
			
			return forwardRemoveResult || backwardRemoveResult;
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
			var forwardRemoveResult = MapForward(item1).Remove(item2);
			var backwardRemoveResult = MapBackward(item2).Remove(item1);
			
			return forwardRemoveResult || backwardRemoveResult;
		}
		
		public void Clear()
		{
			forwardMap.Clear();
			backwardMap.Clear();
		}
	}
}
