//
// RedundantAnonymousTypePropertyNameIssue.cs
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
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription(
		"Redundant anonymous type property name",
		Description = "The name can be inferred from the initializer expression",
		Category = IssueCategories.RedundanciesInCode,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "RedundantAnonymousTypePropertyName")]
	public class RedundantAnonymousTypePropertyNameIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantAnonymousTypePropertyNameIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			static string GetAnonymousTypePropertyName(Expression expr)
			{
				if (expr is MemberReferenceExpression) {
					return ((MemberReferenceExpression)expr).MemberName;
				}
				if (expr is IdentifierExpression) {
					return ((IdentifierExpression)expr).Identifier;
				}
				return null;
			}

			public override void VisitAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression)
			{
				base.VisitAnonymousTypeCreateExpression(anonymousTypeCreateExpression);
				foreach (var expr in anonymousTypeCreateExpression.Initializers) {
					var na = expr as NamedExpression;
					if (na == null)
						continue;
					if (na.Name == GetAnonymousTypePropertyName(na.Expression)) {
						AddIssue(new CodeIssue(na.StartLocation, na.AssignToken.EndLocation,
							ctx.TranslateString("Redundant explicit property name"),
							ctx.TranslateString("Remove redundant name"),
							s => s.Replace(na, na.Expression.Clone())
						) { IssueMarker = IssueMarker.GrayOut });
					}
				}
			}
		}
	}
}

