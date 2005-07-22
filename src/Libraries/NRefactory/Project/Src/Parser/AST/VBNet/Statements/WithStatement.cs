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
	public class WithStatement : Statement
	{
		Expression     expression;
		BlockStatement body = BlockStatement.Null;
		
		public Expression Expression {
			get {
				return expression;
			}
			set {
				expression = Expression.CheckNull(value);
			}
		}
		
		public BlockStatement Body {
			get {
				return body;
			}
			set {
				body = BlockStatement.CheckNull(value);;
			}
		}
		
		public WithStatement(Expression expression)
		{
			this.Expression = expression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[WithStatment: Expression={0}, Body={1}]", 
			                     expression,
			                     body);
		}
	}
}
