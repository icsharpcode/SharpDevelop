//
// PossibleMistakenCallToGetTypeIssue.cs
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
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Possible mistaken call to 'object.GetType()'",
		Description = "Possible mistaken call to 'object.GetType()'",
		Category = IssueCategories.PracticesAndImprovements,
		Severity = Severity.Warning, 
		AnalysisDisableKeyword = "PossibleMistakenCallToGetType" )]
	public class PossibleMistakenCallToGetTypeIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<PossibleMistakenCallToGetTypeIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);

				var mref = invocationExpression.Target as MemberReferenceExpression;
				if (mref == null || mref.MemberName != "GetType")
					return;
				var rr = ctx.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (rr == null || !rr.Member.DeclaringType.IsKnownType(KnownTypeCode.Type) || rr.Member.IsStatic)
					return;
				AddIssue(new CodeIssue (
					invocationExpression,
					ctx.TranslateString("Possible mistaken call to 'object.GetType()'"),
					ctx.TranslateString("Remove call to 'object.GetType()'"),
					s => s.Replace(invocationExpression, mref.Target.Clone())
				));
			}
		}
	}
}

