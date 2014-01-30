//
// ReplaceWithOperatorAssignmentAction.cs
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
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Replace assignment with operator assignment", Description = "Replace assignment with operator assignment")]
	public class ReplaceWithOperatorAssignmentAction : SpecializedCodeAction<AssignmentExpression>
	{
		internal static AssignmentExpression CreateAssignment(AssignmentExpression node)
		{
			var bop = node.Right as BinaryOperatorExpression;
			if (bop == null)
				return null;
			var outerLeft = GetOuterLeft(bop);
			if (!outerLeft.IsMatch(node.Left))
				return null;
			var op = GetAssignmentOperator(bop.Operator);
			if (op == AssignmentOperatorType.Any)
				return null;
			return new AssignmentExpression(node.Left.Clone(), op, SplitIfAction.GetRightSide((BinaryOperatorExpression)outerLeft.Parent));
		}

		protected override CodeAction GetAction(RefactoringContext context, AssignmentExpression node)
		{
			if (!node.OperatorToken.Contains(context.Location))
				return null;

			var ae = CreateAssignment(node);
			if (ae == null)
				return null;
			return new CodeAction (
				string.Format(context.TranslateString("Replace with '{0}='"), ((BinaryOperatorExpression)node.Right).OperatorToken),
				s => s.Replace(node, ae),
				node.OperatorToken
			);
		}

		static AssignmentOperatorType GetAssignmentOperator(BinaryOperatorType op)
		{
			switch (op) {
				case BinaryOperatorType.BitwiseAnd:
					return AssignmentOperatorType.BitwiseAnd;
				case BinaryOperatorType.BitwiseOr:
					return AssignmentOperatorType.BitwiseOr;
				case BinaryOperatorType.ExclusiveOr:
					return AssignmentOperatorType.ExclusiveOr;
				case BinaryOperatorType.Add:
					return AssignmentOperatorType.Add;
				case BinaryOperatorType.Subtract:
					return AssignmentOperatorType.Subtract;
				case BinaryOperatorType.Multiply:
					return AssignmentOperatorType.Multiply;
				case BinaryOperatorType.Divide:
					return AssignmentOperatorType.Divide;
				case BinaryOperatorType.Modulus:
					return AssignmentOperatorType.Modulus;
				case BinaryOperatorType.ShiftLeft:
					return AssignmentOperatorType.ShiftLeft;
				case BinaryOperatorType.ShiftRight:
					return AssignmentOperatorType.ShiftRight;
				default:
					return AssignmentOperatorType.Any;
			}
		}

		static Expression GetOuterLeft (BinaryOperatorExpression bop)
		{
			var leftBop = bop.Left as BinaryOperatorExpression;
			if (leftBop != null && bop.Operator == leftBop.Operator)
				return GetOuterLeft(leftBop);
			return bop.Left;
		}
	}
}

