using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class AddHandlerStatement : Statement
	{
		Expression eventExpression;
		Expression handlerExpression;
		
		public Expression EventExpression {
			get {
				return eventExpression;
			}
			set {
				eventExpression = Expression.CheckNull(value);
			}
		}
		public Expression HandlerExpression {
			get {
				return handlerExpression;
			}
			set {
				handlerExpression = Expression.CheckNull(value);
			}
		}
		
		public AddHandlerStatement(Expression eventExpression, Expression handlerExpression)
		{
			this.EventExpression   = eventExpression;
			this.HandlerExpression = handlerExpression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[AddHandlerStatement: EventExpression = {0}, HandlerExpression = {1}]",
			                     EventExpression,
			                     HandlerExpression);
		}
	}
}
