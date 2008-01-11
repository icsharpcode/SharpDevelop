// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Expressions
{
	public class ThisReferenceExpression: Expression
	{
		public override string Code {
			get {
				return "this";
			}
		}
		
		protected override Value EvaluateInternal(StackFrame context)
		{
			if (context.MethodInfo.IsStatic) {
				throw new ExpressionEvaluateException(this, "'this' is not valid in static method");
			}
			return context.ThisValue;
		}
	}
}
