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
//	public class NamedValueCollection: RemotingObjectBase, IEnumerable<NamedValue>
//	{
//		public static NamedValueCollection Empty = new NamedValueCollection(new NamedValue[0]);
//		
//		string name;
//		string val;
//		IEnumerable<NamedValueCollection> subCollectionsEnum;
//		IEnumerable<NamedValue> collectionEnum;
//		
//		IEnumerator IEnumerable.GetEnumerator()
//		{
//			return GetEnumerator();
//		}
//		
//		public IEnumerator<NamedValue> GetEnumerator()
//		{
//			return Items.GetEnumerator();
//		}
//		
//		public string Name {
//			get {
//				return name;
//			}
//		}
//		
//		public string Value {
//			get {
//				return val;
//			}
//		}
//		
//		public IEnumerable<NamedValueCollection> SubCollections {
//			get {
//				return subCollectionsEnum;
//			}
//		}
//		
//		public IEnumerable<NamedValue> Items {
//			get {
//				return collectionEnum;
//			}
//		}
//		
//		public bool IsEmpty {
//			get {
//				return !SubCollections.GetEnumerator().MoveNext() &&
//				       !Items.GetEnumerator().MoveNext();
//			}
//		}
//		
//		internal NamedValueCollection(IEnumerable<NamedValue> collectionEnum)
//			:this(String.Empty, String.Empty, new VariableCollection[0], collectionEnum)
//		{
//		}
//		
//		public NamedValueCollection(string name, string val):this(name, val, null, null)
//		{
//		}
//		
//		public NamedValueCollection(string name, string val, IEnumerable<NamedValueCollection> subCollectionsEnum, IEnumerable<NamedValue> collectionEnum)
//		{
//			this.name = name;
//			this.val = val;
//			this.subCollectionsEnum = subCollectionsEnum ?? new VariableCollection[0];
//			this.collectionEnum = collectionEnum ?? new Variable[0];
//		}
//		
//		
//		public virtual NamedValue this[string variableName] {
//			get {
//				int index = variableName.IndexOf('.');
//				if (index != -1) {
//					string rootVariable = variableName.Substring(0, index);
//					string subVariable = variableName.Substring(index + 1);
//					return this[rootVariable].Value.SubVariables[subVariable];
//				} else {
//					foreach (Variable v in this) {
//						if (v.Name == variableName) return v;
//					}
//				}
//				throw new DebuggerException("Variable \"" + variableName + "\" is not in collection");
//			}
//		}
//		
//		public override string ToString()
//		{
//			string txt = "";
//			foreach(Variable v in this) {
//				txt += v.ToString() + "\n";
//			}
//			return txt;
//		}
//	}
}
