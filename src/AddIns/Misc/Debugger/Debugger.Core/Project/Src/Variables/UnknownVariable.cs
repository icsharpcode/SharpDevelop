// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;

namespace DebuggerLibrary
{
	public class UnknownVariable: Variable
	{
		public override object Value {
			get {
				return "<unknown>"; 
			} 
		}
		
		public override string Type { 
			get {
				return "<unknown>"; 
			} 
		}

		internal unsafe UnknownVariable(NDebugger debugger, ICorDebugValue corValue, string name):base(debugger, corValue, name)
		{
			
		}
	}
}
			
