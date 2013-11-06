//
// NegateIsExpressionAction.cs
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
	[ContextAction ("Negate 'is' expression", Description = "Negate an is expression.")]
	public class NegateIsExpressionAction : SpecializedCodeAction<IsExpression>
	{
		protected override CodeAction GetAction (RefactoringContext context, IsExpression node)
		{
			if (!node.IsToken.Contains(context.Location))
				return null;
			var pExpr = node.Parent as ParenthesizedExpression;
			if (pExpr != null) {
				var uOp = pExpr.Parent as UnaryOperatorExpression;
				if (uOp != null && uOp.Operator == UnaryOperatorType.Not) {
					return new CodeAction(
						string.Format(context.TranslateString("Negate '{0}'"), uOp),
						script => {
							script.Replace(uOp, node.Clone());
						}, 
						node.IsToken
					);
				}
			}

			return new CodeAction (
				string.Format (context.TranslateString ("Negate '{0}'"), node),
				script => {
					script.Replace (node, new UnaryOperatorExpression(UnaryOperatorType.Not, new ParenthesizedExpression(node.Clone())));
				}, 
				node.IsToken
			);
		}
	}
}
