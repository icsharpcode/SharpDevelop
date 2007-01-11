// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Debugger
{
	/// <summary>
	/// An enumerable collection of values accessible by name.
	/// </summary>
	public class NamedValueCollection: DebuggerObject, IEnumerable<NamedValue>, IEnumerable
	{
		internal static NamedValueCollection Empty = new NamedValueCollection(new NamedValue[0]);
		
		List<NamedValue> list = new List<NamedValue>();
		Dictionary<string, List<NamedValue>> hashtable = new Dictionary<string, List<NamedValue>>();
		
		IEnumerator<NamedValue> IEnumerable<NamedValue>.GetEnumerator()
		{
			foreach(NamedValue namedValue in list) {
				yield return namedValue;
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach(NamedValue namedValue in list) {
				yield return namedValue;
			}
		}
		
		internal NamedValueCollection(IEnumerable<NamedValue> namedValues)
		{
			foreach(NamedValue namedValue in namedValues) {
				string name = namedValue.Name;
				if (hashtable.ContainsKey(name)) {
					hashtable[name].Add(namedValue);
				} else {
					hashtable[name] = new List<NamedValue>(new NamedValue[] {namedValue});
				}
				list.Add(namedValue);
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether the collection contains a
		/// value with a given name
		/// </summary>
		public bool Contains(string name)
		{
			return hashtable.ContainsKey(name);
		}
		
		/// <summary>
		/// Gets number of <see cref="Debugger.NamedValue">named values</see> contained in the collection
		/// </summary>
		public int Count {
			get {
				return list.Count;
			}
		}
		
		/// <summary>
		/// Gets a value by index
		/// </summary>
		public NamedValue this[int i] {
			get {
				return list[i];
			}
		}
		
		/// <summary>
		/// Gets a value by its name.
		/// </summary>
		public NamedValue this[string variableName] {
			get {
				if (hashtable.ContainsKey(variableName)) {
					foreach(NamedValue namedValue in hashtable[variableName]) {
						return namedValue;
					}
				}
				
//				int index = variableName.IndexOf('.');
//				if (index != -1) {
//					string rootVariable = variableName.Substring(0, index);
//					string subVariable = variableName.Substring(index + 1);
//					return this[rootVariable].Value.SubVariables[subVariable];
//				}
				
				throw new DebuggerException("Variable \"" + variableName + "\" is not in collection");
			}
		}
	}
}
