// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using DebuggerInterop.Core;

namespace DebuggerLibrary 
{	
	delegate void CorDebugEvalEventHandler (object sender, CorDebugEvalEventArgs e);
	
	class CorDebugEvalEventArgs : DebuggerEventArgs
	{
		ICorDebugEval corDebugEval;
		
		public ICorDebugEval CorDebugEval {
			get {
				return corDebugEval;
			}
		}
		
		public CorDebugEvalEventArgs(NDebugger debugger, ICorDebugEval corDebugEval): base(debugger)
		{
			this.corDebugEval = corDebugEval;
		}
	}
}
