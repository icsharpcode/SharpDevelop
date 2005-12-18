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
	public class EvalEventArgs : DebuggerEventArgs
	{
		Eval eval;
		
		public Eval Eval {
			get {
				return eval;
			}
		}
		
		public Value Result {
			get {
				return eval.Result;
			}
		}
		
		public EvalEventArgs(Eval eval): base(eval.Debugger)
		{
			this.eval = eval;
		}
	}
}
