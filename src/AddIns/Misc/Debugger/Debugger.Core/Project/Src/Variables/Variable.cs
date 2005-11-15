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
		
		protected virtual void OnValueChanged()
		{
			if (ValueChanged != null) {
				ValueChanged(this, new VariableEventArgs(this));
			}
		}
		
		public Variable(NDebugger debugger, Value val, string name)
		{
			this.debugger = debugger;
			this.val = val;
			this.name = name;
		}
	}
}
