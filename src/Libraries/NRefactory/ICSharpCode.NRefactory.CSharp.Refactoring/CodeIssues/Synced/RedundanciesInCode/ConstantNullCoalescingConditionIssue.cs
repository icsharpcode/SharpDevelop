//
// RedundantNullCoalescingExpressionIssue.cs
//
// Author:
//       Luís Reis <luiscubal@gmail.com>
//
// Copyright (c) 2013 Luís Reis
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
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.PatternMatching;
using System.Runtime.InteropServices.ComTypes;
using ICSharpCode.NRefactory.CSharp.Analysis;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("'??' condition is known to be null or not null",
	                  Description = "Finds redundant null coalescing expressions such as expr ?? expr",
	                  Category = IssueCategories.RedundanciesInCode,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "ConstantNullCoalescingCondition")]
	public class ConstantNullCoalescingConditionIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ConstantNullCoalescingConditionIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			Dictionary<AstNode, NullValueAnalysis> cachedNullAnalysis = new Dictionary<AstNode, NullValueAnalysis>();

			public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
			{
				base.VisitBinaryOperatorExpression(binaryOperatorExpression);

				if (binaryOperatorExpression.Operator != BinaryOperatorType.NullCoalescing) {
					//The issue is not applicable
					return;
				}

				var parentFunction = GetParentFunctionNode(binaryOperatorExpression);
				var analysis = GetAnalysis(parentFunction);

				NullValueStatus leftStatus = analysis.GetExpressionResult(binaryOperatorExpression.Left);
				if (leftStatus == NullValueStatus.DefinitelyNotNull) {
					AddIssue(new CodeIssue(binaryOperatorExpression.OperatorToken.StartLocation,
					         binaryOperatorExpression.Right.EndLocation,
					         ctx.TranslateString("Redundant ??. Left side is never null."),
					         ctx.TranslateString("Remove redundant right side"),
					         script => {

						script.Replace(binaryOperatorExpression, binaryOperatorExpression.Left.Clone());

						}) { IssueMarker = IssueMarker.GrayOut });
					return;
				}
				if (leftStatus == NullValueStatus.DefinitelyNull) {
					AddIssue(new CodeIssue(binaryOperatorExpression.Left.StartLocation,
					         binaryOperatorExpression.OperatorToken.EndLocation,
					         ctx.TranslateString("Redundant ??. Left side is always null."),
					         ctx.TranslateString("Remove redundant left side"),
					         script => {

						script.Replace(binaryOperatorExpression, binaryOperatorExpression.Right.Clone());

						}));
					return;
				}
				NullValueStatus rightStatus = analysis.GetExpressionResult(binaryOperatorExpression.Right);
				if (rightStatus == NullValueStatus.DefinitelyNull) {
					AddIssue(new CodeIssue(binaryOperatorExpression.OperatorToken.StartLocation,
					         binaryOperatorExpression.Right.EndLocation,
					         ctx.TranslateString("Redundant ??. Right side is always null."),
					         ctx.TranslateString("Remove redundant right side"),
					         script => {

						script.Replace(binaryOperatorExpression, binaryOperatorExpression.Left.Clone());

						}));
					return;
				}
			}

			NullValueAnalysis GetAnalysis(AstNode parentFunction)
			{
				NullValueAnalysis analysis;
				if (cachedNullAnalysis.TryGetValue(parentFunction, out analysis)) {
					return analysis;
				}

				analysis = new NullValueAnalysis(ctx, parentFunction.GetChildByRole(Roles.Body), parentFunction.GetChildrenByRole(Roles.Parameter), ctx.CancellationToken);
				analysis.IsParametersAreUninitialized = true;
				analysis.Analyze();
				cachedNullAnalysis [parentFunction] = analysis;
				return analysis;
			}
		}

		public static AstNode GetParentFunctionNode(AstNode node)
		{
			do {
				node = node.Parent;
			} while (node != null && !IsFunctionNode(node));

			return node;
		}

		static bool IsFunctionNode(AstNode node)
		{
			return node is EntityDeclaration ||
				node is LambdaExpression ||
					node is AnonymousMethodExpression;
		}
	}
}