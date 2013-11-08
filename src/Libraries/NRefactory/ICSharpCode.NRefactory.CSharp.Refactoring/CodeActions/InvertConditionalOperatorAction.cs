//
// InvertConditionalOperatorAction.cs
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Invert conditional operator", Description = "Inverts an '?:' expression.")]
	public class InvertConditionalOperatorAction : SpecializedCodeAction<ConditionalExpression>
	{
		protected override CodeAction GetAction(RefactoringContext context, ConditionalExpression conditionalExpr)
		{
			if (context.Location != conditionalExpr.Condition.StartLocation && context.Location < conditionalExpr.Condition.EndLocation ||
			    context.Location != conditionalExpr.TrueExpression.StartLocation && conditionalExpr.TrueExpression.Contains(context.Location) ||
			    context.Location != conditionalExpr.FalseExpression.StartLocation && conditionalExpr.FalseExpression.Contains(context.Location))
				return null;

			var node = conditionalExpr.GetNodeAt(context.Location);
			if (node == null)
				return null;

			return new CodeAction (context.TranslateString("Invert '?:'"), script => {
				script.Replace(conditionalExpr.Condition, CSharpUtil.InvertCondition(conditionalExpr.Condition.Clone()));
				script.Replace(conditionalExpr.TrueExpression, conditionalExpr.FalseExpression.Clone());
				script.Replace(conditionalExpr.FalseExpression, conditionalExpr.TrueExpression.Clone());
				script.FormatText(conditionalExpr);
			}, node);
		}
	}
}

