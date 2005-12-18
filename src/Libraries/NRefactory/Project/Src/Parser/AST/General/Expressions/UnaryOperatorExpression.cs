// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	
	public enum UnaryOperatorType
	{
		None,
		Not,
		BitNot,
		
		Minus,
		Plus,
		
		Increment,
		Decrement,
		
		PostIncrement,
		PostDecrement,
		
		Star,
		BitWiseAnd
	}
	public class UnaryOperatorExpression : Expression
	{
		Expression        expression;
		UnaryOperatorType op;
		
		public Expression Expression {
			get {
				return expression;
			}
			set {
				expression = Expression.CheckNull(value);
			}
		}
		public UnaryOperatorType Op {
			get {
				return op;
			}
			set {
				op = value;
			}
		}
		
		public UnaryOperatorExpression(UnaryOperatorType op)
		{
			this.expression = Expression.Null;
			this.op         = op;
		}
		
		public UnaryOperatorExpression(Expression expression, UnaryOperatorType op)
		{
			this.Expression = expression;
			this.op         = op;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[UnaryOperatorExpression: Op={0}, Expression={1}]",
			                     op,
			                     expression);
		}
	}
}
