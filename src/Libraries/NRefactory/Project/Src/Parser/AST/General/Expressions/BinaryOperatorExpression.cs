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
	public enum BinaryOperatorType
	{
		None,
		
		/// <summary>'&amp;' in C#, 'And' in VB.</summary>
		BitwiseAnd,
		/// <summary>'|' in C#, 'Or' in VB.</summary>
		BitwiseOr,
		/// <summary>'&amp;&amp;' in C#, 'AndAlso' in VB.</summary>
		LogicalAnd,
		/// <summary>'||' in C#, 'OrElse' in VB.</summary>
		LogicalOr,
		/// <summary>'^' in C#, 'Xor' in VB.</summary>
		ExclusiveOr,
		
		/// <summary>&gt;</summary>
		GreaterThan,
		/// <summary>&gt;=</summary>
		GreaterThanOrEqual,
		/// <summary>'==' in C#, '=' in VB.</summary>
		Equality,
		/// <summary>'!=' in C#, '&lt;&gt;' in VB.</summary>
		InEquality,
		/// <summary>&lt;</summary>
		LessThan,
		/// <summary>&lt;=</summary>
		LessThanOrEqual,
		
		/// <summary>+</summary>
		Add,
		/// <summary>-</summary>
		Subtract,
		/// <summary>*</summary>
		Multiply,
		/// <summary>/</summary>
		Divide,
		/// <summary>'%' in C#, 'Mod' in VB.</summary>
		Modulus,
		/// <summary>VB-only: \</summary>
		DivideInteger,
		/// <summary>VB-only: ^</summary>
		Power,
		/// <summary>VB-only: &</summary>
		Concat,
		
		/// <summary>C#: &lt;&lt;</summary>
		ShiftLeft,
		/// <summary>C#: &gt;&gt;</summary>
		ShiftRight,
		/// <summary>VB-only: Is</summary>
		ReferenceEquality,
		/// <summary>VB-only: IsNot</summary>
		ReferenceInequality,
		/// <summary>C#: Is</summary>
		TypeCheck,
		/// <summary>'as' in C#, 'TryCast(l, r)' in VB</summary>
		AsCast,
		/// <summary>VB-only: Like</summary>
		Like,
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
