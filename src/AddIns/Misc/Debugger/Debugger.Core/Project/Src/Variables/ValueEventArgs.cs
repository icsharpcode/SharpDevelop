// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger 
{	
	[Serializable]
	public class ValueEventArgs : DebuggerEventArgs
	{
		Value val;
		
		public Value Value {
			get {
				return val;
			}
		}
		
		public ValueEventArgs(Value val): base(val.Debugger)
		{
			this.val = val;
		}
	}
}
