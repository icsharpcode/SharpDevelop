// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;

using DebuggerInterop.Core;

namespace DebuggerLibrary
{
	[Serializable]
	public class VariableCollection: ReadOnlyCollectionBase
	{
		public static VariableCollection Empty;

		bool readOnly = false;

		internal VariableCollection()
		{

		}

		static VariableCollection()
		{
			Empty = new VariableCollection();
			Empty.readOnly = true;
		}

		internal void Add(Variable variable)
		{
			if (readOnly) {
				throw new DebuggerException("VariableCollection is marked as readOnly"); 
			}
			System.Diagnostics.Trace.Assert(variable != null);
			if (variable != null) {
				InnerList.Add(variable);
			}
		}

		public Variable this[int index] {
			get {
				return (Variable) InnerList[index];
			}
		}

		public Variable this[string variableName]
		{
			get {
				foreach (Variable v in InnerList) {
					if (v.Name == variableName) {
						return v;
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

		public static VariableCollection Merge(params VariableCollection[] collections)
		{
			VariableCollection mergedCollection = new VariableCollection();
			foreach(VariableCollection collection in collections) {
				foreach(Variable variable in collection) {
					mergedCollection.Add(variable);
				}
			}
			return mergedCollection;
		}
	}
}
