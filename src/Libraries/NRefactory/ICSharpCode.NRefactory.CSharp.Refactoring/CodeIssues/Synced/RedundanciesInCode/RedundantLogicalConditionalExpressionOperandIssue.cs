//
// RedundantLogicalConditionalExpressionOperandIssue.cs
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
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Redundant operand in logical conditional expression",
	                  Description= "Redundant operand in logical conditional expression",
	                  Category = IssueCategories.RedundanciesInCode,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "RedundantLogicalConditionalExpressionOperand")]
	public class RedundantLogicalConditionalExpressionOperandIssue: GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantLogicalConditionalExpressionOperandIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base (ctx)
			{
			}

			static readonly AstNode pattern = new Choice {
				PatternHelper.CommutativeOperator(new NamedNode ("redundant", PatternHelper.OptionalParentheses(new PrimitiveExpression(true))), BinaryOperatorType.ConditionalOr, new AnyNode("expr")),	
				PatternHelper.CommutativeOperator(new NamedNode ("redundant", PatternHelper.OptionalParentheses(new PrimitiveExpression(false))), BinaryOperatorType.ConditionalAnd, new AnyNode("expr"))
			};

			public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
			{
				base.VisitBinaryOperatorExpression(binaryOperatorExpression);
				var match = pattern.Match(binaryOperatorExpression);
				if (!match.Success)
					return;
				var redundant = match.Get<Expression>("redundant").Single();
				var expr = match.Get<Expression>("expr").Single();
				AddIssue(new CodeIssue(
					redundant,
					ctx.TranslateString("Redundant operand in logical conditional expression"),
					ctx.TranslateString("Remove expression"),
					script => script.Replace(binaryOperatorExpression, expr.Clone())
				) { IssueMarker = IssueMarker.GrayOut });
			}
		}
	}
}

