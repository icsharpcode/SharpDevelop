// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Expressions
{
	/// <summary>
	/// 'this' reference of a non-static method.
	/// </summary>
	public class ThisReferenceExpression: Expression
	{
		public override string Code {
			get {
				return "this";
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			return context.GetThisValue();
		}
		
		#region GetHashCode and Equals
		
		public override int GetHashCode()
		{
			return typeof(ThisReferenceExpression).GetHashCode();
		}
		
		public override bool Equals(object obj)
		{
			return obj is ThisReferenceExpression;
		}
		
		#endregion
	}
}
