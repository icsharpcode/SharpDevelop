using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.ShortcutsManagement.Data
{
	/// <summary>
	/// Stores one-to-one relationships beween two objects of type T1 and T2
	/// </summary>
	public class MapTable<T1, T2> : IEnumerable<KeyValuePair<T1, T2>> 
	{
		private readonly Dictionary<T1, T2> forwardMaping = new Dictionary<T1, T2>();
		private readonly Dictionary<T2, T1> backwardMaping = new Dictionary<T2, T1>();
		
		/// <summary>
		/// Add new relationship between mapped and mapping object
		/// </summary>
		/// <param name="mappingObject">Mapping object</param>
		/// <param name="mappedObject">Mapping object</param>
		public void Add(T1 mappingObject, T2 mappedObject)
		{
			forwardMaping.Add(mappingObject, mappedObject);
			backwardMaping.Add(mappedObject, mappingObject);
		}
		
		/// <summary>
		/// Tries to find object related to mapping object
		/// </summary>
		/// <param name="mappingObject">Mapping object</param>
		/// <param name="mappedObject">Mapped object</param>
		public void TryMapForward(T1 mappingObject, out T2 mappedObject)
		{
			forwardMaping.TryGetValue(mappingObject, out mappedObject);
		}
		
		/// <summary>
		/// Tries to find object related to mapped object
		/// </summary>
		/// <param name="mappingObject">Mapped object</param>
		/// <param name="mappedObject">Mapping object</param>
		public void TryMapBackward(T2 mappedObject, out T1 mappingObject)
		{
			backwardMaping.TryGetValue(mappedObject, out mappingObject);
		}
		
		
		/// <summary>
		/// Finds object related to mapping object
		/// </summary>
		/// <param name="mappingObject">Mapping object</param>
		/// <returns>Mapped object</returns>
		public T2 MapForward(T1 mappingObject)
		{
			return forwardMaping[mappingObject];
		}
		
		/// <summary>
		/// Finds object related to mapped object
		/// </summary>
		/// <param name="mappingObject">Mapped object</param>
		/// <returns>Mapping object</returns>
		public T1 MapBackward(T2 mappingObject)
		{
			return backwardMaping[mappingObject];
		}
		
		/// <summary>
		/// Removes all relationships from map
		/// </summary>
		public void Clear()
		{
			forwardMaping.Clear();
			backwardMaping.Clear();
		}
		
		/// <inheritdoc />
		public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
		{
			return forwardMaping.GetEnumerator();
		}
		
		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return forwardMaping.GetEnumerator();
		}
	}
}
