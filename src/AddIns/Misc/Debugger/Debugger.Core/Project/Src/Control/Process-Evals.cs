// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class Process
	{
		List<Eval> activeEvals = new List<Eval>();
		
		public bool Evaluating {
			get {
				return activeEvals.Count > 0;
			}
		}
		
		internal Eval GetEval(ICorDebugEval corEval)
		{
			foreach(Eval eval in activeEvals) {
				if (eval.IsCorEval(corEval)) {
					return eval;
				}
			}
			throw new DebuggerException("Eval not found for given ICorDebugEval");
		}
		
		internal void NotifyEvaluationStarted(Eval eval)
		{
			activeEvals.Add(eval);
		}
		
		internal void NotifyEvaluationComplete(Eval eval)
		{
			activeEvals.Remove(eval);
		}
	}
}
