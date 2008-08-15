/*
 * Created by SharpDevelop.
 * User: HP
 * Date: 01.05.2008
 * Time: 20:37
 */

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using Dom = ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Document;

namespace SharpRefactoring.Visitors
{
	/// <summary>
	/// Description of FindJumpInstructionVisitor.
	/// </summary>
	public class FindJumpInstructionsVisitor : AbstractAstVisitor
	{
		MethodDeclaration method;
		ISelection selection;
		List<LabelStatement> labels;
		List<CaseLabel> cases;
		bool isOk = true;
		
		public bool IsOk {
			get { return isOk; }
		}
		
		public FindJumpInstructionsVisitor(MethodDeclaration method, ISelection selection)
		{
			this.method = method;
			this.selection = selection;
			this.labels = new List<LabelStatement>();
			this.cases = new List<CaseLabel>();
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
			if (!(data is bool)) {
				this.isOk = false;
				MessageService.ShowError("Selection contains a 'break' statement without the enclosing loop.");
			}
			return base.VisitBreakStatement(breakStatement, data);
		}
		
		public override object VisitContinueStatement(ContinueStatement continueStatement, object data)
		{
			if (!(data is bool)) {
				this.isOk = false;
				MessageService.ShowError("Selection contains a 'continue' statement without the enclosing loop.");
			}
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
			this.isOk = false;
			foreach (CaseLabel label in this.cases) {
				if (label.Label.ToString() == gotoCaseStatement.Expression.ToString())
					this.isOk = true;
			}
			if (!this.isOk) {
				MessageService.ShowError("Case section '" + ((PrimitiveExpression)gotoCaseStatement.Expression).StringValue + "' not found inside the selected range!");
			}

			return base.VisitGotoCaseStatement(gotoCaseStatement, data);
		}
		
		public override object VisitGotoStatement(GotoStatement gotoStatement, object data)
		{
			this.isOk = false;
			foreach (LabelStatement label in this.labels) {
				if (label.Label == gotoStatement.Label)
					this.isOk = true;
			}
			if (!this.isOk) {
				MessageService.ShowError("Label '" + gotoStatement.Label + "' not found inside the selected range!");
			}
			
			return base.VisitGotoStatement(gotoStatement, data);
		}
	}
}
