//
// ConvertBitwiseFlagComparisonToHasFlagsAction.cs
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
using ICSharpCode.NRefactory.PatternMatching;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction ("Replace bitwise flag comparison with call to 'Enum.HasFlag'", Description = "Replace bitwise flag comparison with call to 'Enum.HasFlag'")]
	public class ConvertBitwiseFlagComparisonToHasFlagsAction : SpecializedCodeAction<BinaryOperatorExpression>
	{
		static readonly AstNode truePattern = new Choice {
			new BinaryOperatorExpression (new ParenthesizedExpression(new BinaryOperatorExpression (PatternHelper.OptionalParentheses(new AnyNode("target")), BinaryOperatorType.BitwiseAnd, PatternHelper.OptionalParentheses(new AnyNode("expr")))), BinaryOperatorType.InEquality, PatternHelper.OptionalParentheses(new PrimitiveExpression(0))),
			new BinaryOperatorExpression (new ParenthesizedExpression(new BinaryOperatorExpression (PatternHelper.OptionalParentheses(new AnyNode("target")), BinaryOperatorType.BitwiseAnd, PatternHelper.OptionalParentheses(new AnyNode("expr")))), BinaryOperatorType.Equality, PatternHelper.OptionalParentheses(new Backreference("expr")))
		};

		static readonly AstNode falsePattern = new Choice {
			new BinaryOperatorExpression (new ParenthesizedExpression(new BinaryOperatorExpression (PatternHelper.OptionalParentheses(new AnyNode("target")), BinaryOperatorType.BitwiseAnd, PatternHelper.OptionalParentheses(new AnyNode("expr")))), BinaryOperatorType.Equality, PatternHelper.OptionalParentheses(new PrimitiveExpression(0))),
			new BinaryOperatorExpression (new ParenthesizedExpression(new BinaryOperatorExpression (PatternHelper.OptionalParentheses(new AnyNode("target")), BinaryOperatorType.BitwiseAnd, PatternHelper.OptionalParentheses(new AnyNode("expr")))), BinaryOperatorType.InEquality, PatternHelper.OptionalParentheses(new Backreference("expr")))
		};

		internal static Expression MakeFlatExpression (Expression expr, BinaryOperatorType opType)
		{
			var bOp = expr as BinaryOperatorExpression;
			if (bOp == null)
				return expr.Clone();
			return new BinaryOperatorExpression(
				MakeFlatExpression(bOp.Left, opType),
				opType,
				MakeFlatExpression(bOp.Right, opType)
			);
		}

		static Expression BuildHasFlagExpression (Expression target, Expression expr)
		{
			var bOp = expr as BinaryOperatorExpression;
			if (bOp == null)
				return new InvocationExpression(new MemberReferenceExpression(target.Clone(), "HasFlag"), expr.Clone());

			if (bOp.Operator == BinaryOperatorType.BitwiseOr) {
				return new BinaryOperatorExpression(
					BuildHasFlagExpression(target, bOp.Left),
					BinaryOperatorType.BitwiseOr,
					BuildHasFlagExpression(target, bOp.Right)
				);
			}

			return new InvocationExpression(new MemberReferenceExpression(target.Clone(), "HasFlag"), MakeFlatExpression (bOp, BinaryOperatorType.BitwiseOr));
		}

		static CodeAction CreateAction(BaseRefactoringContext context, Match match, bool negateMatch, BinaryOperatorExpression node)
		{
			var target = match.Get<Expression>("target").Single();
			var expr = match.Get<Expression>("expr").Single();

			if (!expr.DescendantsAndSelf.All(x => !(x is BinaryOperatorExpression) || ((BinaryOperatorExpression)x).Operator == BinaryOperatorType.BitwiseOr) &&
				!expr.DescendantsAndSelf.All(x => !(x is BinaryOperatorExpression) || ((BinaryOperatorExpression)x).Operator == BinaryOperatorType.BitwiseAnd))
				return null;
			var rr = context.Resolve(target);
			if (rr.Type.Kind != ICSharpCode.NRefactory.TypeSystem.TypeKind.Enum)
				return null;

			return new CodeAction(
				context.TranslateString("Replace with 'Enum.HasFlag'"), 
				script => {
					Expression newExpr = BuildHasFlagExpression (target, expr);
					if (negateMatch)
						newExpr = new UnaryOperatorExpression(UnaryOperatorType.Not, newExpr);
					script.Replace(node, newExpr);
				}, 
				node.OperatorToken);
		}

		protected override CodeAction GetAction(RefactoringContext context, BinaryOperatorExpression node)
		{
			if (!node.OperatorToken.Contains(context.Location))
				return null;
			var match = truePattern.Match(node);
			if (match.Success)
				return CreateAction(context, match, false, node);
			match = falsePattern.Match(node);
			if (match.Success)
				return CreateAction(context, match, true, node);
			return null;
		}
	}
}

