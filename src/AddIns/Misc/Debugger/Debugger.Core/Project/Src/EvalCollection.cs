// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Debugger.Interop.CorDebug;

namespace Debugger
{
	public class EvalCollection: CollectionWithEvents<Eval>
	{
		public EvalCollection(NDebugger debugger): base(debugger) {}
		
		internal Eval this[ICorDebugEval corEval] {
			get {
				foreach(Eval eval in this) {
					if (eval.IsCorEval(corEval)) {
						return eval;
					}
				}
				throw new DebuggerException("Eval not found for given ICorDebugEval");
			}
		}
	}
}
