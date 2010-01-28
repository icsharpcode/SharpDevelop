// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
