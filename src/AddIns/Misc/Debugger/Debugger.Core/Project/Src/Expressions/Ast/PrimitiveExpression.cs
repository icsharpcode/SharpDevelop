// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Expressions
{
	/// <summary>
	/// Literal expression
	/// </summary>
	public class PrimitiveExpression: Expression
	{
		object value;
		
		public object Value {
			get { return value; }
		}
		
		public PrimitiveExpression(object value)
		{
			this.value = value;
		}
		
		public override string Code {
			get {
				if (value == null) {
					return "null";
				} else {
					return value.ToString();
				}
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			return Eval.CreateValue(context.Process, this.Value);
		}
		
		#region GetHashCode and Equals
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (value != null) hashCode += 1000000007 * value.GetHashCode(); 
			}
			return hashCode;
		}
		
		public override bool Equals(object obj)
		{
			PrimitiveExpression other = obj as PrimitiveExpression;
			if (other == null) return false; 
			return object.Equals(this.value, other.value);
		}
		
		#endregion
	}
}
