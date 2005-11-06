// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using DebuggerInterop.Core;
	
namespace Debugger 
{
	public class PropertyVariable: Variable 
	{
		Eval           eval;
		Variable       currentValue;
		public event EventHandler<DebuggerEventArgs> ValueEvaluated;
		
		internal PropertyVariable(NDebugger debugger, Eval eval, string name):base(debugger, null, name)
		{
			this.eval = eval;
			eval.EvalComplete += new EventHandler<DebuggerEventArgs>(EvalComplete);
		}
		
		public bool IsEvaluated {
			get {
				return currentValue != null;
			}
		}
		
		public override object Value { 
			get {
				if (!IsEvaluated) {
					Evaluate();
				}
				return currentValue.Value;
			}
		}
		
		public override string Type { 
			get {
				if (!IsEvaluated) {
					Evaluate();
				}
				return currentValue.Type;
			}
		}

		public override bool MayHaveSubVariables {
			get {
				if (IsEvaluated) {
					return currentValue.MayHaveSubVariables;
				} else {
					return true;
				}
			}
		}

		protected override VariableCollection GetSubVariables()
		{
			if (!IsEvaluated) {
					Evaluate();
				}
			return currentValue.SubVariables;
		}
		
		/// <summary>
		/// Executes evaluation of variable and doesn't return
		/// until value is evaluated.
		/// </summary>
		public void Evaluate()
		{
			eval.PerformEval();
		}
		
		/// <summary>
		/// Executes evaluation of variable and returns imideatly
		/// </summary>
		public void AsyncEvaluate()
		{
			eval.AsyncPerformEval();
		}
		
		void EvalComplete(object sender, DebuggerEventArgs args)
		{
			currentValue = VariableFactory.CreateVariable(debugger, eval.GetResult(), Name);
			OnValueEvaluated();
		}
		
		protected void OnValueEvaluated()
		{
			if (ValueEvaluated != null) {
				ValueEvaluated(this, new DebuggerEventArgs(debugger));
			}
		}
	}
}
