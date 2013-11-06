// 
// AutoLinqSum.cs
//
// Author:
//       Luís Reis <luiscubal@gmail.com>
// 
// Copyright (c) 2013 Luís Reis <luiscubal@gmail.com>
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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Convers a loop to a Linq expression.
	/// </summary>
	[ContextAction("Convert loop to Linq expression", Description = "Converts a loop to a Linq expression")]
	public class AutoLinqSumAction : CodeActionProvider
	{
		// Disabled for nullables, since int? x = 3; x += null; has result x = null,
		// but LINQ Sum behaves differently : nulls are treated as zero
		static readonly IEnumerable<string> LinqSummableTypes = new string[] {
			"System.UInt16",
			"System.Int16",
			"System.UInt32",
			"System.Int32",
			"System.UInt64",
			"System.Int64",
			"System.Single",
			"System.Double",
			"System.Decimal"
		};

		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var loop = GetForeachStatement (context);
			if (loop == null) {
				yield break;
			}

			if (context.GetResolverStateBefore(loop)
			    .LookupSimpleNameOrTypeName("Enumerable", new List<IType>(), NameLookupMode.Type)
			    .Type.FullName != "System.Linq.Enumerable") {

				yield break;
			}

			var outputStatement = GetTransformedAssignmentExpression (context, loop);
			if (outputStatement == null) {
				yield break;
			}

			yield return new CodeAction(context.TranslateString("Convert foreach loop to LINQ expression"), script => {

				var prevSibling = loop.GetPrevSibling(node => node is Statement);

				Expression leftSide = outputStatement.Left;
				Expression rightSide = outputStatement.Right;

				Expression expressionToReplace = GetExpressionToReplace(prevSibling, leftSide);

				if (expressionToReplace != null) {
					Expression replacementExpression = rightSide.Clone();
					if (!IsZeroPrimitive(expressionToReplace)) {
						replacementExpression = new BinaryOperatorExpression(ParenthesizeIfNeeded(expressionToReplace).Clone(),
						                                                     BinaryOperatorType.Add,
						                                                     replacementExpression);
					}

					script.Replace(expressionToReplace, replacementExpression);
					script.Remove(loop);
				}
				else {
					script.Replace(loop, new ExpressionStatement(outputStatement));
				}

			}, loop);
		}

		bool IsZeroPrimitive(Expression expr)
		{
			//We want a very simple check -- no looking at constants, no constant folding, etc.
			//So 1+1 should return false, but (0) should return true

			var parenthesizedExpression = expr as ParenthesizedExpression;
			if (parenthesizedExpression != null) {
				return IsZeroPrimitive(parenthesizedExpression.Expression);
			}

			var zeroLiteralInteger = new PrimitiveExpression(0);
			var zeroLiteralFloat = new PrimitiveExpression(0.0f);
			var zeroLiteralDouble = new PrimitiveExpression(0.0);
			var zeroLiteralDecimal = new PrimitiveExpression(0.0m);

			return SameNode(zeroLiteralInteger, expr) ||
				SameNode(zeroLiteralFloat, expr) ||
				SameNode(zeroLiteralDouble, expr) ||
				SameNode(zeroLiteralDecimal, expr);
		}

		Expression GetExpressionToReplace(AstNode prevSibling, Expression requiredLeftSide)
		{
			if (prevSibling == null) {
				return null;
			}

			var declarationStatement = prevSibling as VariableDeclarationStatement;
			if (declarationStatement != null)
			{
				if (declarationStatement.Variables.Count != 1) {
					return null;
				}

				var identifierExpr = requiredLeftSide as IdentifierExpression;
				if (identifierExpr == null) {
					return null;
				}

				var variableDecl = declarationStatement.Variables.First();

				if (!SameNode(identifierExpr.IdentifierToken, variableDecl.NameToken)) {
					return null;
				}

				return variableDecl.Initializer;
			}

			var exprStatement = prevSibling as ExpressionStatement;
			if (exprStatement != null) {
				var assignment = exprStatement.Expression as AssignmentExpression;
				if (assignment != null) {
					if (assignment.Operator != AssignmentOperatorType.Assign &&
						assignment.Operator != AssignmentOperatorType.Add) {

						return null;
					}

					if (!SameNode(requiredLeftSide, assignment.Left)) {
						return null;
					}

					return assignment.Right;
				}
			}

			return null;
		}

		bool IsUnaryModifierExpression(UnaryOperatorExpression expr)
		{
			return expr.Operator == UnaryOperatorType.Increment || expr.Operator == UnaryOperatorType.PostIncrement || expr.Operator == UnaryOperatorType.Decrement || expr.Operator == UnaryOperatorType.PostDecrement;
		}

		AssignmentExpression GetTransformedAssignmentExpression (RefactoringContext context, ForeachStatement foreachStatement)
		{
			var enumerableToIterate = foreachStatement.InExpression.Clone();

			Statement statement = foreachStatement.EmbeddedStatement;

			Expression leftExpression, rightExpression;
			if (!ExtractExpression(statement, out leftExpression, out rightExpression)) {
				return null;
			}
			if (leftExpression == null || rightExpression == null) {
				return null;
			}

			var type = context.Resolve(leftExpression).Type;
			if (!IsLinqSummableType(type)) {
				return null;
			}

			if (rightExpression.DescendantsAndSelf.OfType<AssignmentExpression>().Any()) {
				// Reject loops such as
				// int k = 0;
				// foreach (var x in y) { k += (z = 2); }

				return null;
			}

			if (rightExpression.DescendantsAndSelf.OfType<UnaryOperatorExpression>().Any(IsUnaryModifierExpression)) {
				// int k = 0;
				// foreach (var x in y) { k += (z++); }

				return null;
			}

			var zeroLiteral = new PrimitiveExpression(0);

			Expression baseExpression = enumerableToIterate;
			for (;;) {
				ConditionalExpression condition = rightExpression as ConditionalExpression;
				if (condition == null) {
					break;
				}

				if (SameNode(zeroLiteral, condition.TrueExpression)) {
					baseExpression = new InvocationExpression(new MemberReferenceExpression(baseExpression.Clone(), "Where"),
					                                          BuildLambda(foreachStatement.VariableName, CSharpUtil.InvertCondition(condition.Condition)));
					rightExpression = condition.FalseExpression.Clone();

					continue;
				}

				if (SameNode(zeroLiteral, condition.FalseExpression)) {
					baseExpression = new InvocationExpression(new MemberReferenceExpression(baseExpression.Clone(), "Where"),
					                                          BuildLambda(foreachStatement.VariableName, condition.Condition.Clone()));
					rightExpression = condition.TrueExpression.Clone();

					continue;
				}

				break;
			}

			var primitiveOne = new PrimitiveExpression(1);
			bool isPrimitiveOne = SameNode(primitiveOne, rightExpression);

			var arguments = new List<Expression>();

			string method = isPrimitiveOne ? "Count" : "Sum";

			if (!isPrimitiveOne && !IsIdentifier(rightExpression, foreachStatement.VariableName)) {
				var lambda = BuildLambda(foreachStatement.VariableName, rightExpression);

				arguments.Add(lambda);
			}

			var rightSide = new InvocationExpression(new MemberReferenceExpression(baseExpression, method), arguments);

			return new AssignmentExpression(leftExpression.Clone(), AssignmentOperatorType.Add, rightSide);
		}

		static LambdaExpression BuildLambda(string variableName, Expression expression)
		{
			var lambda = new LambdaExpression();
			lambda.Parameters.Add(new ParameterDeclaration() {
				Name = variableName
			});
			lambda.Body = expression.Clone();
			return lambda;
		}

		bool IsIdentifier(Expression expr, string variableName)
		{
			var identifierExpr = expr as IdentifierExpression;
			if (identifierExpr != null) {
				return identifierExpr.Identifier == variableName;
			}

			var parenthesizedExpr = expr as ParenthesizedExpression;
			if (parenthesizedExpr != null) {
				return IsIdentifier(parenthesizedExpr.Expression, variableName);
			}

			return false;
		}

		bool IsLinqSummableType(IType type) {
			return LinqSummableTypes.Contains(type.FullName);
		}

		bool ExtractExpression (Statement statement, out Expression leftSide, out Expression rightSide) {
			ExpressionStatement expression = statement as ExpressionStatement;
			if (expression != null) {
				AssignmentExpression assignment = expression.Expression as AssignmentExpression;
				if (assignment != null) {
					if (assignment.Operator == AssignmentOperatorType.Add) {
						leftSide = assignment.Left;
						rightSide = assignment.Right;
						return true;
					}
					if (assignment.Operator == AssignmentOperatorType.Subtract) {
						leftSide = assignment.Left;
						rightSide = new UnaryOperatorExpression(UnaryOperatorType.Minus, assignment.Right.Clone());
						return true;
					}

					leftSide = null;
					rightSide = null;
					return false;
				}

				UnaryOperatorExpression unary = expression.Expression as UnaryOperatorExpression;
				if (unary != null) {
					leftSide = unary.Expression;
					if (unary.Operator == UnaryOperatorType.Increment || unary.Operator == UnaryOperatorType.PostIncrement) {
						rightSide = new PrimitiveExpression(1);
						return true;
					} else if (unary.Operator == UnaryOperatorType.Decrement || unary.Operator == UnaryOperatorType.PostDecrement) {
						rightSide = new PrimitiveExpression(-1);
						return true;
					} else {
						leftSide = null;
						rightSide = null;
						return false;
					}
				}
			}

			if (statement is EmptyStatement || statement.IsNull) {
				leftSide = null;
				rightSide = null;
				return true;
			}

			BlockStatement block = statement as BlockStatement;
			if (block != null) {
				leftSide = null;
				rightSide = null;

				foreach (Statement child in block.Statements) {
					Expression newLeft, newRight;
					if (!ExtractExpression(child, out newLeft, out newRight)) {
						leftSide = null;
						rightSide = null;
						return false;
					}

					if (newLeft == null) {
						continue;
					}

					if (leftSide == null) {
						leftSide = newLeft;
						rightSide = newRight;
					} else if (SameNode(leftSide, newLeft)) {
						rightSide = new BinaryOperatorExpression(ParenthesizeIfNeeded(rightSide).Clone(),
						                                         BinaryOperatorType.Add,
						                                         ParenthesizeIfNeeded(newRight).Clone());
					} else {
						return false;
					}
				}

				return true;
			}

			IfElseStatement condition = statement as IfElseStatement;
			if (condition != null) {
				Expression ifLeft, ifRight;
				if (!ExtractExpression(condition.TrueStatement, out ifLeft, out ifRight)) {
					leftSide = null;
					rightSide = null;
					return false;
				}

				Expression elseLeft, elseRight;
				if (!ExtractExpression(condition.FalseStatement, out elseLeft, out elseRight)) {
					leftSide = null;
					rightSide = null;
					return false;
				}

				if (ifLeft == null && elseLeft == null) {
					leftSide = null;
					rightSide = null;
					return true;
				}

				if (ifLeft != null && elseLeft != null && !SameNode(ifLeft, elseLeft)) {
					leftSide = null;
					rightSide = null;
					return false;
				}

				ifRight = ifRight ?? new PrimitiveExpression(0);
				elseRight = elseRight ?? new PrimitiveExpression(0);

				leftSide = ifLeft ?? elseLeft;
				rightSide = new ConditionalExpression(condition.Condition.Clone(), ifRight.Clone(), elseRight.Clone());
				return true;
			}

			leftSide = null;
			rightSide = null;
			return false;
		}

		Expression ParenthesizeIfNeeded(Expression expr)
		{
			if (expr is ConditionalExpression) {
				return new ParenthesizedExpression(expr.Clone());
			}

			var binaryExpression = expr as BinaryOperatorExpression;
			if (binaryExpression != null) {
				if (binaryExpression.Operator != BinaryOperatorType.Multiply &&
					binaryExpression.Operator != BinaryOperatorType.Divide &&
					binaryExpression.Operator != BinaryOperatorType.Modulus) {

					return new ParenthesizedExpression(expr.Clone());
				}
			}

			return expr;
		}

		bool SameNode(INode expr1, INode expr2)
		{
			Match m = expr1.Match(expr2);
			return m.Success;
		}

		static ForeachStatement GetForeachStatement (RefactoringContext context)
		{
			var foreachStatement = context.GetNode<ForeachStatement>();
			if (foreachStatement == null || !foreachStatement.ForeachToken.Contains(context.Location))
				return null;

			return foreachStatement;
		}
	}
}

