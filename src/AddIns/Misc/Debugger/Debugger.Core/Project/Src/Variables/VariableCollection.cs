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
		internal VariableCollection()
		{

		}

		internal void Add(Variable variable)
		{
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

				throw new UnableToGetPropertyException(this, "this[string]", "Variable \"" + variableName + "\" is not in collection");
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
