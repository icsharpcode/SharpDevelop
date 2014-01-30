// 
// ConvertIfToNullCoalescingAction.cs
//  
// Author:
//       Luís Reis <luiscubal@gmail.com>
// 
// Copyright (c) 2013 Luís Reis
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
	[ContextAction ("Convert 'if' to '??' expression",
	                Category = IssueCategories.Opportunities,
	                Description = "Convert 'if' statement to '??' expression.")]
	public class ConvertIfStatementToNullCoalescingExpressionAction : SpecializedCodeAction <IfElseStatement>
	{
		const string expressionGroupName = "expression";
		const string comparedNodeGroupName = "comparedNode";
		const string valueOnNullGroupName = "valueOnNull";

		static readonly Expression ActionPattern = 
			PatternHelper.OptionalParentheses(
				new NamedNode(
					expressionGroupName, 
					new Choice {
						PatternHelper.CommutativeOperatorWithOptionalParentheses(new AnyNode(comparedNodeGroupName), BinaryOperatorType.Equality, new NullReferenceExpression()),
						PatternHelper.CommutativeOperatorWithOptionalParentheses(new AnyNode(comparedNodeGroupName), BinaryOperatorType.InEquality, new NullReferenceExpression())
					}
				)
			);

		internal static Expression CheckNode(IfElseStatement node, out Expression rightSide)
		{
			rightSide = null;
			var match = ActionPattern.Match(node.Condition);
			if (!match.Success)
				return null;
			var conditionExpression = match.Get<BinaryOperatorExpression>(expressionGroupName).Single();
			bool isEqualityComparison = conditionExpression.Operator == BinaryOperatorType.Equality;
			Expression comparedNode = match.Get<Expression>(comparedNodeGroupName).Single();
			Statement contentStatement;
			if (isEqualityComparison) {
				contentStatement = node.TrueStatement;
				if (!IsEmpty(node.FalseStatement))
					return null;
			}
			else {
				contentStatement = node.FalseStatement;
				if (!IsEmpty(node.TrueStatement))
					return null;
			}
			contentStatement = GetSimpleStatement(contentStatement);
			if (contentStatement == null)
				return null;
			var leftExpressionPattern = PatternHelper.OptionalParentheses(comparedNode);
			var expressionPattern = new AssignmentExpression(leftExpressionPattern, AssignmentOperatorType.Assign, new AnyNode(valueOnNullGroupName));
			var statementPattern = new ExpressionStatement(PatternHelper.OptionalParentheses(expressionPattern));
			var statementMatch = statementPattern.Match(contentStatement);
			if (!statementMatch.Success)
				return null;
			rightSide = statementMatch.Get<Expression>(valueOnNullGroupName).Single();
			return comparedNode;
		}

		protected override CodeAction GetAction (RefactoringContext context, IfElseStatement node)
		{
			if (!node.IfToken.Contains(context.Location))
				return null;
			Expression rightSide;
			var comparedNode = CheckNode(node, out rightSide);
			if (comparedNode == null)
				return null;

			return new CodeAction(context.TranslateString("Replace with '??'"),
			                      script => {

				var previousNode = node.GetPrevSibling(sibling => sibling is Statement);

				var previousDeclaration = previousNode as VariableDeclarationStatement;
				if (previousDeclaration != null && previousDeclaration.Variables.Count() == 1) {
					var variable = previousDeclaration.Variables.First();

					var comparedNodeIdentifierExpression = comparedNode as IdentifierExpression;
					if (comparedNodeIdentifierExpression != null &&
					    comparedNodeIdentifierExpression.Identifier == variable.Name) {

						script.Replace(variable.Initializer, new BinaryOperatorExpression(variable.Initializer.Clone(),
						                                                                  BinaryOperatorType.NullCoalescing,
						                                                                  rightSide.Clone()));
						script.Remove(node);

						return;
					}
				}

				var previousExpressionStatement = previousNode as ExpressionStatement;
				if (previousExpressionStatement != null)
				{
					var previousAssignment = previousExpressionStatement.Expression as AssignmentExpression;
					if (previousAssignment != null &&
					    comparedNode.IsMatch(previousAssignment.Left)) {

						var newExpression = new BinaryOperatorExpression(previousAssignment.Right.Clone(),
						                                                 BinaryOperatorType.NullCoalescing,
						                                                 rightSide.Clone());

						script.Replace(previousAssignment.Right, newExpression);
						script.Remove(node);
						return;
					}
				}

				var coalescedExpression = new BinaryOperatorExpression(comparedNode.Clone(),
				                                                       BinaryOperatorType.NullCoalescing,
				                                                       rightSide.Clone());

				var newAssignment = new ExpressionStatement(new AssignmentExpression(comparedNode.Clone(), coalescedExpression));
				script.Replace(node, newAssignment);
			}, node);
		}

		static Statement GetSimpleStatement (Statement statement)
		{
			BlockStatement blockStatement;
			while ((blockStatement = statement as BlockStatement) != null) {
				var statements = blockStatement.Descendants.OfType<Statement>()
					.Where(descendant => !IsEmpty(descendant)).ToList();

				if (statements.Count() != 1) {
					return null;
				}

				statement = statements.First();
			}
			return statement;
		}

		static bool IsEmpty (Statement statement)
		{
			return statement.IsNull || 
				!statement.DescendantsAndSelf.OfType<Statement>().Any(descendant => !(descendant is EmptyStatement || descendant is BlockStatement));
		}
	}
}
