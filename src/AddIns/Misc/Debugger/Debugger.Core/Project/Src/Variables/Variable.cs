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
		
		public virtual Value Value {
			get {
				return val;
			}
			internal set {
				val = value;
				OnValueChanged();
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
		
		public Variable(NDebugger debugger, Value val, string name)
		{
			this.debugger = debugger;
			this.val = val;
			this.name = name;
			this.subVariables = new VariableCollection(debugger);
			this.subVariables.Updating += OnSubVariablesUpdating;
		}
	}
}
