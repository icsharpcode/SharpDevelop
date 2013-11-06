//
// ConvertMultiplyToShiftAction.cs
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Convert '*'/'/' to '<<'/'>>'",
	               Description = "Convert '*'/'/' to '<<'/'>>'")]
	public class ConvertMultiplyToShiftAction : SpecializedCodeAction<BinaryOperatorExpression>
	{
		protected override CodeAction GetAction(RefactoringContext context, BinaryOperatorExpression node)
		{
			if (!node.OperatorToken.Contains(context.Location))
				return null;
			if (node.Operator != BinaryOperatorType.Multiply && node.Operator != BinaryOperatorType.Divide || !(node.Right is PrimitiveExpression))
				return null;

			var valueObj = context.Resolve(node.Right).ConstantValue;
			if (!(valueObj is int))
				return null;
			var value = (int)valueObj;

			var log2 = (int)Math.Log(value, 2);
			if (value != 1 << log2)
				return null;

			return new CodeAction (
				node.Operator == BinaryOperatorType.Multiply ? context.TranslateString("Replace with '<<'") : context.TranslateString("Replace with '>>'"),
				script => script.Replace(
					node, 
					new BinaryOperatorExpression(
						node.Left.Clone(), 
						node.Operator == BinaryOperatorType.Multiply ? BinaryOperatorType.ShiftLeft : BinaryOperatorType.ShiftRight,
						new PrimitiveExpression(log2)
					)
				),
				node.OperatorToken
			);
		}
	}
}