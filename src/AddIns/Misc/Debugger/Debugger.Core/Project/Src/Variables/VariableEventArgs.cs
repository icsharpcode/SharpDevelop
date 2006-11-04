// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	public class VariableEventArgs: ProcessEventArgs
	{
		Variable variable;
		
		public Variable Variable {
			get {
				return variable;
			}
		}
		
		public VariableEventArgs(Variable variable): base(variable.Value.Process)
		{
			this.variable = variable;
		}
	}
}
