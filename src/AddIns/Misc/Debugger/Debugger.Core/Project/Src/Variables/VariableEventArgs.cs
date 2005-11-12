// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger 
{	
	[Serializable]
	public class VariableEventArgs : DebuggerEventArgs
	{
		Variable variable;
		
		public Variable Variable {
			get {
				return variable;
			}
			set {
				variable = value;
			}
		}
		
		public VariableEventArgs(NDebugger debugger, Variable variable): base(debugger)
		{
			this.variable = variable;
		}
	}
}
