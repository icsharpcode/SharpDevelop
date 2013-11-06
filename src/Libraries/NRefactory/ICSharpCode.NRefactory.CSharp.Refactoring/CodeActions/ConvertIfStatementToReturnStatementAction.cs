//
// ConvertIfStatementToReturnStatementAction.cs
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
	[ContextAction("Convert 'if' to 'return'",
	               Description = "Convert 'if' to 'return'")]
	public class ConvertIfStatementToReturnStatementAction : SpecializedCodeAction <IfElseStatement>
	{
		static readonly AstNode ifElsePattern = 
			new IfElseStatement(
				new AnyNode("condition"),
				PatternHelper.EmbeddedStatement (new ReturnStatement(new AnyNode("expr1"))),
				PatternHelper.EmbeddedStatement (new ReturnStatement(new AnyNode("expr2")))
			);

		static readonly AstNode ifPattern = 
			new IfElseStatement(
				new AnyNode("condition"),
				PatternHelper.EmbeddedStatement (new ReturnStatement(new AnyNode("expr1")))
			);

		static readonly AstNode returnPattern = 
			new ReturnStatement(new AnyNode("expr2"));

		public static bool GetMatch(IfElseStatement ifElseStatement, out Expression condition, out Expression expr1, out Expression expr2, out AstNode returnStatement)
		{
			var match = ifElsePattern.Match(ifElseStatement);
			returnStatement = null;
			if (match.Success) {
				condition = match.Get<Expression>("condition").Single();
				expr1 = match.Get<Expression>("expr1").Single();
				expr2 = match.Get<Expression>("expr2").Single();
				return true;
			}

			match = ifPattern.Match(ifElseStatement);
			if (match.Success) {
				returnStatement = ifElseStatement.GetNextSibling(s => s.Role == BlockStatement.StatementRole);
				var match2 = returnPattern.Match(returnStatement);

				if (match2.Success) {
					condition = match.Get<Expression>("condition").Single();
					expr1 = match.Get<Expression>("expr1").Single();
					expr2 = match2.Get<Expression>("expr2").Single();
					return true;
				}
			}

			condition = expr1 = expr2 = null;
			return false;
		}

		static readonly AstNode truePattern = PatternHelper.OptionalParentheses(new PrimitiveExpression (true));
		static readonly AstNode falsePattern = PatternHelper.OptionalParentheses(new PrimitiveExpression (false));

		static Expression CreateCondition(Expression c, Expression e1, Expression e2)
		{
			if (truePattern.IsMatch(e1) && falsePattern.IsMatch(e2))
				return c.Clone();
			return new ConditionalExpression(c.Clone(), e1.Clone(), e2.Clone());
		}

		protected override CodeAction GetAction(RefactoringContext context, IfElseStatement ifElseStatement)
		{
			if (!ifElseStatement.IfToken.Contains(context.Location))
				return null;

			Expression c, e1, e2;
			AstNode rs;
			if (!ConvertIfStatementToReturnStatementAction.GetMatch(ifElseStatement, out c, out e1, out e2, out rs))
				return null;
			return new CodeAction (
				context.TranslateString("Replace with 'return'"),
				script => {
					script.Replace(ifElseStatement, new ReturnStatement(
					CreateCondition(c, e1, e2)
				)); 
					if (rs != null)
						script.Remove(rs); 
				},
				ifElseStatement
			);
		}
	}
}

