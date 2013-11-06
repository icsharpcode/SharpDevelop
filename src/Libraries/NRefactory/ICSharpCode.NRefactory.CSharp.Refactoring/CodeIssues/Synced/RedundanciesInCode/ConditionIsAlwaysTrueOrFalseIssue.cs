//
// ConditionIsAlwaysTrueOrFalseIssue.cs
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
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.PatternMatching;
using System.Runtime.InteropServices.ComTypes;
using ICSharpCode.NRefactory.CSharp.Analysis;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Expression is always 'true' or always 'false'",
		Description = "Value of the expression can be determined at compile time",
		Category = IssueCategories.RedundanciesInCode,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "ConditionIsAlwaysTrueOrFalse")]
	public class ConditionIsAlwaysTrueOrFalseIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ConditionIsAlwaysTrueOrFalseIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
			{
				if (CheckConstant(binaryOperatorExpression))
					return;
				if (CSharpUtil.GetInnerMostExpression(binaryOperatorExpression.Left) is NullReferenceExpression) {
					if (CheckNullComparison(binaryOperatorExpression, binaryOperatorExpression.Right, binaryOperatorExpression.Left))
						return;
				}

				if (CSharpUtil.GetInnerMostExpression(binaryOperatorExpression.Right) is NullReferenceExpression) {
					if (CheckNullComparison(binaryOperatorExpression, binaryOperatorExpression.Left, binaryOperatorExpression.Right))
						return;
				}

				base.VisitBinaryOperatorExpression(binaryOperatorExpression);
			}

			bool CheckNullComparison(BinaryOperatorExpression binaryOperatorExpression, Expression right, Expression nullNode)
			{
				if (binaryOperatorExpression.Operator != BinaryOperatorType.Equality && binaryOperatorExpression.Operator != BinaryOperatorType.InEquality)
					return false;
				// note null == null is checked by similiar expression comparison.
				var expr = CSharpUtil.GetInnerMostExpression(right);

				var rr = ctx.Resolve(expr);
				if (rr.Type.IsReferenceType == false) {
					// nullable check
					if (NullableType.IsNullable(rr.Type))
						return false;

					var conversion = ctx.GetConversion(nullNode);
					if (conversion.ConversionAfterUserDefinedOperator == Conversion.IdentityConversion)
						return false;

					// check for user operators
					foreach (var op in rr.Type.GetMethods(m => m.SymbolKind == SymbolKind.Operator && m.Parameters.Count == 2)) {
						if (op.Parameters[0].Type.IsReferenceType == false && op.Parameters[1].Type.IsReferenceType == false)
							continue;
						if (binaryOperatorExpression.Operator == BinaryOperatorType.Equality && op.Name == "op_Equality")
							return false;
						if (binaryOperatorExpression.Operator == BinaryOperatorType.InEquality && op.Name == "op_Inequality")
							return false;
					}

					AddIssue(binaryOperatorExpression, binaryOperatorExpression.Operator != BinaryOperatorType.Equality);
					return true;
				}
				return false;
			}

			void AddIssue(Expression expr, bool result)
			{
				AddIssue(new CodeIssue(
					expr, 
					string.Format(ctx.TranslateString("Expression is always '{0}'"), result ? "true" : "false"), 
					string.Format(ctx.TranslateString("Replace with '{0}'"), result ? "true" : "false"), 
					s => s.Replace(expr, new PrimitiveExpression(result))
				));
			}

			bool CheckConstant(Expression expr)
			{
				var rr = ctx.Resolve(expr);
				if (rr.ConstantValue is bool) {
					var result = (bool)rr.ConstantValue;
					AddIssue(expr, result);
					return true;
				}
				return false;
			}

			public override void VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression)
			{
				if (CheckConstant(unaryOperatorExpression))
					return;
				base.VisitUnaryOperatorExpression(unaryOperatorExpression);
			}
		}
	}
}

