// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using DebuggerInterop.Core;

namespace DebuggerLibrary 
{	
	delegate void CorDebugEvalEventHandler (object sender, CorDebugEvalEventArgs e);
	
	class CorDebugEvalEventArgs : System.EventArgs 
	{
		ICorDebugEval corDebugEval;
		
		public ICorDebugEval CorDebugEval {
			get {
				return corDebugEval;
			}
		}
		
		public CorDebugEvalEventArgs(ICorDebugEval corDebugEval)
		{
			this.corDebugEval = corDebugEval;
		}
	}
}
