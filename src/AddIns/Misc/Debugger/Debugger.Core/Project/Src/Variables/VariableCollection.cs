// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

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

		public bool Contains(Variable variable)
		{
			foreach (Variable v in InnerList) {
				if (v == variable) {
					return true;
				}
			}
			return false;
		}

		public bool Contains(string variableName)
		{
			foreach (Variable v in InnerList) {
				if (v.Name == variableName) {
					return true;
				}
			}
			return false;
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
			if (variable != null) {
				InnerList.Add(variable);
			}
		}

		public Variable this[int index] {
			get {
				return (Variable) InnerList[index];
			}
		}

		public Variable this[string variableName] {
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
