//
// ConvertEqualityOperatorToEqualsAction.cs
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
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Convert do...while to while. For instance, { do x++; while (Foo(x)); } becomes { while(Foo(x)) x++; }.
	/// Note that this action will often change the semantics of the code.
	/// </summary>
	[ContextAction("Convert '==' to 'Equals'", Description = "Converts '==' to call to 'object.Equals'")]
	public class ConvertEqualityOperatorToEqualsAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var node = context.GetNode<BinaryOperatorExpression>();
			if (node == null || 
			    (node.Operator != BinaryOperatorType.Equality && node.Operator != BinaryOperatorType.InEquality) ||
			    !node.OperatorToken.Contains(context.Location))
				yield break;

			yield return new CodeAction(
				context.TranslateString("Use 'Equals'"),
				script => {
					Expression expr = new InvocationExpression(GenerateTarget(context, node), node.Left.Clone(), node.Right.Clone());
					if (node.Operator == BinaryOperatorType.InEquality)
						expr = new UnaryOperatorExpression(UnaryOperatorType.Not, expr);
					script.Replace(node, expr);
				}, 
				node.OperatorToken
			);
		}

		readonly IList<IType> emptyTypes = new IType[0];

		bool HasDifferentEqualsMethod(IEnumerable<IMethod> methods)
		{
			foreach (var method in methods) {
				if (method.Parameters.Count == 2 && method.FullName != "System.Object.Equals") {
					return true;
				}
			}
			return false;
		}

		Expression GenerateTarget(RefactoringContext context, BinaryOperatorExpression bOp)
		{
			var rr = context.Resolver.GetResolverStateBefore(bOp).LookupSimpleNameOrTypeName("Equals", emptyTypes, NameLookupMode.Expression) as MethodGroupResolveResult;
			if (rr == null || rr.IsError || HasDifferentEqualsMethod (rr.Methods)) {
				return new PrimitiveType ("object").Member("Equals");
			}
			return new IdentifierExpression("Equals");
		}
	}
}

