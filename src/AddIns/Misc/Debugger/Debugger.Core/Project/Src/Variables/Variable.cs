// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	public class Variable: RemotingObjectBase
	{
		protected NDebugger debugger;
		
		string name;
		Value val;
		VariableCollection subVariables;
		
		public event EventHandler<VariableEventArgs> ValueChanged;
		public event EventHandler<VariableCollectionEventArgs> ValueRemovedFromCollection;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public virtual string Name {
			get{ 
				return name; 
			}
		}
		
		/// <summary>
		/// Gets value of variable, which is safe to use.
		/// </summary>
		public Value Value {
			get {
				Value v = GetValue();
				if (v.IsExpired) {
					return new UnavailableValue(debugger, "The value has expired");
				} else {
					return v;
				}
			}
			internal set {
				val = value;
				val.ValueChanged += delegate { OnValueChanged(); };
				OnValueChanged();
			}
		}
		
		protected virtual Value GetValue()
		{
			return val;
		}
		
		/// <summary>
		/// Return up-to-date collection of subvariables.
		/// This collection is lazy - you need to call its method Update if you want to use it later
		/// </summary>
		public VariableCollection SubVariables {
			get {
				subVariables.Update();
				return subVariables;
			}
		}
		
		public bool MayHaveSubVariables {
			get {
				return val.MayHaveSubVariables;
			}
		}
		
		protected virtual void OnValueChanged()
		{
			if (ValueChanged != null) {
				ValueChanged(this, new VariableEventArgs(this));
			}
		}
		
		void OnSubVariablesUpdating(object sender, VariableCollectionEventArgs e)
		{
			VariableCollection newVariables = new VariableCollection(debugger);
			foreach(Variable v in Value.SubVariables) {
				newVariables.Add(v);
			}
			subVariables.UpdateTo(newVariables);
		}
		
		protected internal virtual void OnValueRemovedFromCollection(VariableCollectionEventArgs e) {
			if (ValueRemovedFromCollection != null) {
				ValueRemovedFromCollection(this, e);
			}
		}
		
		public Variable(NDebugger debugger, Value val, string name)
		{
			this.debugger = debugger;
			this.Value = val;
			this.name = name;
			this.subVariables = new VariableCollection(debugger);
			this.subVariables.Updating += OnSubVariablesUpdating;
		}
	}
}
