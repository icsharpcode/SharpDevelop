// 
// ForControlVariableIsNeverModifiedIssue.cs
// 
// Author:
//      Mansheng Yang <lightyang0@gmail.com>
// 
// Copyright (c) 2012 Mansheng Yang <lightyang0@gmail.com>
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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("'for' loop control variable is never modified",
					   Description = "'for' loop control variable is never modified.",
					   Category = IssueCategories.CodeQualityIssues,
					   Severity = Severity.Warning,
                       AnalysisDisableKeyword = "ForControlVariableIsNeverModified")]
    public class ForControlVariableIsNeverModifiedIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ForControlVariableIsNeverModifiedIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			static VariableInitializer GetControlVariable(VariableDeclarationStatement variableDecl, 
														  BinaryOperatorExpression condition)
			{
				var controlVariables = variableDecl.Variables.Where (
					v =>
					{
						var identifier = new IdentifierExpression (v.Name);
						return condition.Left.Match (identifier).Success ||
							condition.Right.Match (identifier).Success;
					}).ToList ();
				return controlVariables.Count == 1 ? controlVariables [0] : null;
			}

			static VariableInitializer GetControlVariable(VariableDeclarationStatement variableDecl,
														  UnaryOperatorExpression condition)
			{
				var controlVariables = variableDecl.Variables.Where (
					v =>
					{
						var identifier = new IdentifierExpression (v.Name);
						return condition.Expression.Match (identifier).Success;
					}).ToList ();
				return controlVariables.Count == 1 ? controlVariables [0] : null;
			}

			public override void VisitForStatement (ForStatement forStatement)
			{
				base.VisitForStatement (forStatement);

				if (forStatement.Initializers.Count != 1)
					return;
				var variableDecl = forStatement.Initializers.First () as VariableDeclarationStatement;
				if (variableDecl == null)
					return;

				VariableInitializer controlVariable = null;
				if (forStatement.Condition is BinaryOperatorExpression) {
					controlVariable = GetControlVariable (variableDecl, (BinaryOperatorExpression)forStatement.Condition);
				} else if (forStatement.Condition is UnaryOperatorExpression) {
					controlVariable = GetControlVariable (variableDecl, (UnaryOperatorExpression)forStatement.Condition);
				} else if (forStatement.Condition is IdentifierExpression) {
					controlVariable = variableDecl.Variables.FirstOrDefault (
						v => v.Name == ((IdentifierExpression)forStatement.Condition).Identifier);
				}

				if (controlVariable == null)
					return;

				var localResolveResult = ctx.Resolve (controlVariable) as LocalResolveResult;
				if (localResolveResult == null)
					return;

				var results = ctx.FindReferences (forStatement, localResolveResult.Variable);
				var modified = false;
				foreach (var result in results) {
					if (modified)
						break;
					var node = result.Node;
					var unary = node.Parent as UnaryOperatorExpression;
					if (unary != null && unary.Expression == node) {
						modified = unary.Operator == UnaryOperatorType.Decrement ||
							unary.Operator == UnaryOperatorType.PostDecrement ||
							unary.Operator == UnaryOperatorType.Increment ||
							unary.Operator == UnaryOperatorType.PostIncrement;
						continue;
					}

					var assignment = node.Parent as AssignmentExpression;
					modified = assignment != null && assignment.Left == node;
				}

				if (!modified)
					AddIssue (new CodeIssue(controlVariable.NameToken,
						ctx.TranslateString ("'for' loop control variable is never modified")));

			}
		}
	}
}
