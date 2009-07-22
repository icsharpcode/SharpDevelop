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
		EvalCollection evals;
		
		public EvalCollection ActiveEvals {
			get { return evals; }
		}
		
		internal bool Evaluating {
			get { return evals.Count > 0; }
		}
	}
	
	public class EvalCollection: CollectionWithEvents<Eval>
	{
		public EvalCollection(NDebugger debugger): base(debugger) {}
		
		internal Eval Get(ICorDebugEval corEval)
		{
			foreach(Eval eval in this) {
				if (eval.IsCorEval(corEval)) {
					return eval;
				}
			}
			throw new DebuggerException("Eval not found for given ICorDebugEval");
		}
	}
}
