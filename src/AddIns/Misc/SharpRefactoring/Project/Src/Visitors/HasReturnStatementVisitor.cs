/*
 * Created by SharpDevelop.
 * User: HP
 * Date: 28.04.2008
 * Time: 22:18
 */

using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace SharpRefactoring.Visitors
{
	public class HasReturnStatementVisitor : AbstractAstVisitor
	{
		bool hasReturn;
		
		public bool HasReturn {
			get { return hasReturn; }
		}
		
		public HasReturnStatementVisitor()
		{
		}
		
		public override object VisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			this.hasReturn = true;
			return base.VisitReturnStatement(returnStatement, data);
		}
	}
}
