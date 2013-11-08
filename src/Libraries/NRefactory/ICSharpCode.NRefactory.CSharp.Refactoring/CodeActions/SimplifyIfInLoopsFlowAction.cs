//
// SimplifyIfInLoopsFlowAction.cs
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Simplify if flow in loops", Description = "Inverts if and reduces branching ")]
	public class SimplifyIfInLoopsFlowAction : CodeActionProvider
	{
		readonly InsertParenthesesVisitor _insertParenthesesVisitor = new InsertParenthesesVisitor();

		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var ifStatement = GetIfElseStatement(context);
			if (ifStatement == null)
				yield break;
			yield return new CodeAction(context.TranslateString("Simplify if in loops"), script => {
				GenerateNewScript(script, ifStatement);
			}, ifStatement);
		}
		
		void GenerateNewScript(Script script, IfElseStatement ifStatement)
		{
			var mergedIfStatement = new IfElseStatement {
				Condition = CSharpUtil.InvertCondition(ifStatement.Condition),
				TrueStatement = new ContinueStatement()
			};
			mergedIfStatement.Condition.AcceptVisitor(_insertParenthesesVisitor);
			
			script.Replace(ifStatement, mergedIfStatement);
			
			SimplifyIfFlowAction.InsertBody(script, ifStatement);
		}
		
		static IfElseStatement GetIfElseStatement(RefactoringContext context)
		{
			var result = context.GetNode<IfElseStatement>();
			if (result == null)
				return null;
			if (!(result.IfToken.Contains(context.Location)
				&& !result.TrueStatement.IsNull
				&& result.FalseStatement.IsNull))
				return null;
			if (!(result.Parent is BlockStatement))
				return null;
			var condition = (result.Parent.Parent is WhileStatement) 
				|| (result.Parent.Parent is ForeachStatement) 
				|| (result.Parent.Parent is ForStatement);
			if (!condition)
				return null;
			
			var nextSibling = result.GetNextSibling(n => n is Statement);
			if (nextSibling == null)
				return result;
			nextSibling = nextSibling.GetNextSibling (n => n is Statement);
			if (nextSibling != null)
				return null;
			return result;
		}
	}
}