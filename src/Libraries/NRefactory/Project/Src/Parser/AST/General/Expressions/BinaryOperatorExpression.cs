using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public enum BinaryOperatorType
	{
		None,
		
		BitwiseAnd,
		BitwiseOr,
		LogicalAnd, // Lazy operator
		LogicalOr,  // Lazy operator
		ExclusiveOr,
		
		GreaterThan,
		GreaterThanOrEqual,
		Equality,
		InEquality,
		LessThan,
		LessThanOrEqual,
		
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulus,
		ValueEquality, // What was this for ?
		// VB specific operators
		DivideInteger,
		Power,
		Concat,
		
		// additional
		ShiftLeft,
		ShiftRight,
		IS,
		IsNot,
		AS,
		Like, // What was this for ?
	}
	
	public class BinaryOperatorExpression : Expression
	{
		Expression         left;
		BinaryOperatorType op;
		Expression         right;
		
		public Expression Left {
			get {
				return left;
			}
			set {
				left = Expression.CheckNull(value);
			}
		}
		
		public BinaryOperatorType Op {
			get {
				return op;
			}
			set {
				op = value;
			}
		}
		
		public Expression Right {
			get {
				return right;
			}
			set {
				right = Expression.CheckNull(value);
			}
		}
		
		public BinaryOperatorExpression(Expression left, BinaryOperatorType op, Expression right)
		{
			this.Left  = left;
			this.op    = op;
			this.Right = right;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[BinaryOperatorExpression: Op={0}, Left={1}, Right={2}]",
			                     op,
			                     left,
			                     right);
		}
	}
}
