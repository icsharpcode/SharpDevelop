// 
// RedundantArgumentNameIssue.cs
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
using ICSharpCode.NRefactory.Semantics;
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{ 
	[IssueDescription("Redundant explicit argument name specification",
	                  Description= "Redundant explicit argument name specification",
	                  Category = IssueCategories.RedundanciesInCode,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "RedundantArgumentName")]
	public class RedundantArgumentNameIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}
		
		class GatherVisitor : GatherVisitorBase<RedundantArgumentNameIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base (ctx)
			{
			}

			void CheckParameters(ResolveResult resolveResult, AstNodeCollection<Expression> arguments)
			{
				var ir = resolveResult as InvocationResolveResult;
				if (ir == null || ir.IsError)
					return;
				int i = 0;
				foreach (var arg in arguments) {
					var na = arg as NamedArgumentExpression;
					if (na != null) {
						if (na.Name != ir.Member.Parameters[i].Name)
							break;
						var _i = i;
						AddIssue(new CodeIssue(
							na.NameToken.StartLocation,
							na.ColonToken.EndLocation,
							ctx.TranslateString("Redundant argument name specification"), 
							ctx.TranslateString("Remove argument name specification"),
							script => {
								foreach (var node in arguments.Take(_i + 1).OfType<NamedArgumentExpression>()) {
									script.Replace(node, node.Expression.Clone());
								}
							}
						) { IssueMarker = IssueMarker.GrayOut });
					}
					i++;
				}
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);
				CheckParameters(ctx.Resolve(invocationExpression), invocationExpression.Arguments);
			}

			public override void VisitIndexerExpression(IndexerExpression indexerExpression)
			{
				base.VisitIndexerExpression(indexerExpression);
				CheckParameters(ctx.Resolve(indexerExpression), indexerExpression.Arguments);
			}

			public override void VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression)
			{
				base.VisitObjectCreateExpression(objectCreateExpression);
				CheckParameters(ctx.Resolve(objectCreateExpression), objectCreateExpression.Arguments);
			}

			public override void VisitAttribute(Attribute attribute)
			{
				base.VisitAttribute(attribute);
				CheckParameters(ctx.Resolve(attribute), attribute.Arguments);
			}
		}
	}
}