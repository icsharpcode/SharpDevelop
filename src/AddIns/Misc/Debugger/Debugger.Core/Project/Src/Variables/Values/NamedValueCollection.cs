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
	public class NamedValueCollection: RemotingObjectBase, IEnumerable<NamedValue>, IEnumerable
	{
		internal static NamedValueCollection Empty = new NamedValueCollection(new NamedValue[0]);
		
		Dictionary<string, List<NamedValue>> collection = new Dictionary<string, List<NamedValue>>();
		
		IEnumerator<NamedValue> IEnumerable<NamedValue>.GetEnumerator()
		{
			foreach(KeyValuePair<string, List<NamedValue>> kvp in collection) {
				foreach(NamedValue namedValue in kvp.Value) {
					yield return namedValue;
				}
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<NamedValue>)this).GetEnumerator();
		}
		
		internal NamedValueCollection(IEnumerable<NamedValue> namedValues)
		{
			foreach(NamedValue namedValue in namedValues) {
				string name = namedValue.Name;
				if (collection.ContainsKey(name)) {
					collection[name].Add(namedValue);
				} else {
					collection[name] = new List<NamedValue>(new NamedValue[] {namedValue});
				}
			}
		}
		
		/// <summary>
		/// Gets a value by its name.
		/// </summary>
		public virtual NamedValue this[string variableName] {
			get {
				if (collection.ContainsKey(variableName)) {
					foreach(NamedValue namedValue in collection[variableName]) {
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
