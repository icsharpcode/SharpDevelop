// 
// InlineLocalVariableAction.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
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

using System.Threading;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Inline local variable", Description = "Inlines a local variable.")]
	public class InlineLocalVariableAction : CodeActionProvider
	{
		static FindReferences refFinder = new FindReferences();
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			if (context.IsSomethingSelected) {
				yield break;
			}
			var node = context.GetNode<VariableDeclarationStatement>();
			if (node == null || node.Variables.Count != 1) {
				yield break;
			}
			var initializer = node.Variables.First();
			if (!initializer.NameToken.Contains(context.Location) || initializer.Initializer.IsNull) {
				yield break;
			}
			var resolveResult = context.Resolve(initializer) as LocalResolveResult;
			if (resolveResult == null || resolveResult.IsError) {
				yield break;
			}
			var unit = context.RootNode as SyntaxTree;
			if (unit == null) {
				yield break;
			}
			yield return new CodeAction(context.TranslateString("Inline local variable"), script => {
				refFinder.FindLocalReferences(resolveResult.Variable, context.UnresolvedFile, unit, context.Compilation, (n, r) => script.Replace(n, AddParensIfRequired (n, initializer.Initializer.Clone())), default(CancellationToken));
				script.Remove(node);
			}, initializer);
		}

		public static bool RequiresParens(AstNode replaceNode, AstNode replaceWithNode)
		{
			if (!(replaceWithNode is BinaryOperatorExpression) &&
			    !(replaceWithNode is AssignmentExpression) &&
			    !(replaceWithNode is AsExpression) &&
			    !(replaceWithNode is IsExpression) &&
			    !(replaceWithNode is CastExpression) &&
			    !(replaceWithNode is LambdaExpression) &&
				!(replaceWithNode is ConditionalExpression)) {
				return false;
			}

			var cond = replaceNode.Parent as ConditionalExpression;
			if (cond != null && cond.Condition == replaceNode)
				return true;

			var indexer = replaceNode.Parent as IndexerExpression;
			if (indexer != null && indexer.Target == replaceNode)
				return true;

			return replaceNode.Parent is BinaryOperatorExpression || 
				replaceNode.Parent is UnaryOperatorExpression || 
				replaceNode.Parent is AssignmentExpression || 
				replaceNode.Parent is MemberReferenceExpression ||
				replaceNode.Parent is AsExpression || 
				replaceNode.Parent is IsExpression || 
				replaceNode.Parent is CastExpression ||
				replaceNode.Parent is LambdaExpression ||
				replaceNode.Parent is PointerReferenceExpression;
		}

		static Expression AddParensIfRequired(AstNode replaceNode, Expression expression)
		{
			if (RequiresParens(replaceNode, expression))
				return new ParenthesizedExpression(expression);
			return expression;
		}
	}
}
