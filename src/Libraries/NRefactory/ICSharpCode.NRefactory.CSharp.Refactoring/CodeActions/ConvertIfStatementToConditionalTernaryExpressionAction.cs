//
// ConvertIfStatementToConditionalTernaryExpressionAction.cs
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
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Convert 'if' to '?:'",
	               Description = "Convert 'if' to '?:'")]
	public class ConvertIfStatementToConditionalTernaryExpressionAction : SpecializedCodeAction <IfElseStatement>
	{
		static readonly AstNode Pattern = 
			new IfElseStatement(
				new AnyNode("condition"),
				PatternHelper.EmbeddedStatement (new ExpressionStatement(new NamedNode ("assign1", new AssignmentExpression(new AnyNode("target"), AssignmentOperatorType.Any, new AnyNode("expr1"))))),
				PatternHelper.EmbeddedStatement (new ExpressionStatement(new NamedNode ("assign2", new AssignmentExpression(new Backreference("target"), AssignmentOperatorType.Any, new AnyNode("expr2")))))
			);

		public static bool GetMatch(IfElseStatement ifElseStatement, out Match match)
		{
			match = ConvertIfStatementToConditionalTernaryExpressionAction.Pattern.Match(ifElseStatement);
			if (!match.Success || ifElseStatement.Parent is IfElseStatement)
				return false;
			var firstAssign = match.Get<AssignmentExpression>("assign1").Single();
			var secondAssign = match.Get<AssignmentExpression>("assign2").Single();
			return firstAssign.Operator == secondAssign.Operator;
		}

		static CodeAction CreateAction (BaseRefactoringContext ctx, IfElseStatement ifElseStatement, Match match)
		{
			var target = match.Get<Expression>("target").Single();
			var condition = match.Get<Expression>("condition").Single();
			var trueExpr = match.Get<Expression>("expr1").Single();
			var falseExpr = match.Get<Expression>("expr2").Single();
			var firstAssign = match.Get<AssignmentExpression>("assign1").Single();

			return new CodeAction(
				ctx.TranslateString("Replace with '?:' expression"),
				script => {
					script.Replace(
						ifElseStatement, 
						new ExpressionStatement(
							new AssignmentExpression(
								target.Clone(),
								firstAssign.Operator,
								new ConditionalExpression(condition.Clone(), trueExpr.Clone(), falseExpr.Clone())
							)
						)
					); 
				},
				ifElseStatement
			);
		}

		protected override CodeAction GetAction(RefactoringContext context, IfElseStatement node)
		{
			if (!node.IfToken.Contains(context.Location))
				return null;
			Match match;
			if (!GetMatch(node, out match))
				return null;
			return CreateAction(context, node, match);
		}
	}
}