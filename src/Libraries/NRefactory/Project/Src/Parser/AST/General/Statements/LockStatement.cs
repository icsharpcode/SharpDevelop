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
	public class LockStatement : Statement
	{
		Expression lockExpression;
		Statement  embeddedStatement;
		
		public Expression LockExpression {
			get {
				return lockExpression;
			}
			set {
				lockExpression = Expression.CheckNull(value);
			}
		}
		public Statement EmbeddedStatement {
			get {
				return embeddedStatement;
			}
			set {
				embeddedStatement = Statement.CheckNull(value);
			}
		}
		
		public LockStatement(Expression lockExpression, Statement embeddedStatement)
		{
			this.LockExpression    = lockExpression;
			this.EmbeddedStatement = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[LockStatement: LockExpression={0}, EmbeddedStatement={1}]", 
			                     lockExpression,
			                     embeddedStatement);
		}
	}
}
