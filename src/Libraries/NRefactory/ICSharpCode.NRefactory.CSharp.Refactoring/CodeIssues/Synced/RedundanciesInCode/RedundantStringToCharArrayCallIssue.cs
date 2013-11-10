//
// RedundantStringToCharArrayCallIssue.cs
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
using System.Linq;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Redundant 'string.ToCharArray()' call",
		Description = "Redundant 'string.ToCharArray()' call",
		Category = IssueCategories.RedundanciesInCode,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "RedundantStringToCharArrayCall")]
	public class RedundantStringToCharArrayCallIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantStringToCharArrayCallIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base (ctx)
			{
			}

			void AddProblem(AstNode replaceExpression, InvocationExpression invocationExpression)
			{
				var t = invocationExpression.Target as MemberReferenceExpression;
				AddIssue(new CodeIssue (
					t.DotToken.StartLocation,
					t.MemberNameToken.EndLocation,
					ctx.TranslateString("Redundant 'string.ToCharArray()' call"),
					ctx.TranslateString("Remove redundant 'string.ToCharArray()' call"),
					s => s.Replace(replaceExpression, t.Target.Clone())
				) { IssueMarker = IssueMarker.GrayOut });
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);

				var t = invocationExpression.Target as MemberReferenceExpression;
				if (t == null || t.MemberName != "ToCharArray")
					return;
				var rr = ctx.Resolve(t.Target);
				if (!rr.Type.IsKnownType(KnownTypeCode.String))
					return;
				if (invocationExpression.Parent is ForeachStatement && invocationExpression.Role == Roles.Expression) {
					AddProblem(invocationExpression, invocationExpression);
					return;
				}
				var p = invocationExpression.Parent;
				while (p is ParenthesizedExpression) {
					p = p.Parent;
				}
				var idx = p as IndexerExpression;
				if (idx != null) {
					AddProblem(idx.Target, invocationExpression);
					return;
				}
			}
		}
	}
}

