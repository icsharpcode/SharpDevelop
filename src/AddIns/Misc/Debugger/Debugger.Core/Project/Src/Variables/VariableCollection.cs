// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

using Debugger.Interop.CorDebug;

namespace Debugger
{
	[Serializable]
	public class VariableCollection: ReadOnlyCollectionBase
	{
		public static VariableCollection Empty;
		
		bool readOnly = false;
		
		public event EventHandler<VariableEventArgs> VariableAdded;
		public event EventHandler<VariableEventArgs> VariableRemoved;
		internal event EventHandler Updating;
		
		internal VariableCollection()
		{
		
		}
		
		/// <summary>
		/// Creates new collection and fills it by calling 'updating' delegate
		/// </summary>
		/// <param name="updating"></param>
		/// <returns></returns>
		internal VariableCollection(EventHandler updating)
		{
			this.Updating += updating;
			this.Update();
		}
		
		static VariableCollection()
		{
			Empty = new VariableCollection();
			Empty.readOnly = true;
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
		
		internal void Add(Variable variable)
		{
			if (readOnly) {
				throw new DebuggerException("VariableCollection is marked as read only"); 
			}
			if (variable != null) {
				InnerList.Add(variable);
				OnVariableAdded(new VariableEventArgs(variable));
			}
		}
		
		internal void Remove(Variable variable)
		{
			if (readOnly) {
				throw new DebuggerException("VariableCollection is marked as read only"); 
			}
			if (variable != null) {
				InnerList.Remove(variable);
				OnVariableRemoved(new VariableEventArgs(variable));
			}
		}
		
		/// <summary>
		/// Removes all variables from collection and resets the updating function
		/// </summary>
		internal void Clear()
		{
			foreach(Variable variable in InnerList) {
				Remove(variable);
			}
			Updating = null;
		}
		
		public Variable this[int index] {
			get {
				return (Variable) InnerList[index];
			}
		}

		public Variable this[string variableName] {
			get {
				int index = variableName.IndexOf('.');
				if (index != -1) {
					string rootVariable = variableName.Substring(0, index);
					string subVariable = variableName.Substring(index + 1);
					return this[rootVariable].SubVariables[subVariable];
				} else {
					foreach (Variable v in InnerList) {
						if (v.Name == variableName) {
							return v;
						}
					}
				}

				throw new DebuggerException("Variable \"" + variableName + "\" is not in collection");
			}
		}
		
		public void Update()
		{
			OnUpdating(EventArgs.Empty);
		}
		
		/// <summary>
		/// Updates the given collections and changes the state of this collection so that it matches the given collections.
		/// </summary>
		/// <param name="collections"></param>
		public void UpdateTo(params VariableCollection[] collections)
		{
			VariableCollection mergedCollection = VariableCollection.Merge(collections);
			
			mergedCollection.Update();
			
			// Update existing variables
			foreach(Variable variable in mergedCollection) {
				if (this.Contains(variable.Name)) {
					this.Remove(this[variable.Name]);
					this.Add(variable);
				}
			}
			
			// Add new variables
			foreach(Variable variable in mergedCollection) {
				if (!this.Contains(variable.Name)) {
					this.Add(variable);
				}
			}
			
			// Remove variables that are not in merged collection
			foreach(Variable variable in this) {
				if (!mergedCollection.Contains(variable.Name)) {
					this.Remove(variable);
				}
			}
		}
		
		protected virtual void OnVariableAdded(VariableEventArgs e)
		{
			if (VariableAdded != null) {
				VariableAdded(this, e);
			}
		}
		
		protected virtual void OnVariableRemoved(VariableEventArgs e)
		{
			if (VariableRemoved != null) {
				VariableRemoved(this, e);
			}
		}
		
		protected virtual void OnUpdating(EventArgs e)
		{
			if (Updating != null) {
				Updating(this, e);
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
			VariableCollection newCollection = new VariableCollection();
			newCollection.MergeWith(collections);
			return newCollection;
		}
		
		public void MergeWith(params VariableCollection[] collections)
		{
			// Add items of subcollections and ensure the stay in sync
			foreach(VariableCollection collection in collections) {
				foreach(Variable variable in collection) {
					this.Add(variable);
				}
				collection.VariableAdded += delegate(object sender, VariableEventArgs e) {
					this.Add(e.Variable);
				};
				collection.VariableRemoved += delegate(object sender, VariableEventArgs e) {
					this.Remove(e.Variable);
				};
			}
			
			// Update subcollections at update
			this.Updating += delegate {
				foreach(VariableCollection collection in collections) {
					collection.Update();
				};
			};
		}
	}
}
