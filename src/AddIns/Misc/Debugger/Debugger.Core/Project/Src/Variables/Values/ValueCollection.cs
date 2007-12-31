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
	public class ValueCollection: DebuggerObject, IEnumerable<Value>, IEnumerable
	{
		internal static ValueCollection Empty = new ValueCollection(new Value[0]);
		
		List<Value> list = new List<Value>();
		Dictionary<string, List<Value>> hashtable = new Dictionary<string, List<Value>>();
		
		IEnumerator<Value> IEnumerable<Value>.GetEnumerator()
		{
			foreach(Value namedValue in list) {
				yield return namedValue;
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach(Value namedValue in list) {
				yield return namedValue;
			}
		}
		
		internal ValueCollection(IEnumerable<Value> namedValues)
		{
			foreach(Value namedValue in namedValues) {
				string name = namedValue.Name;
				if (hashtable.ContainsKey(name)) {
					hashtable[name].Add(namedValue);
				} else {
					hashtable[name] = new List<Value>(new Value[] {namedValue});
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
		public Value this[int i] {
			get {
				return list[i];
			}
		}
		
		/// <summary>
		/// Gets a value by its name.
		/// </summary>
		public Value this[string variableName] {
			get {
				if (hashtable.ContainsKey(variableName)) {
					foreach(Value val in hashtable[variableName]) {
						return val;
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
