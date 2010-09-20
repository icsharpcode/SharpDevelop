// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
