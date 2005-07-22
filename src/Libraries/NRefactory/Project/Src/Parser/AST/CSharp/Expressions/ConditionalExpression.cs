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
	public class ConditionalExpression : Expression
	{
		Expression condition;
		Expression trueExpression;
		Expression falseExpression;
		
		public Expression Condition {
			get {
				return condition;
			}
			set {
				condition = Expression.CheckNull(value);
			}
		}
		
		public Expression TrueExpression {
			get {
				return trueExpression;
			}
			set {
				trueExpression = Expression.CheckNull(value);
			}
		}
		
		public Expression FalseExpression {
			get {
				return falseExpression;
			}
			set {
				falseExpression = Expression.CheckNull(value);
			}
		}
		
		public ConditionalExpression(Expression condition, Expression trueExpression, Expression falseExpression)
		{
			this.Condition       = condition;
			this.TrueExpression  = trueExpression;
			this.FalseExpression = falseExpression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ConditionalExpression: Condition={0}, TrueExpression={1}, FalseExpression={2}]",
			                     condition,
			                     trueExpression,
			                     falseExpression);
		}
	}
}
