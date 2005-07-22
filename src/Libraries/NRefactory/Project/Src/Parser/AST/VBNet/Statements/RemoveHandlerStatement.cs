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
	public class RemoveHandlerStatement : Statement
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
		
		public RemoveHandlerStatement(Expression eventExpression, Expression handlerExpression)
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
			return String.Format("[RemoveHandlerStatement: EventExpression = {0}, HandlerExpression = {1}]",
			                     EventExpression,
			                     HandlerExpression);
		}
	}
}
