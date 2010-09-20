// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Visitors;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace SharpRefactoring.Visitors
{
	public enum ErrorKind {
		ContainsBreak,
		ContainsContinue,
		ContainsGoto,
		None
	}
	
	public class FindJumpInstructionsVisitor : AbstractAstVisitor
	{
		MethodDeclaration method;
		
		List<LabelStatement> labels;
		List<CaseLabel> cases;
		
		List<GotoStatement> gotos;
		List<GotoCaseStatement> gotoCases;
		
		List<BreakStatement> breaks;
		List<ContinueStatement> continues;
		
		public FindJumpInstructionsVisitor(MethodDeclaration method)
		{
			this.method = method;
			
			this.labels = new List<LabelStatement>();
			this.cases = new List<CaseLabel>();
			this.gotoCases = new List<GotoCaseStatement>();
			this.gotos = new List<GotoStatement>();
			this.breaks = new List<BreakStatement>();
			this.continues = new List<ContinueStatement>();
		}
		
		public override object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
		{
			return base.VisitDoLoopStatement(doLoopStatement, true);
		}
		
		public override object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			return base.VisitForeachStatement(foreachStatement, true);
		}
		
		public override object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			return base.VisitForNextStatement(forNextStatement, true);
		}
		
		public override object VisitForStatement(ForStatement forStatement, object data)
		{
			return base.VisitForStatement(forStatement, true);
		}
		
		public override object VisitSwitchStatement(SwitchStatement switchStatement, object data)
		{
			return base.VisitSwitchStatement(switchStatement, true);
		}
		
		public override object VisitBreakStatement(BreakStatement breakStatement, object data)
		{
			if (!(data is bool))
				this.breaks.Add(breakStatement);
			return base.VisitBreakStatement(breakStatement, data);
		}
		
		public override object VisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			if (!(data is bool))
				this.continues.Add(continueStatement);
			return base.VisitContinueStatement(continueStatement, data);
		}
		
		public override object VisitLabelStatement(LabelStatement labelStatement, object data)
		{
			this.labels.Add(labelStatement);
			return base.VisitLabelStatement(labelStatement, data);
		}
		
		public override object VisitCaseLabel(CaseLabel caseLabel, object data)
		{
			this.cases.Add(caseLabel);
			return base.VisitCaseLabel(caseLabel, data);
		}
		
		public override object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
		{
			gotoCases.Add(gotoCaseStatement);
			return base.VisitGotoCaseStatement(gotoCaseStatement, data);
		}
		
		public override object VisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			gotos.Add(gotoStatement);
			return base.VisitGotoStatement(gotoStatement, data);
		}
		
		public ErrorKind DoCheck()
		{
			if (this.breaks.Any())
				return ErrorKind.ContainsBreak;
			
			if (this.continues.Any())
				return ErrorKind.ContainsContinue;
			
			if (this.gotos.Any()) {
				foreach (GotoStatement stmt in this.gotos) {
					if (!this.labels.Any(label => label.Label == stmt.Label))
						return ErrorKind.ContainsGoto;
				}
			}
			
			if (this.gotoCases.Any()) {
				foreach (GotoCaseStatement stmt in this.gotoCases) {
					if (!this.cases.Any(@case => CompareCase(@case, stmt)))
						return ErrorKind.ContainsGoto;
				}
			}
			
			return ErrorKind.None;
		}
		
		bool CompareCase(CaseLabel label, GotoCaseStatement stmt)
		{
			if (label.IsDefault && stmt.IsDefaultCase)
				return true;
			
			if (stmt.Expression is PrimitiveExpression && label.Label is PrimitiveExpression) {
				PrimitiveExpression e1 = stmt.Expression as PrimitiveExpression;
				PrimitiveExpression e2 = label.Label as PrimitiveExpression;

				return object.Equals(e1.Value, e2.Value);
			}
			
			return false;
		}
	}
}
