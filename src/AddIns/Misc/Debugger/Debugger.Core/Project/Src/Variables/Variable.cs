// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Interop.CorDebug;

namespace Debugger
{
	public delegate Value ValueGetter();
	
	public class Variable: RemotingObjectBase
	{
		protected NDebugger debugger;
		
		string name;
		Value val;
		VariableCollection subVariables;
		
		event ValueGetter updating;
		
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
				if (v == null) {
					return new UnavailableValue(debugger);
				}
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
			if ((val == null || val.IsExpired) && updating != null) {
				val = updating();
			}
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
			subVariables.UpdateTo(Value.GetSubVariables(delegate{return this.Value;}));
		}
		
		protected internal virtual void OnValueRemovedFromCollection(VariableCollectionEventArgs e) {
			if (ValueRemovedFromCollection != null) {
				ValueRemovedFromCollection(this, e);
			}
		}
		
		public Variable(NDebugger debugger, ICorDebugValue corValue, string name):this(debugger, Value.CreateValue(debugger, corValue), name, null)
		{
			
		}
		
		public Variable(NDebugger debugger, string name, ValueGetter updating):this(debugger, null, name, updating)
		{
			
		}
		
		public Variable(Value val, string name):this(val.Debugger, val, name, null)
		{
			
		}
		
		Variable(NDebugger debugger, Value val, string name, ValueGetter updating)
		{
			this.debugger = debugger;
			if (val != null) {
				this.Value = val;
			}
			this.name = name;
			this.updating = updating;
			this.subVariables = new VariableCollection(debugger);
			this.subVariables.Updating += OnSubVariablesUpdating;
		}
	}
}
