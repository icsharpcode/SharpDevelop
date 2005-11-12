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
		
		public override object Value {
			get {
				if (IsEvaluated) {
					if (eval.Result != null) {
						return eval.Result.Value;
					} else {
						return null;
					}
				} else {
					if (eval.Evaluating) {
						return new VariableMagicValue("Evaluating...");
					} else {
						return new VariableMagicValue("Evaluation pending");
					}
				}
			}
		}
		
		public override string Type { 
			get {
				if (IsEvaluated) {
					if (eval.Result != null) {
						return eval.Result.Type;
					} else {
						return String.Empty;
					}
				} else {
					return String.Empty;
				}
			}
		}

		public override bool MayHaveSubVariables {
			get {
				if (IsEvaluated) {
					if (eval.Result != null) {
						return eval.Result.MayHaveSubVariables;
					} else {
						return true;
					}
				} else {
					return true;
				}
			}
		}

		protected override VariableCollection GetSubVariables()
		{
			if (IsEvaluated) {
				if (eval.Result != null) {
					return eval.Result.SubVariables;
				} else {
					return VariableCollection.Empty;
				}
			} else {
				return VariableCollection.Empty;
			}
		}
		
		void EvalStarted(object sender, EvalEventArgs args)
		{
			OnValueChanged(new VariableEventArgs(this));
		}
		
		void EvalComplete(object sender, EvalEventArgs args)
		{
			if (eval.Result != null) {
				eval.Result.Name = this.Name;
			}
			OnValueEvaluated();
			OnValueChanged(new VariableEventArgs(this));
		}
		
		protected void OnValueEvaluated()
		{
			if (ValueEvaluated != null) {
				ValueEvaluated(this, new DebuggerEventArgs(debugger));
			}
		}
	}
}
