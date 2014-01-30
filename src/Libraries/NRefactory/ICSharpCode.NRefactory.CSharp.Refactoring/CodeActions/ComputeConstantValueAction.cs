// 
// ComputeConstantValueAction.cs
//  
// Author:
//       Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
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
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Compute constant value", Description = "Computes the value of the current expression and replaces it.")]
	public class ComputeConstantValueAction : CodeActionProvider
	{
		public override System.Collections.Generic.IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var expression = context.GetNode(i => i is BinaryOperatorExpression || i is UnaryOperatorExpression);
			if (expression == null)
				yield break;
			var node = context.GetNode();
			if (node == null || !(node is PrimitiveExpression) && node.StartLocation != context.Location)
				yield break;

			var rr = context.Resolve(expression);
			if (rr.ConstantValue == null)
				yield break;
			yield return new CodeAction(
				context.TranslateString("Compute constant value"),
				script => script.Replace(expression, new PrimitiveExpression(rr.ConstantValue)), 
				node
			);
		}
	}
}