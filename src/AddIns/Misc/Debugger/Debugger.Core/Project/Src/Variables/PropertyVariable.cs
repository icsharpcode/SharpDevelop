// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

using DebuggerInterop.Core;
	
namespace DebuggerLibrary 
{
	public class PropertyVariable: Variable 
	{
		Eval           eval;
		Variable       currentValue;
		public event EventHandler ValueEvaluated;
		
		internal PropertyVariable(Eval eval, string name):base(null, name)
		{
			this.eval = eval;
			eval.EvalComplete += new EventHandler(EvalComplete);
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
		
		public override VariableCollection SubVariables { 
			get {
				if (!IsEvaluated) {
					Evaluate();
				}
				return currentValue.SubVariables;
			}
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
		
		void EvalComplete(object sender, EventArgs args)
		{
			currentValue = VariableFactory.CreateVariable(eval.GetResult(), Name);
			OnValueEvaluated();
		}
		
		protected void OnValueEvaluated()
		{
			if (ValueEvaluated != null) {
				ValueEvaluated(this, EventArgs.Empty);
			}
		}
	}
}
