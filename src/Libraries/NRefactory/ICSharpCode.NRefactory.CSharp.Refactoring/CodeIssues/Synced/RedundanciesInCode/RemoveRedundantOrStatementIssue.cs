//
// RemoveRedundantOrStatementIssue.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
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
using ICSharpCode.NRefactory.Refactoring;
using System.Linq;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription(
		"Remove redundant statement",
		Description = "Remove redundant statement",
		Category = IssueCategories.RedundanciesInCode,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "RemoveRedundantOrStatement")]
	public class RemoveRedundantOrStatementIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RemoveRedundantOrStatementIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			readonly AstNode pattern = new ExpressionStatement (
				new Choice {
					new AssignmentExpression(new AnyNode(), AssignmentOperatorType.BitwiseOr, new PrimitiveExpression(false)),
					new AssignmentExpression(new AnyNode(), AssignmentOperatorType.BitwiseAnd, new PrimitiveExpression(true))
				}
			);

			public override void VisitExpressionStatement(ExpressionStatement expressionStatement)
			{
				base.VisitExpressionStatement(expressionStatement);
				if (pattern.IsMatch(expressionStatement)) {
					AddIssue(new CodeIssue(
						expressionStatement,
						ctx.TranslateString("Statement is redundant"),
						ctx.TranslateString("Remove redundant statement"),
						s => s.Remove(expressionStatement)
					) { IssueMarker = IssueMarker.GrayOut });
				}
			}
		}
	}
}

