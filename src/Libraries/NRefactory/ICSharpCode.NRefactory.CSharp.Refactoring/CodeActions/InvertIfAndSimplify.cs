//
// InvertIfAndSimplify.cs
//
// Author:
//      Ciprian Khlud <ciprian.mustiata@yahoo.com>
//
// Copyright (c) 2013 Ciprian Khlud <ciprian.mustiata@yahoo.com>
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

using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Invert If and Simplify", Description = "Inverts if and simplify branching")]
	public class InvertIfAndSimplify : CodeActionProvider
	{
		readonly InsertParenthesesVisitor _insertParenthesesVisitor = new InsertParenthesesVisitor();

		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			// if (condition) {CodeBlock();}else { return|break|continue;} 
			// will be reduced to:
			//if (!condition) return|break|continue;
			//CodeBlock();

			var ifStatement = GetIfElseStatement(context);
			if (ifStatement == null)
				yield break;
			yield return new CodeAction(context.TranslateString("Simplify if in loops"), script => GenerateNewScript(
				script, ifStatement), ifStatement);
		}

		static Statement GenerateNewTrueStatement(Statement falseStatement)
		{
			var blockStatement = falseStatement as BlockStatement;
			if (blockStatement != null) {
				if (blockStatement.Children.Count(n => n.Role != Roles.NewLine && n.Role != Roles.LBrace && n.Role != Roles.RBrace) == 1)
					return blockStatement.Statements.First().Clone ();
			}
			return falseStatement.Clone();
		}

		void GenerateNewScript(Script script, IfElseStatement ifStatement)
		{
			var mergedIfStatement = new IfElseStatement
			{
				Condition = CSharpUtil.InvertCondition(ifStatement.Condition)
			};
			var falseStatement = ifStatement.FalseStatement;
			mergedIfStatement.TrueStatement = GenerateNewTrueStatement(falseStatement);
			mergedIfStatement.Condition.AcceptVisitor(_insertParenthesesVisitor);

			script.Replace(ifStatement, mergedIfStatement);

			SimplifyIfFlowAction.InsertBody(script, ifStatement);
		}

		static IfElseStatement GetIfElseStatement(RefactoringContext context)
		{
			var result = context.GetNode<IfElseStatement>();
			if (result == null || !result.IfToken.Contains(context.Location))
				return null;
			var falseStatement = result.FalseStatement;
			var isQuitingStatement = falseStatement;
			var blockStatement = falseStatement as BlockStatement;
			if (blockStatement != null)
				isQuitingStatement = blockStatement.Statements.FirstOrDefault();
			if (isQuitingStatement is ReturnStatement || isQuitingStatement is ContinueStatement || isQuitingStatement is BreakStatement)
				return result;
			return null;
		}
	}
}