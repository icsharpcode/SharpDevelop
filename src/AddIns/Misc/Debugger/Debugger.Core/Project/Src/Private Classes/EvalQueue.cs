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
	class EvalQueue
	{
		static ArrayList waitingEvals = new ArrayList();
		
		public static event EventHandler AllEvalsComplete;
		
		static public void AddEval(Eval eval)
		{
			waitingEvals.Add(eval);
		}
		
		static public void PerformAllEvals()
		{
			while (waitingEvals.Count > 0) {
				PerformNextEval();
			}
		}
		
		static public void PerformNextEval()
		{
			if (NDebugger.IsProcessRunning) {
				return;
			}
			if (waitingEvals.Count == 0) {
				return;
			}
			Eval eval = (Eval)waitingEvals[0];
			waitingEvals.Remove(eval);
			eval.PerformEval();
			
			if (waitingEvals.Count == 0) {
				if (AllEvalsComplete != null) {
					AllEvalsComplete(null, EventArgs.Empty);
				}
			}
		}
	}
}
