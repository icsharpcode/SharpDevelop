// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2285 $</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Ast;
using Debugger.Wrappers.CorSym;

namespace Debugger
{
	class EvaluateAstVisitor: NotImplementedAstVisitor
	{
		StackFrame stackFrame;
		
		public StackFrame StackFrame {
			get { return stackFrame; }
		}
		
		public EvaluateAstVisitor(StackFrame stackFrame)
		{
			this.stackFrame = stackFrame;
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public override object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			throw new NotImplementedException();
		}
	}
}
