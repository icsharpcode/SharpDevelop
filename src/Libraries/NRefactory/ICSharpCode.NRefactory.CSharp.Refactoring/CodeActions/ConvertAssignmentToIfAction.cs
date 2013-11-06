//
// ConvertAssignmentToIfAction.cs
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Convert assignment to 'if'",
	               Description = "Convert assignment to 'if'")]
	public class ConvertAssignmentToIfAction : SpecializedCodeAction<AssignmentExpression>
	{
		protected override CodeAction GetAction(RefactoringContext context, AssignmentExpression node)
		{
			if (!node.OperatorToken.Contains(context.Location) || !(node.Parent is ExpressionStatement))
				return null;

			if (node.Right is ConditionalExpression)
				return CreateForConditionalExpression(context, node, (ConditionalExpression)node.Right);

			var bOp = node.Right as BinaryOperatorExpression;
			if (bOp != null && bOp.Operator == BinaryOperatorType.NullCoalescing)
				return CreateForNullCoalesingExpression(context, node, bOp);
			return null;
		}

		static CodeAction CreateForConditionalExpression(RefactoringContext ctx, AssignmentExpression node, ConditionalExpression conditionalExpression)
		{
			return new CodeAction (
				ctx.TranslateString("Replace with 'if' statement"),
				script => {
					var ifStatement = new IfElseStatement(
						conditionalExpression.Condition.Clone(),
						new AssignmentExpression(node.Left.Clone(), node.Operator, conditionalExpression.TrueExpression.Clone()),
						new AssignmentExpression(node.Left.Clone(), node.Operator, conditionalExpression.FalseExpression.Clone())
					);
					script.Replace(node.Parent, ifStatement); 
				},
				node.OperatorToken
			);
		}

		static CodeAction CreateForNullCoalesingExpression(RefactoringContext ctx, AssignmentExpression node, BinaryOperatorExpression bOp)
		{
			return new CodeAction (
				ctx.TranslateString("Replace with 'if' statement"),
				script => {
					var ifStatement = new IfElseStatement(
						new BinaryOperatorExpression(bOp.Left.Clone(), BinaryOperatorType.InEquality, new NullReferenceExpression()), 
						new AssignmentExpression(node.Left.Clone(), node.Operator, bOp.Left.Clone()),
						new AssignmentExpression(node.Left.Clone(), node.Operator, bOp.Right.Clone())
					);
					script.Replace(node.Parent, ifStatement); 
				},
				node.OperatorToken
			);
		}

	}
}

