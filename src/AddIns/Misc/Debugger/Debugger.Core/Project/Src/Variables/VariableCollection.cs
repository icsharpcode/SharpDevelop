// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	[Serializable]
	public class VariableCollection: RemotingObjectBase, IEnumerable<Variable>
	{
		public static VariableCollection Empty = new VariableCollection(new Variable[] {});
		
		IEnumerable<Variable> collectionEnum;
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public IEnumerator<Variable> GetEnumerator()
		{
			return collectionEnum.GetEnumerator();
		}
		
		internal VariableCollection(IEnumerable<Variable> collectionEnum)
		{
			this.collectionEnum = collectionEnum;
		}
		
		public Variable this[string variableName] {
			get {
				int index = variableName.IndexOf('.');
				if (index != -1) {
					string rootVariable = variableName.Substring(0, index);
					string subVariable = variableName.Substring(index + 1);
					return this[rootVariable].Value.SubVariables[subVariable];
				} else {
					foreach (Variable v in this) {
						if (v.Name == variableName) return v;
					}
				}
				throw new DebuggerException("Variable \"" + variableName + "\" is not in collection");
			}
		}
		
		public override string ToString() {
			string txt = "";
			foreach(Variable v in this) {
				txt += v.ToString() + "\n";
			}
			return txt;
		}
	}
}
