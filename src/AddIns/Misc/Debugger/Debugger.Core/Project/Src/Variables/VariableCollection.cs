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
				return !SubCollections.GetEnumerator().MoveNext() &&
				       !Items.GetEnumerator().MoveNext();
			}
		}
		
		internal VariableCollection(IEnumerable<Variable> collectionEnum)
			:this(String.Empty, String.Empty, new VariableCollection[0], collectionEnum)
		{
		}
		
		public VariableCollection(string name, string val):this(name, val, null, null)
		{
		}
		
		public VariableCollection(string name, string val, IEnumerable<VariableCollection> subCollectionsEnum, IEnumerable<Variable> collectionEnum)
		{
			this.name = name;
			this.val = val;
			if (subCollectionsEnum != null) {
				this.subCollectionsEnum = subCollectionsEnum;
			} else {
				this.subCollectionsEnum = new VariableCollection[0];
			}
			if (collectionEnum != null) {
				this.collectionEnum = collectionEnum;
			} else {
				this.collectionEnum = new Variable[0];
			}
		}
		
		
		public virtual Variable this[string variableName] {
			get {
				int index = variableName.IndexOf('.');
				if (index != -1) {
					string rootVariable = variableName.Substring(0, index);
					string subVariable = variableName.Substring(index + 1);
					return this[rootVariable].ValueProxy.SubVariables[subVariable];
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
