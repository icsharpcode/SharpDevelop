using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class TypeOfIsExpression : TypeOfExpression
	{
		Expression expression;
		
		public Expression Expression {
			get {
				return expression;
			}
			set {
				expression = Expression.CheckNull(value);
			}
		}
		
		public TypeOfIsExpression(Expression expression, TypeReference typeReference) : base(typeReference)
		{
			this.Expression = expression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[TypeOfIsExpression: Expression={0}, TypeReference={1}]",
			                     Expression,
			                     TypeReference);
		}
		
	}
}
