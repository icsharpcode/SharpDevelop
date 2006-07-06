// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public class Variable: RemotingObjectBase
	{
		protected NDebugger debugger;
		
		string name;
		VariableCollection subVariables;
		
		internal protected PersistentValue pValue;
		internal Value cachedValue;
		
		event EventHandler<DebuggerEventArgs> valueChanged;
		public event EventHandler<VariableCollectionEventArgs> ValueRemovedFromCollection;
		
		public event EventHandler<DebuggerEventArgs> ValueChanged {
			add {
				valueChanged += value;
				debugger.DebuggeeStateChanged += value;
				debugger.ProcessExited += delegate {
					debugger.DebuggeeStateChanged -= value;
				};
			}
			remove {
				valueChanged -= value;
				debugger.DebuggeeStateChanged -= value;
			}
		}
		
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
		/// Gets value of variable which is safe to use (it is not null and it is not expired)
		/// </summary>
		public Value Value {
			get {
				if (cachedValue == null || cachedValue.IsExpired) {
					cachedValue = pValue.Value;
					if (cachedValue == null) throw new DebuggerException("ValueGetter returned null");
					cachedValue.ValueChanged += delegate { OnValueChanged(); };
				}
				if (cachedValue.IsExpired) {
					return new UnavailableValue(debugger, "The value has expired");
				}
				return cachedValue;
			}
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
				return Value.MayHaveSubVariables;
			}
		}
		
		protected internal virtual void OnValueChanged()
		{
			cachedValue = null;
			if (valueChanged != null) {
				valueChanged(this, new VariableEventArgs(this));
			}
		}
		
		protected internal virtual void OnValueRemovedFromCollection(VariableCollectionEventArgs e) {
			if (ValueRemovedFromCollection != null) {
				ValueRemovedFromCollection(this, e);
			}
		}
		
		public Variable(NDebugger debugger, ICorDebugValue corValue, string name):this(Value.CreateValue(debugger, corValue), name)
		{
			
		}
		
		public Variable(Value val, string name):this(val.Debugger, name, new PersistentValue(delegate {return val;}))
		{
			
		}
		
		public Variable(NDebugger debugger, string name, PersistentValue pValue)
		{
			this.debugger = debugger;
			this.name = name;
			this.pValue = pValue;
			this.subVariables = new VariableCollection(debugger);
			this.subVariables.Updating += OnSubVariablesUpdating;
			
			if (name.StartsWith("<") && name.Contains(">") && name != "<Base class>") {
				string middle = name.TrimStart('<').Split('>')[0]; // Get text between '<' and '>'
				if (middle != "") {
					this.name = middle;
				}
			}
		}
		
		void OnSubVariablesUpdating(object sender, VariableCollectionEventArgs e)
		{
			subVariables.UpdateTo(Value.GetSubVariables(new PersistentValue(delegate{return this.Value;})));
		}
	}
}
