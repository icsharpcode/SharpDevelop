//
// RedundantLambdaSignatureParenthesesIssue.cs
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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Redundant lambda signature parentheses",
	                  Description= "Redundant lambda signature parentheses",
	                  Category = IssueCategories.RedundanciesInCode,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "RedundantLambdaSignatureParentheses")]
	public class RedundantLambdaSignatureParenthesesIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantLambdaSignatureParenthesesIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitLambdaExpression(LambdaExpression lambdaExpression)
			{
				base.VisitLambdaExpression(lambdaExpression);
				var p = lambdaExpression.Parameters.FirstOrDefault();
				if (p == null || !p.Type.IsNull)
					return;
				var lParToken = lambdaExpression.LParToken;
				if (lParToken.IsNull)
					return;
				if (p.GetNextSibling(n => n.Role == Roles.Parameter) != null)
					return;
				Action<Script> action = script =>  {
					script.Remove(lParToken);
					script.Remove(lambdaExpression.RParToken);
					script.FormatText(lambdaExpression);
				};

				var issueText = ctx.TranslateString("Redundant lambda signature parentheses");
				var fixText = ctx.TranslateString("Remove signature parentheses");
				AddIssue(new CodeIssue(lambdaExpression.LParToken, issueText, fixText, action) { IssueMarker = IssueMarker.GrayOut });
				AddIssue(new CodeIssue(lambdaExpression.RParToken, issueText, fixText, action) { IssueMarker = IssueMarker.GrayOut });
			}
		}
	}
}

