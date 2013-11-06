//
// RedundantCatchIssue.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Redundant catch clause",
                      Description = "Catch clause with a single 'throw' statement is redundant.",
	                  Category = IssueCategories.RedundanciesInCode,
	                  Severity = Severity.Hint,
                      AnalysisDisableKeyword = "RedundantCatchClause")]
	public class RedundantCatchClauseIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}
		
		class GatherVisitor : GatherVisitorBase<RedundantCatchClauseIssue>
		{
			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
			}

			public override void VisitTryCatchStatement(TryCatchStatement tryCatchStatement)
			{
				var redundantCatchClauses = new List<CatchClause>();
				bool hasNonRedundantCatch = false;
				foreach (var catchClause in tryCatchStatement.CatchClauses) {
					if (IsRedundant(catchClause)) {
						redundantCatchClauses.Add(catchClause);
					} else {
						hasNonRedundantCatch = true;
					}
				}

				if (hasNonRedundantCatch || !tryCatchStatement.FinallyBlock.IsNull) {
					AddIssuesForClauses(tryCatchStatement, redundantCatchClauses);
				} else {
					AddIssueForTryCatchStatement(tryCatchStatement);
				}
			}

			void AddIssuesForClauses(AstNode node, List<CatchClause> redundantCatchClauses)
			{
				var allCatchClausesMessage = ctx.TranslateString("Remove all '{0}' redundant 'catch' clauses");
				var removeAllRedundantClausesAction = new CodeAction(allCatchClausesMessage, script => {
					foreach (var redundantCatchClause in redundantCatchClauses) {
						script.Remove(redundantCatchClause);
					}
				}, node);
				var singleCatchClauseMessage = ctx.TranslateString("Remove 'catch'");
                var redundantCatchClauseMessage = ctx.TranslateString("Catch clause with a single 'throw' statement is redundant.");
				foreach (var redundantCatchClause in redundantCatchClauses) {
					var closureLocalCatchClause = redundantCatchClause;
					var removeRedundantClauseAction = new CodeAction(singleCatchClauseMessage, script => {
						script.Remove(closureLocalCatchClause);
					}, node);
					var actions = new List<CodeAction>();
					actions.Add(removeRedundantClauseAction);
					if (redundantCatchClauses.Count > 1) {
						actions.Add(removeAllRedundantClausesAction);
					}
					AddIssue(new CodeIssue(closureLocalCatchClause, redundantCatchClauseMessage, actions) { IssueMarker = IssueMarker.GrayOut });
				}
			}

			void AddIssueForTryCatchStatement(TryCatchStatement tryCatchStatement)
			{
				var lastCatch = tryCatchStatement.CatchClauses.LastOrNullObject();
				if (lastCatch.IsNull)
					return;

				var removeTryCatchMessage = ctx.TranslateString("Remove 'try' statement");

				var removeTryStatementAction = new CodeAction(removeTryCatchMessage, script => {
					var statements = tryCatchStatement.TryBlock.Statements;
					if (statements.Count == 1 || tryCatchStatement.Parent is BlockStatement) {
						foreach (var statement in statements) {
							script.InsertAfter(tryCatchStatement.GetPrevSibling (s => !(s is NewLineNode)), statement.Clone());
						}
						script.Remove(tryCatchStatement);
					} else {
						var blockStatement = new BlockStatement();
						foreach (var statement in statements) {
							blockStatement.Statements.Add(statement.Clone());
						}
						script.Replace(tryCatchStatement, blockStatement);
					}
					// The replace and insert script functions does not format these things well on their own
					script.FormatText(tryCatchStatement.Parent);
				}, tryCatchStatement);

				var fixes = new [] {
					removeTryStatementAction
				};
				AddIssue(new CodeIssue(tryCatchStatement.TryBlock.EndLocation, lastCatch.EndLocation, removeTryCatchMessage, fixes) { IssueMarker = IssueMarker.GrayOut });
			}

			static bool IsThrowsClause (CatchClause catchClause)
			{
				var firstStatement = catchClause.Body.Statements.FirstOrNullObject();
				if (firstStatement.IsNull)
					return false;
				var throwStatement = firstStatement as ThrowStatement;
				if (throwStatement == null || !throwStatement.Expression.IsNull)
					return false;
				return true;
			}

			bool IsRedundant(CatchClause catchClause)
			{
				if (!IsThrowsClause (catchClause))
					return false;

				var type = ctx.Resolve (catchClause.Type).Type;
				var n = catchClause.NextSibling;
				while (n != null) {
					var nextClause = n as CatchClause;
					if (nextClause != null) {
						if (nextClause.Type.IsNull && !IsThrowsClause(nextClause))
							return false;
						if (!IsThrowsClause(nextClause) && type.GetDefinition ().IsDerivedFrom (ctx.Resolve (nextClause.Type).Type.GetDefinition ()))
							return false;
					}
					n = n.NextSibling;
				}
				return true;
			}
		}
	}
}

