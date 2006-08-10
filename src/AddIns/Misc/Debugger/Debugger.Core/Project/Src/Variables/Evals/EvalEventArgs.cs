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
	public class EvalEventArgs : ProcessEventArgs
	{
		Eval eval;
		
		public Eval Eval {
			get {
				return eval;
			}
		}
		
		public EvalEventArgs(Eval eval): base(eval.Process)
		{
			this.eval = eval;
		}
	}
}
