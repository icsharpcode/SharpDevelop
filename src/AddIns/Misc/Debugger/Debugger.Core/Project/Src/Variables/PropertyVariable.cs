// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Interop.CorDebug;
	
namespace Debugger 
{
	public class PropertyVariable: Variable 
	{
		Eval           eval;
		
		public event EventHandler<DebuggerEventArgs> ValueEvaluated;
		
		internal PropertyVariable(Eval eval, string name):base(eval.Debugger, null, name)
		{
			this.eval = eval;
			eval.EvalStarted += EvalStarted;
			eval.EvalComplete += EvalComplete;
		}
		
		public bool IsEvaluated {
			get {
				return eval.Completed;
			}
		}
		
		public override Value Value {
			get {
				if (IsEvaluated) {
					if (eval.Result != null) {
						return eval.Result;
					} else {
						return new UnavailableValue(debugger, "No return value");
					}
				} else {
					if (eval.Evaluating) {
						return new UnavailableValue(debugger, "Evaluating...");
					} else {
						return new UnavailableValue(debugger, "Evaluation pending");
					}
				}
			}
		}
		
		void EvalStarted(object sender, EvalEventArgs args)
		{
			OnValueChanged();
		}
		
		void EvalComplete(object sender, EvalEventArgs args)
		{
			OnValueEvaluated();
			OnValueChanged();
		}
		
		protected void OnValueEvaluated()
		{
			if (ValueEvaluated != null) {
				ValueEvaluated(this, new DebuggerEventArgs(debugger));
			}
		}
	}
}
