//
// ReplaceWithSimpleAssignmentIssue.cs
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
using ICSharpCode.NRefactory.PatternMatching;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription(
		"Replace with simple assignment",
		Description = "Replace with simple assignment",
		Category = IssueCategories.PracticesAndImprovements,
		Severity = Severity.Suggestion,
		AnalysisDisableKeyword = "ReplaceWithSimpleAssignment")]
	public class ReplaceWithSimpleAssignmentIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ReplaceWithSimpleAssignmentIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			static readonly AstNode truePattern = 
				new AssignmentExpression(
					new AnyNode("expr"),
					AssignmentOperatorType.BitwiseOr,
					PatternHelper.OptionalParentheses(new PrimitiveExpression(true))
				);

			static readonly AstNode falsePattern = 
				new AssignmentExpression(
					new AnyNode("expr"),
					AssignmentOperatorType.BitwiseAnd,
					PatternHelper.OptionalParentheses(new PrimitiveExpression(false))
				);

			public override void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
			{
				base.VisitAssignmentExpression(assignmentExpression);
				var match = truePattern.Match(assignmentExpression);
				AssignmentExpression simpleAssignment;

				if (match.Success) {
					var expr = match.Get<Expression>("expr").Single();
					simpleAssignment = new AssignmentExpression(expr.Clone(), new PrimitiveExpression(true));
				} else {
					match = falsePattern.Match(assignmentExpression);
					if (!match.Success)
						return;
					var expr = match.Get<Expression>("expr").Single();
					simpleAssignment = new AssignmentExpression(expr.Clone(), new PrimitiveExpression(false));
				}

				AddIssue(new CodeIssue(
					assignmentExpression,
					ctx.TranslateString("Replace with simple assignment"),
					string.Format(ctx.TranslateString("Replace with '{0}'"), simpleAssignment),
					script => {
						script.Replace(assignmentExpression, simpleAssignment);
					}
				));
			}
		}
	}
}
