//
// SplitIfAction.cs
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
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction ("Split 'if' statement", 
	                Description = "Splits an if statement into two nested if statements")] 
	public class SplitIfAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions (RefactoringContext context)
		{
			var ifStatement = context.GetNode<IfElseStatement>();
			if (ifStatement == null)
				yield break;
			var bOp = ifStatement.GetNodeAt<BinaryOperatorExpression>(context.Location);
			if (bOp == null || !bOp.OperatorToken.Contains(context.Location))
				yield break;
			if (bOp.Ancestors.OfType<BinaryOperatorExpression>().Any(b => b.Operator != bOp.Operator))
				yield break;
			if (bOp.Operator == BinaryOperatorType.ConditionalAnd) {
				yield return CreateAndSplit(context, ifStatement, bOp);
			} else if (bOp.Operator == BinaryOperatorType.ConditionalOr) {
				yield return CreateOrSplit(context, ifStatement, bOp);
			}
		}

		static CodeAction CreateAndSplit(RefactoringContext context, IfElseStatement ifStatement, BinaryOperatorExpression bOp)
		{
			return new CodeAction(
				context.TranslateString("Split if"),
				script => {
					var nestedIf = (IfElseStatement)ifStatement.Clone();
					nestedIf.Condition = GetRightSide(bOp); 
					script.Replace(ifStatement.Condition, GetLeftSide(bOp));
					script.Replace(ifStatement.TrueStatement, new BlockStatement { nestedIf });
				},
				bOp.OperatorToken
			);
		}

		static CodeAction CreateOrSplit(RefactoringContext context, IfElseStatement ifStatement, BinaryOperatorExpression bOp)
		{
			return new CodeAction(
				context.TranslateString("Split if"),
				script => {
					var newElse = (IfElseStatement)ifStatement.Clone();
					newElse.Condition = GetRightSide(bOp); 
					
					var newIf = (IfElseStatement)ifStatement.Clone();
					newIf.Condition = GetLeftSide(bOp); 
					newIf.FalseStatement = newElse;

					script.Replace(ifStatement, newIf);
					script.FormatText(newIf);
				},
				bOp.OperatorToken
			);
		}

		internal static Expression GetRightSide(BinaryOperatorExpression expression)
		{
			var parent = expression.Parent as BinaryOperatorExpression;
			if (parent != null) {
				if (parent.Left == expression) {
					var parentClone = (BinaryOperatorExpression)parent.Clone();
					parentClone.Left = expression.Right.Clone();
					return parentClone;
				}

			}
			return expression.Right.Clone();
		}

		internal static Expression GetLeftSide(BinaryOperatorExpression expression)
		{
			var parent = expression.Parent as BinaryOperatorExpression;
			if (parent != null) {
				if (parent.Right == expression) {
					var parentClone = (BinaryOperatorExpression)parent.Clone();
					parentClone.Right = expression.Left.Clone();
					return parentClone;
				}
			}
			return expression.Left.Clone();
		}
	}
}

