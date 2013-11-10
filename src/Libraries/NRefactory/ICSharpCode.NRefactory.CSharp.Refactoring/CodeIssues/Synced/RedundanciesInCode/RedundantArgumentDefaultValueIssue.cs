//
// RedundantArgumentDefaultValueIssue.cs
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
		"Redundant argument with default value",
		Description = "The parameter is optional with the same default value",
		Category = IssueCategories.RedundanciesInCode,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "RedundantArgumentDefaultValue")]
	public class RedundantArgumentDefaultValueIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantArgumentDefaultValueIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			bool IsDefaultValue(Expression arg, ICSharpCode.NRefactory.TypeSystem.IParameter par)
			{
				var ne = arg as NamedArgumentExpression;
				if (ne != null) {
					if (ne.Name != par.Name)
						return false;
					arg = ne.Expression;
				}
				var cr = ctx.Resolve(arg);
				if (cr == null || !cr.IsCompileTimeConstant || !par.IsOptional)
					return false;
				return 
					cr.ConstantValue == null && par.ConstantValue == null || 
					cr.ConstantValue.Equals(par.ConstantValue);
			}

			public override void VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression)
			{
				base.VisitObjectCreateExpression(objectCreateExpression);
				Check(objectCreateExpression, objectCreateExpression.Arguments.ToList());
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);
				Check(invocationExpression, invocationExpression.Arguments.ToList());
			}

			public override void VisitIndexerExpression(IndexerExpression indexerExpression)
			{
				base.VisitIndexerExpression(indexerExpression);
				Check(indexerExpression, indexerExpression.Arguments.ToList());
			}

			void Check(AstNode invocationExpression, List<Expression> arguments)
			{
				var rr = ctx.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (rr == null || rr.IsError)
					return;

				for (int i = 0; i < arguments.Count && i < rr.Member.Parameters.Count; i++) {

					if (!IsDefaultValue(arguments[i], rr.Member.Parameters[i]))
						continue;
					bool nextAreAllDefault = true;
					for (int j = i + 1; j < arguments.Count && j < rr.Member.Parameters.Count; j++) {
						if (!IsDefaultValue(arguments[j], rr.Member.Parameters[j])) {
							nextAreAllDefault = false;
							break;
						}
					}
					if (!nextAreAllDefault)
						continue;
					for (int j = i; j < arguments.Count && j < rr.Member.Parameters.Count; j++) {
						var _i = j;
						AddIssue(new CodeIssue(
							arguments[j],
							i + 1 < arguments.Count ? ctx.TranslateString("Argument values are redundant") : ctx.TranslateString("Argument value is redundant"),
							i + 1 < arguments.Count ? ctx.TranslateString("Remove redundant arguments") : ctx.TranslateString("Remove redundant argument"),
							script => {
								var invoke = invocationExpression.Clone();

								var argCollection = invoke.GetChildrenByRole<Expression>(Roles.Argument);
								argCollection.Clear();
								for (int k = 0; k < _i; k++)
									argCollection.Add(arguments[k].Clone());
								script.Replace(invocationExpression, invoke);
							}
						) { IssueMarker = IssueMarker.GrayOut });
					}
					break;
				}
			}
		}
	}
}