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
		
		string name;
		string val;
		IEnumerable<VariableCollection> subCollectionsEnum;
		IEnumerable<Variable> collectionEnum;
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public IEnumerator<Variable> GetEnumerator()
		{
			return Items.GetEnumerator();
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string Value {
			get {
				return val;
			}
		}
		
		public IEnumerable<VariableCollection> SubCollections {
			get {
				return subCollectionsEnum;
			}
		}
		
		public IEnumerable<Variable> Items {
			get {
				return collectionEnum;
			}
		}
		
		public bool IsEmpty {
			get {
				foreach(VariableCollection col in SubCollections) {
					if (!col.IsEmpty) return false;
				}
				return !Items.GetEnumerator().MoveNext();
			}
		}
		
		internal VariableCollection(IEnumerable<Variable> collectionEnum)
			:this(String.Empty, String.Empty, new VariableCollection[0], collectionEnum)
		{
		}
		
		public VariableCollection(string name, string val, IEnumerable<VariableCollection> subCollectionsEnum, IEnumerable<Variable> collectionEnum)
		{
			this.name = name;
			this.val = val;
			this.subCollectionsEnum = subCollectionsEnum;
			this.collectionEnum = collectionEnum;
		}
		
		
		public virtual Variable this[string variableName] {
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
				return null;
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
