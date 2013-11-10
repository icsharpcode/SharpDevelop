//
// ConvertReturnStatementToIfAction.cs
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
using ICSharpCode.NRefactory.PatternMatching;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Convert 'return' to 'if'",
	               Description = "Convert 'return' to 'if'")]
	public class ConvertReturnStatementToIfAction : SpecializedCodeAction <ReturnStatement>
	{
		protected override CodeAction GetAction(RefactoringContext context, ReturnStatement node)
		{
			if (!node.ReturnToken.Contains(context.Location))
				return null;

			if (node.Expression is ConditionalExpression)
				return CreateForConditionalExpression(context, node, (ConditionalExpression)node.Expression);
			var bOp = node.Expression as BinaryOperatorExpression;
			if (bOp != null && bOp.Operator == BinaryOperatorType.NullCoalescing)
				return CreateForNullCoalesingExpression(context, node, bOp);
			return null;
		}

		CodeAction CreateForConditionalExpression(RefactoringContext ctx, ReturnStatement node, ConditionalExpression conditionalExpression)
		{
			return new CodeAction (
				ctx.TranslateString("Replace with 'if' statement"),
				script => {
					var ifStatement = new IfElseStatement(conditionalExpression.Condition.Clone(), new ReturnStatement(conditionalExpression.TrueExpression.Clone()));
					script.Replace(node, ifStatement); 
					script.InsertAfter(ifStatement, new ReturnStatement(conditionalExpression.FalseExpression.Clone()));
				},
				node
			);
		}

		CodeAction CreateForNullCoalesingExpression(RefactoringContext ctx, ReturnStatement node, BinaryOperatorExpression bOp)
		{
			return new CodeAction (
				ctx.TranslateString("Replace with 'if' statement"),
				script => {
					var ifStatement = new IfElseStatement(new BinaryOperatorExpression(bOp.Left.Clone(), BinaryOperatorType.InEquality, new NullReferenceExpression()), new ReturnStatement(bOp.Left.Clone()));
					script.Replace(node, ifStatement); 
					script.InsertAfter(ifStatement, new ReturnStatement(bOp.Right.Clone()));
				},
				node
			);
		}
	}
}

