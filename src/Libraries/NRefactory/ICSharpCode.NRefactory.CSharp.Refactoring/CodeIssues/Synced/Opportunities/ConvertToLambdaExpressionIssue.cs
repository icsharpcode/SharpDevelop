//
// ConvertToLambdaExpressionIssue.cs
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

using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Convert to lambda expression",
		Description = "Convert to lambda with expression",
		Category = IssueCategories.Opportunities,
		Severity = Severity.Suggestion,
		AnalysisDisableKeyword = "ConvertToLambdaExpression")]
	public class ConvertToLambdaExpressionIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ConvertToLambdaExpressionIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base(ctx)
			{
			}

			public override void VisitLambdaExpression(LambdaExpression lambdaExpression)
			{
				base.VisitLambdaExpression(lambdaExpression);
				BlockStatement block;
				Expression expr;
				if (!ConvertLambdaBodyStatementToExpressionAction.TryGetConvertableExpression(lambdaExpression.Body, out block, out expr))
					return;
				var node = block.Statements.FirstOrDefault() ?? block;
				var expressionStatement = node as ExpressionStatement;
				if (expressionStatement != null) {
					if (expressionStatement.Expression is AssignmentExpression)
						return;
				}
				var returnTypes = new List<IType>();
				foreach (var type in TypeGuessing.GetValidTypes(ctx.Resolver, lambdaExpression)) {
					if (type.Kind != TypeKind.Delegate)
						continue;
					var invoke = type.GetDelegateInvokeMethod();
					if (!returnTypes.Contains(invoke.ReturnType))
						returnTypes.Add(invoke.ReturnType);
				}
				if (returnTypes.Count > 1)
					return;

				AddIssue(new CodeIssue(
					node,
					ctx.TranslateString("Can be converted to expression"),
					ConvertLambdaBodyStatementToExpressionAction.CreateAction(ctx, node, block, expr)
				));
			}

			public override void VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
			{
				base.VisitAnonymousMethodExpression(anonymousMethodExpression);
				if (!anonymousMethodExpression.HasParameterList)
					return;
				BlockStatement block;
				Expression expr;
				if (!ConvertLambdaBodyStatementToExpressionAction.TryGetConvertableExpression(anonymousMethodExpression.Body, out block, out expr))
					return;
				var node = block.Statements.FirstOrDefault() ?? block;
				var returnTypes = new List<IType>();
				foreach (var type in TypeGuessing.GetValidTypes(ctx.Resolver, anonymousMethodExpression)) {
					if (type.Kind != TypeKind.Delegate)
						continue;
					var invoke = type.GetDelegateInvokeMethod();
					if (!returnTypes.Contains(invoke.ReturnType))
						returnTypes.Add(invoke.ReturnType);
				}
				if (returnTypes.Count > 1)
					return;

				AddIssue(new CodeIssue(
					node,
					ctx.TranslateString("Can be converted to expression"),
					ctx.TranslateString("Convert to lambda expression"),
					script => {
						var lambdaExpression = new LambdaExpression();
						foreach (var parameter in anonymousMethodExpression.Parameters)
							lambdaExpression.Parameters.Add(parameter.Clone());
						lambdaExpression.Body = expr.Clone();
						script.Replace(anonymousMethodExpression, lambdaExpression);
					}
				));
			}
		}
	}
}
