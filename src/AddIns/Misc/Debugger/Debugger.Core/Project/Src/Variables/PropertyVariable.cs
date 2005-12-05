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
		
		internal PropertyVariable(Eval eval, string name):base(new UnavailableValue(eval.Debugger), name)
		{
			this.Eval = eval;
		}
		
		public bool IsEvaluated {
			get {
				return eval.Completed;
			}
		}
		
		protected override Value GetValue()
		{
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
		
		public Eval Eval {
			get {
				return eval;
			}
			set {
				if (debugger.PausedReason != PausedReason.AllEvalsComplete) {
					eval = value;
					eval.EvalStarted += EvalStarted;
					eval.EvalComplete += EvalComplete;
					OnValueChanged();
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
