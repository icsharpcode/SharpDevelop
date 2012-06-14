//
// IterateViaForeachTests.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Iterate via foreach", Description = "Iterates an IEnumerable with a foreach loop.")]
	public class IterateViaForeachAction : ICodeActionProvider
	{
		#region ICodeActionProvider implementation

		public IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			CodeAction action;
			action = ActionFromVariableInitializer(context);
			if (action != null)
				yield return action;
			action = ActionFromExpressionStatement(context);
			if (action != null)
				yield return action;
		}

		CodeAction ActionFromVariableInitializer(RefactoringContext context)
		{
			var initializer = context.GetNode<VariableInitializer>();
			if (initializer == null)
				return null;
			var initializerRR = context.Resolve(initializer) as LocalResolveResult;
			if (!ImplementsInterface(initializerRR.Variable.Type, "System.Collections.IEnumerable"))
				return null;
			return new CodeAction(context.TranslateString("Iterate via foreach"), script => {
				var iterator = MakeForeach(new IdentifierExpression(initializer.Name), context);
				script.InsertBefore(context.GetNode<Statement>().NextSibling, iterator);
				// Work around incorrect formatting when inserting relatively to
				// a node on a different indentation level
				script.FormatText(context.GetNode<BlockStatement>());
			});
		}

		CodeAction ActionFromExpressionStatement(RefactoringContext context)
		{
			var expressionStatement = context.GetNode<ExpressionStatement>();
			if (expressionStatement == null)
				return null;
			var expression = expressionStatement.Expression;
			if (expression is AssignmentExpression)
				expression = ((AssignmentExpression)expression).Left;
			var expressionRR = context.Resolve(expression) as ResolveResult;
			if (!ImplementsInterface(expressionRR.Type, "System.Collections.IEnumerable"))
				return null;
			return new CodeAction(context.TranslateString("Iterate via foreach"), script => {
				var iterator = MakeForeach(expression.Clone(), context);
				if (expression == expressionStatement.Expression)
					script.Replace(expressionStatement, iterator);
				else
					script.InsertAfter(expressionStatement, iterator);
			});

		}

		static AstNode MakeForeach(Expression node, RefactoringContext context)
		{
			return new ForeachStatement() {
				VariableType = new SimpleType("var"),
				VariableName = "item",
				InExpression = node,
				EmbeddedStatement = new BlockStatement()
			};
		}

		bool ImplementsInterface(IType type, string typeName)
		{
			if (type.FullName == typeName)
				return true;
			foreach (var baseType in type.DirectBaseTypes) {
				if (ImplementsInterface(baseType, typeName))
					return true;
			}
			return false;
		}

		#endregion
	}
}

