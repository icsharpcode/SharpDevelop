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
				variable.OnValueRemovedFromCollection(new VariableCollectionEventArgs(this));
			}
		}
		
		/// <summary>
		/// Removes all variables from collection and resets the updating function
		/// </summary>
		internal void Clear()
		{
			while(this.Count > 0) {
				this.Remove(this[0]);
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
			OnUpdating();
		}
		
		public void UpdateTo(IEnumerable<Variable> newVariables)
		{
			ArrayList toBeRemoved = (ArrayList)this.InnerList.Clone();
			
			foreach(Variable newVariable in newVariables) {
				if (this.Contains(newVariable.Name)) {
					Variable oldVariable = this[newVariable.Name];
					// HACK: Realy bad object-oriented design!!!
					// Trasfer the new variable into the old one
					if (oldVariable != newVariable) {
						oldVariable.valueGetter = newVariable.valueGetter;
						oldVariable.cachedValue = null;
						if (newVariable is ClassVariable && oldVariable is ClassVariable) {
							((ClassVariable)oldVariable).isPublic = ((ClassVariable)oldVariable).isPublic;
							((ClassVariable)oldVariable).isStatic = ((ClassVariable)oldVariable).isStatic;
						}
						if (newVariable is PropertyVariable) {
							newVariable.ValueChanged += delegate { oldVariable.OnValueChanged(); };
						}
					}
					// Keep the variable in the list
					toBeRemoved.Remove(this[newVariable.Name]);
				} else {
					// Add new variable
					this.Add(newVariable);
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
	}
}
