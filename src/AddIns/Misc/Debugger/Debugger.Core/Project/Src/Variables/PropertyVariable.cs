// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

using DebuggerInterop.Core;
using DebuggerInterop.Symbols;
	
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
		
		public override object Value { 
			get {
				if (currentValue == null) {
					return String.Empty;
				}
				return currentValue.Value;
			}
		}
		
		public override string Type { 
			get {
				if (currentValue == null) {
					return String.Empty;
				}
				return currentValue.Type;
			}
		}
		
		public override VariableCollection SubVariables { 
			get {
				if (currentValue == null) {
					return new VariableCollection();
				}
				return currentValue.SubVariables;
			}
		}
		
		internal void EvalComplete(object sender, EventArgs args)
		{
			/*ICorDebugValue corValue;
			eval.corEval.GetResult(out corValue);
			currentValue = VariableFactory.CreateVariable(corValue, Name);
			OnValueEvaluated();*/
		}
		
		internal void OnValueEvaluated()
		{
			if (ValueEvaluated != null) {
				ValueEvaluated(this, EventArgs.Empty);
			}
		}
	}
}
