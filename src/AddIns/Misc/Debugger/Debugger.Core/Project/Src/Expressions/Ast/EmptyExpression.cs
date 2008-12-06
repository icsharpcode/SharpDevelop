// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Expressions
{
	/// <summary>
	/// A placeholder expression which can not be evaluated.
	/// </summary>
	public class EmptyExpression: Expression
	{
		public override string Code {
			get {
				return "?";
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			throw new GetValueException("Empty expression");
		}
		
		#region GetHashCode and Equals
		
		public override int GetHashCode()
		{
			return typeof(EmptyExpression).GetHashCode();
		}
		
		public override bool Equals(object obj)
		{
			return obj is EmptyExpression;
		}
		
		#endregion
	}
}
