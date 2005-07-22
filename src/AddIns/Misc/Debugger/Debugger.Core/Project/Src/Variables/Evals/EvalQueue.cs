// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{
	class EvalQueue
	{
		NDebugger debugger;

		ArrayList waitingEvals = new ArrayList();
		
		public event EventHandler<DebuggerEventArgs> AllEvalsComplete;

		public EvalQueue(NDebugger debugger)
		{
			this.debugger = debugger;
		}
		
		public void AddEval(Eval eval)
		{
			waitingEvals.Add(eval);
		}
		
		public void PerformAllEvals()
		{
			while (waitingEvals.Count > 0) {
				PerformNextEval();
			}
		}
		
		public void PerformNextEval()
		{
			if (waitingEvals.Count == 0) {
				throw new DebuggerException("No eval in queue to perform.");
			}
			Eval eval = (Eval)waitingEvals[0];
			waitingEvals.Remove(eval);
			eval.PerformEval();
			
			if (waitingEvals.Count == 0) {
				if (AllEvalsComplete != null) {
					AllEvalsComplete(this, new DebuggerEventArgs(debugger));
				}
			}
		}
	}
}
