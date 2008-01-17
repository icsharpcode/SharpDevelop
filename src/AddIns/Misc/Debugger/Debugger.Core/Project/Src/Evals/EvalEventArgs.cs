// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger 
{	
	/// <summary>
	/// Provides data related to evalution events
	/// </summary>
	[Serializable]
	public class EvalEventArgs : ProcessEventArgs
	{
		Eval eval;
		
		/// <summary> The evaluation that caused the event </summary>
		public Eval Eval {
			get {
				return eval;
			}
		}
		
		/// <summary> Initializes a new instance of the class </summary>
		public EvalEventArgs(Eval eval): base(eval.Process)
		{
			this.eval = eval;
		}
	}
}
