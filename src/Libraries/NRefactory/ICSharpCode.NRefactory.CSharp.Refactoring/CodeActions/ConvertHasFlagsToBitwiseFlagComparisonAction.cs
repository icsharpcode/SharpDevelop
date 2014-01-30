//
// ConvertHasFlagsToBitwiseFlagComparisonAction.cs
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
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction ("Replace 'Enum.HasFlag' call with bitwise flag comparison", Description = "Replace 'Enum.HasFlag' call with bitwise flag comparison")]
	public class ConvertHasFlagsToBitwiseFlagComparisonAction : SpecializedCodeAction<InvocationExpression>
	{
		protected override CodeAction GetAction(RefactoringContext context, InvocationExpression node)
		{
			var mRef = node.Target as MemberReferenceExpression;
			if (mRef == null)
				return null;
			var rr = context.Resolve(node) as CSharpInvocationResolveResult;
			if (rr == null || rr.IsError)
				return null;
			if (rr.Member.Name != "HasFlag" || rr.Member.DeclaringType.GetDefinition().KnownTypeCode != ICSharpCode.NRefactory.TypeSystem.KnownTypeCode.Enum)
				return null;
			var arg = node.Arguments.First ().Clone ();
			if (!arg.DescendantsAndSelf.All(x => !(x is BinaryOperatorExpression) || ((BinaryOperatorExpression)x).Operator == BinaryOperatorType.BitwiseOr))
				return null;
			arg = ConvertBitwiseFlagComparisonToHasFlagsAction.MakeFlatExpression(arg, BinaryOperatorType.BitwiseAnd);
			if (arg is BinaryOperatorExpression)
				arg = new ParenthesizedExpression(arg);
			return new CodeAction(
				context.TranslateString("Replace with bitwise flag comparison"), 
				script => {
					var uOp = node.Parent as UnaryOperatorExpression;
					if (uOp != null && uOp.Operator == UnaryOperatorType.Not) {
						script.Replace(uOp, 
							new BinaryOperatorExpression(
								new ParenthesizedExpression(new BinaryOperatorExpression(mRef.Target.Clone(), BinaryOperatorType.BitwiseAnd, arg)), 
								BinaryOperatorType.Equality, 
								new PrimitiveExpression(0)
							)
						);
					} else {
						script.Replace(node, 
							new BinaryOperatorExpression(
								new ParenthesizedExpression(new BinaryOperatorExpression(mRef.Target.Clone(), BinaryOperatorType.BitwiseAnd, arg)), 
								BinaryOperatorType.InEquality, 
								new PrimitiveExpression(0)
							)
						);
					}
				}, 
				node
			);
		}
	}
}

