//
// ConvertShiftToMultiplyAction.cs
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
	[ContextAction("Convert '<<'/'>>' to '*'/'/'",
	               Description = "Convert '<<'/'>>' to '*'/'/'")]
	public class ConvertShiftToMultiplyAction : SpecializedCodeAction <BinaryOperatorExpression>
	{
		protected override CodeAction GetAction(RefactoringContext context, BinaryOperatorExpression node)
		{
			if (!node.OperatorToken.Contains(context.Location))
				return null;
			if (node.Operator != BinaryOperatorType.ShiftLeft && node.Operator != BinaryOperatorType.ShiftRight || !(node.Right is PrimitiveExpression))
				return null;

			var value = context.Resolve(node.Right).ConstantValue;
			if (!(value is int))
				return null;

			return new CodeAction (
				node.Operator == BinaryOperatorType.ShiftLeft ? context.TranslateString("Replace with '*'") : context.TranslateString("Replace with '/'"),
				script => script.Replace(
					node, 
					new BinaryOperatorExpression(
						node.Left.Clone(), 
						node.Operator == BinaryOperatorType.ShiftLeft ? BinaryOperatorType.Multiply : BinaryOperatorType.Divide,
						new PrimitiveExpression(1 << (int)value)
					)
				),
				node.OperatorToken
			);
		}
	}
}