// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public enum AssignmentOperatorType
	{
		None,
		Assign,
		
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulus,
		
		Power,         // (VB only)
		DivideInteger, // (VB only)
		ConcatString,  // (VB only)
		
		ShiftLeft,
		ShiftRight,
		
		BitwiseAnd,
		BitwiseOr,
		ExclusiveOr,
	}
	
	public class AssignmentExpression : Expression
	{
		Expression             left;
		AssignmentOperatorType op;
		Expression             right;
		
		public Expression Left {
			get {
				return left;
			}
			set {
				left = Expression.CheckNull(value);
			}
		}
		
		public AssignmentOperatorType Op {
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
		
		public AssignmentExpression(Expression left, AssignmentOperatorType op, Expression right)
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
			return String.Format("[AssignmentExpression: Op={0}, Left={1}, Right={2}]",
			                     op,
			                     left,
			                     right);
		}
	}
}
