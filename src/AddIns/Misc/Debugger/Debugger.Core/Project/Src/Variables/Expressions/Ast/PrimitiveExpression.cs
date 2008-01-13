// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

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
			// TODO: Need for literal method arguments.  Use Eval to create value.
			throw new NotImplementedException();
		}
	}
}
