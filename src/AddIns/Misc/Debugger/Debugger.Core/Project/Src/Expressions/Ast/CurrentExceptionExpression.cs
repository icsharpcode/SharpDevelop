// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Expressions
{
	/// <summary>
	/// An expression which returns the current exception on the thread
	/// </summary>
	public class CurrentExceptionExpression: Expression
	{
		public override string Code {
			get {
				return "$exception";
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			if (context.Thread.CurrentException != null) {
				return context.Thread.CurrentException.Value;
			} else {
				throw new GetValueException("No current exception");
			}
		}
		
		#region GetHashCode and Equals
		
		public override int GetHashCode()
		{
			return typeof(CurrentExceptionExpression).GetHashCode();
		}
		
		public override bool Equals(object obj)
		{
			return obj is CurrentExceptionExpression;
		}
		
		#endregion
	}
}
