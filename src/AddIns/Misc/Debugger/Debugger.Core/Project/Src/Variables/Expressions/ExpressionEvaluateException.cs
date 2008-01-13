// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Expressions
{
	public class ExpressionEvaluateException: GetValueException
	{
		Expression expression;
		string error;
		
		public Expression Expression {
			get { return expression; }
		}
		
		public string Error {
			get { return error; }
		}
		
		public ExpressionEvaluateException(Expression expression, string error)
			:base(GetErrorMessage(expression, error))
		{
			this.expression = expression;
			this.error = error;
		}
		
		public static string GetErrorMessage(Expression expression, string error)
		{
			return String.Format("Error evaluating \"{0}\": {1}", expression.Code, error);
		}
	}
}
