using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.ShortcutsManagement.Data
{
	public class MapTable<T1, T2> : IEnumerable<KeyValuePair<T1, T2>> 
	{
		private readonly Dictionary<T1, T2> forwardMaping = new Dictionary<T1, T2>();
		
		private readonly Dictionary<T2, T1> backwardMaping = new Dictionary<T2, T1>();
		
		public void Add(T1 mappingObject, T2 mappedObject)
		{
			forwardMaping.Add(mappingObject, mappedObject);
			backwardMaping.Add(mappedObject, mappingObject);
		}
		
		public void TryMapForward(T1 mappingObject, out T2 mappedObject)
		{
			forwardMaping.TryGetValue(mappingObject, out mappedObject);
		}
		
		public void TryMapBackward(T2 mappingObject, out T1 mappedObject)
		{
			backwardMaping.TryGetValue(mappingObject, out mappedObject);
		}
		
		public T2 MapForward(T1 mappingObject)
		{
			return forwardMaping[mappingObject];
		}
		
		public T1 MapBackward(T2 mappingObject)
		{
			return backwardMaping[mappingObject];
		}
		
		public void Clear()
		{
			forwardMaping.Clear();
			backwardMaping.Clear();
		}
		
		public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
		{
			return forwardMaping.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return forwardMaping.GetEnumerator();
		}
	}
}
