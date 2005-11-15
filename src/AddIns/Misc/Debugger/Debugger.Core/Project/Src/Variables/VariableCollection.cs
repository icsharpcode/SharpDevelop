// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using Debugger.Interop.CorDebug;

namespace Debugger
{
	[Serializable]
	public class VariableCollection: ReadOnlyCollectionBase
	{
		NDebugger debugger;
		
		public static VariableCollection Empty;
		
		bool readOnly = false;
		
		public event EventHandler<VariableEventArgs> VariableAdded;
		public event EventHandler<VariableEventArgs> VariableRemoved;
		internal event EventHandler<VariableCollectionEventArgs> Updating;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		internal VariableCollection(NDebugger debugger)
		{
			this.debugger = debugger;
		}
		
		/// <summary>
		/// Creates new collection and fills it by calling 'updating' delegate
		/// </summary>
		/// <param name="updating"></param>
		/// <returns></returns>
		internal VariableCollection(NDebugger debugger, EventHandler<VariableCollectionEventArgs> updating): this(debugger)
		{
			this.Updating += updating;
			this.Update();
		}
		
		static VariableCollection()
		{
			Empty = new VariableCollection(null);
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
			InnerList.Clear();
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
					return this[rootVariable].Value.SubVariables[subVariable];
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
			OnUpdating();
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
					this[variable.Name].Value = variable.Value;
				}
			}
			
			// Add new variables
			foreach(Variable variable in mergedCollection) {
				if (!this.Contains(variable.Name)) {
					this.Add(variable);
				}
			}
			
			// Remove variables that are not in merged collection
			List<Variable> toBeRemoved = new List<Variable>(); // We can NOT modify collection which are using!!!
			foreach(Variable variable in this) {
				if (!mergedCollection.Contains(variable.Name)) {
					toBeRemoved.Add(variable);
				}
			}
			foreach(Variable variable in toBeRemoved) {
				this.Remove(variable);
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
		
		protected virtual void OnUpdating()
		{
			if (Updating != null) {
				Updating(this, new VariableCollectionEventArgs(this));
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
			if (collections.Length == 0) throw new ArgumentException("Can not have lenght of 0", "collections");
			VariableCollection newCollection = new VariableCollection(collections[0].Debugger);
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
