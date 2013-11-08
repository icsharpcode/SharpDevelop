//
// RedundantDelegateCreationIssue.cs
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
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription (
		"Explicit delegate creation expression is redundant",
		Description = "Explicit delegate creation expression is redundant",
		Category = IssueCategories.RedundanciesInCode,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "RedundantDelegateCreation")]
	public class RedundantDelegateCreationIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantDelegateCreationIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
			{
				base.VisitAssignmentExpression(assignmentExpression);
				if (assignmentExpression.Operator != AssignmentOperatorType.Add && assignmentExpression.Operator != AssignmentOperatorType.Subtract)
					return;
				var oce = assignmentExpression.Right as ObjectCreateExpression;
				if (oce == null || oce.Arguments.Count != 1)
					return;
				var left = ctx.Resolve(assignmentExpression.Left) as MemberResolveResult;
				if (left == null || left.Member.SymbolKind != SymbolKind.Event)
					return;
				var right = ctx.Resolve(assignmentExpression.Right);
				if (right.IsError || !Equals(left.Type, right.Type))
					return;
				AddIssue(new CodeIssue(oce.StartLocation, oce.Type.EndLocation,
					ctx.TranslateString("Redundant explicit delegate declaration"),
					ctx.TranslateString("Remove redundant 'new'"),
					s => s.Replace(assignmentExpression.Right, oce.Arguments.First())
				) { IssueMarker = IssueMarker.GrayOut });
			}
		}
	}
}

