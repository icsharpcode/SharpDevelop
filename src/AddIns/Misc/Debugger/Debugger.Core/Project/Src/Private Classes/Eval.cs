// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;
using DebuggerInterop.Symbols;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{
	class Eval 
	{
		ICorDebugEval     corEval;
		ICorDebugFunction corFunction;
		ICorDebugValue[]  args;
		
		public event EventHandler EvalComplete;
		
		void OnEvalComplete()
		{
			if (EvalComplete != null) {
				EvalComplete(this, EventArgs.Empty);
			}
		}		
		
		public Eval(ICorDebugFunction corFunction, ICorDebugValue[] args)
		{
			this.corFunction = corFunction;
			this.args = args;
		}
		
		public void PerformEval()
		{
			if (NDebugger.IsProcessRunning) {
				throw new DebuggerException("Debugger must be paused");
			}

			NDebugger.CurrentThread.CorThread.CreateEval(out corEval);
			
			corEval.CallFunction(corFunction, (uint)args.Length, args);
			
			NDebugger.Continue();
		}
	}
}
