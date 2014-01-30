//
// ReverseDirectionForForLoopAction.cs
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
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Reverse the direction of a for loop", Description = "Reverse the direction of a for loop")]
	public class ReverseDirectionForForLoopAction : SpecializedCodeAction<ForStatement>
	{
		static bool? IsForward(ExpressionStatement statement, string name, out Expression step)
		{
			step = null;
			if (statement == null)
				return null;

			var forwardPattern = new Choice {
				PatternHelper.OptionalParentheses(new UnaryOperatorExpression (UnaryOperatorType.Increment, new IdentifierExpression(name))),
				PatternHelper.OptionalParentheses(new UnaryOperatorExpression (UnaryOperatorType.PostIncrement, new IdentifierExpression(name))),
				PatternHelper.OptionalParentheses(new AssignmentExpression (new IdentifierExpression(name), AssignmentOperatorType.Add, PatternHelper.OptionalParentheses(new AnyNode("step"))))
			};
			var match = forwardPattern.Match(statement.Expression);
			if (match.Success) {
				step = match.Get<Expression>("step").FirstOrDefault();
				return true;
			}

			var backwardPattern = new Choice {
				PatternHelper.OptionalParentheses(new UnaryOperatorExpression (UnaryOperatorType.Decrement, new IdentifierExpression(name))),
				PatternHelper.OptionalParentheses(new UnaryOperatorExpression (UnaryOperatorType.PostDecrement, new IdentifierExpression(name))),
				PatternHelper.OptionalParentheses(new AssignmentExpression (new IdentifierExpression(name), AssignmentOperatorType.Subtract, PatternHelper.OptionalParentheses(new AnyNode("step"))))
			};

			match = backwardPattern.Match(statement.Expression);
			if (match.Success) {
				step = match.Get<Expression>("step").FirstOrDefault();
				return false;
			}
			return null;
		}

		Expression GetNewCondition(Expression condition, VariableInitializer initializer, bool? direction, Expression step, out Expression newInitializer)
		{
			var name = initializer.Name;
			newInitializer = null;
			var bOp = condition as BinaryOperatorExpression;
			if (bOp == null || direction == null)
				return null;
			if (direction == true) {
				var upwardPattern = new Choice {
					PatternHelper.OptionalParentheses(new BinaryOperatorExpression(PatternHelper.OptionalParentheses(new IdentifierExpression(name)), BinaryOperatorType.LessThan, PatternHelper.OptionalParentheses(new AnyNode("bound")))),
					PatternHelper.OptionalParentheses(new BinaryOperatorExpression(PatternHelper.OptionalParentheses(new AnyNode("bound")), BinaryOperatorType.GreaterThan, PatternHelper.OptionalParentheses(new IdentifierExpression(name))))
				};
				var upMatch = upwardPattern.Match(condition);
				if (upMatch.Success) {
					var bound = upMatch.Get<Expression>("bound").FirstOrDefault();
					newInitializer = direction == true ? Subtract(bound, step) : bound.Clone();
					return GetNewBound(name, false, initializer.Initializer.Clone(), step);
				}

				var altUpwardPattern = new Choice {
					PatternHelper.OptionalParentheses(new BinaryOperatorExpression(PatternHelper.OptionalParentheses(new IdentifierExpression(name)), BinaryOperatorType.LessThanOrEqual, PatternHelper.OptionalParentheses(new AnyNode("bound")))),
					PatternHelper.OptionalParentheses(new BinaryOperatorExpression(PatternHelper.OptionalParentheses(new AnyNode("bound")), BinaryOperatorType.GreaterThanOrEqual, PatternHelper.OptionalParentheses(new IdentifierExpression(name))))
				};
				var altUpMatch = altUpwardPattern.Match(condition);
				if (altUpMatch.Success) {
					var bound = altUpMatch.Get<Expression>("bound").FirstOrDefault();
					newInitializer = bound.Clone();
					return GetNewBound(name, false, initializer.Initializer.Clone(), step);
				}
			}

			var downPattern = new Choice {
				PatternHelper.OptionalParentheses(new BinaryOperatorExpression(PatternHelper.OptionalParentheses(new IdentifierExpression(name)), BinaryOperatorType.GreaterThanOrEqual, PatternHelper.OptionalParentheses(new AnyNode("bound")))),
				PatternHelper.OptionalParentheses(new BinaryOperatorExpression(PatternHelper.OptionalParentheses(new AnyNode("bound")), BinaryOperatorType.LessThanOrEqual, PatternHelper.OptionalParentheses(new IdentifierExpression(name))))
			};
			var downMatch = downPattern.Match(condition);
			if (!downMatch.Success)
				return null;
			var bound2 = downMatch.Get<Expression>("bound").FirstOrDefault();
			newInitializer = direction == true ? Subtract(bound2, step) : bound2.Clone();
			return GetNewBound(name, true, initializer.Initializer.Clone(), step);
		}

		static readonly AstNode zeroExpr = new PrimitiveExpression(0);
		static readonly AstNode oneExpr = new PrimitiveExpression(1);

		Expression Subtract(Expression expr, Expression step)
		{
			if (step != null && CSharpUtil.GetInnerMostExpression(expr).IsMatch(CSharpUtil.GetInnerMostExpression(step)))
				return new PrimitiveExpression(0);

			var pe = expr as PrimitiveExpression;
			if (pe != null) {
				if (step == null)
					return new PrimitiveExpression((int)pe.Value - 1);
				var stepExpr = step as PrimitiveExpression;
				if (stepExpr != null)
					return new PrimitiveExpression((int)pe.Value - (int)stepExpr.Value);
			} 

			var bOp = expr as BinaryOperatorExpression;
			if (bOp != null) {
				if (bOp.Operator == BinaryOperatorType.Subtract) {
					var right = Add(bOp.Right, step);
					if (zeroExpr.IsMatch(right))
						return bOp.Left.Clone();
					return new BinaryOperatorExpression(bOp.Left.Clone(), BinaryOperatorType.Subtract, right);
				}
				if (bOp.Operator == BinaryOperatorType.Add) {
					var right = Subtract(bOp.Right, step);
					if (zeroExpr.IsMatch(right))
						return bOp.Left.Clone();
					return new BinaryOperatorExpression(bOp.Left.Clone(), BinaryOperatorType.Add, Subtract(bOp.Right, step));
				}
			} 
			if (step == null)
				return new BinaryOperatorExpression(expr.Clone(), BinaryOperatorType.Subtract, new PrimitiveExpression(1));

			return new BinaryOperatorExpression(expr.Clone(), BinaryOperatorType.Subtract, CSharpUtil.AddParensForUnaryExpressionIfRequired(step.Clone()));
		}

		Expression Add(Expression expr, Expression step)
		{
			var pe = expr as PrimitiveExpression;
			if (pe != null) {
				if (step == null)
					return new PrimitiveExpression((int)pe.Value + 1);
				var stepExpr = step as PrimitiveExpression;
				if (stepExpr != null)
					return new PrimitiveExpression((int)pe.Value + (int)stepExpr.Value);
			} 

			var bOp = expr as BinaryOperatorExpression;
			if (bOp != null) {
				if (bOp.Operator == BinaryOperatorType.Add) {
					var right = Add(bOp.Right, step);
					if (zeroExpr.IsMatch(right))
						return bOp.Left.Clone();
					return new BinaryOperatorExpression(bOp.Left.Clone(), BinaryOperatorType.Add, right);
				}
				if (bOp.Operator == BinaryOperatorType.Subtract) {
					var right = Subtract(bOp.Right, step);
					if (zeroExpr.IsMatch(right))
						return bOp.Left.Clone();
					return new BinaryOperatorExpression(bOp.Left.Clone(), BinaryOperatorType.Subtract, right);
				}
			} 
			if (step == null)
				return new BinaryOperatorExpression(expr.Clone(), BinaryOperatorType.Add, new PrimitiveExpression(1));

			return new BinaryOperatorExpression(expr.Clone(), BinaryOperatorType.Add, CSharpUtil.AddParensForUnaryExpressionIfRequired(step.Clone()));
		}

		Expression GetNewBound(string name, bool? direction, Expression initializer, Expression step)
		{
			if (initializer == null)
				return null;

			return new BinaryOperatorExpression (
				new IdentifierExpression (name),
				direction == true ? BinaryOperatorType.LessThan : BinaryOperatorType.GreaterThanOrEqual,
				direction == true ? Add(initializer, step) : initializer
			);
		}

		Expression CreateIterator(string name, bool? direction, Expression step)
		{
			if (direction == true) {
				if (step == null || oneExpr.IsMatch(step)) {
					return new UnaryOperatorExpression(UnaryOperatorType.PostIncrement, new IdentifierExpression(name));
				}
				return new AssignmentExpression(new IdentifierExpression(name), AssignmentOperatorType.Add, step.Clone());
			}
			if (step == null || oneExpr.IsMatch(step)) {
				return new UnaryOperatorExpression(UnaryOperatorType.PostDecrement, new IdentifierExpression(name));
			}
			return new AssignmentExpression(new IdentifierExpression(name), AssignmentOperatorType.Subtract, step.Clone());
		}

		protected override CodeAction GetAction(RefactoringContext context, ForStatement node)
		{
			if (!node.ForToken.Contains(context.Location))
				return null;
			if (node.Initializers.Count() != 1)
				return null;
			var varDelc = node.Initializers.Single() as VariableDeclarationStatement;
			if (varDelc == null)
				return null;

			if (varDelc.Variables.Count() != 1)
				return null;
			var initalizer = varDelc.Variables.First ();
			if (initalizer == null)
				return null;

			if (!context.Resolve(initalizer.Initializer).Type.IsKnownType(KnownTypeCode.Int32))
				return null;

			if (node.Iterators.Count() != 1)
				return null;
			var iterator = node.Iterators.First();
			Expression step;
			var direction = IsForward(iterator as ExpressionStatement, initalizer.Name, out step);

			Expression newInitializer;
			var newCondition = GetNewCondition(node.Condition, initalizer, direction, step, out newInitializer);
			if (newCondition == null)
				return null;

			return new CodeAction (
				context.TranslateString("Reverse 'for' loop"),
				s => {
					var newFor = new ForStatement() {
						Initializers = { new VariableDeclarationStatement(varDelc.Type.Clone(), initalizer.Name, newInitializer) },
						Condition = newCondition,
						Iterators = { CreateIterator(initalizer.Name, !direction, step) },
						EmbeddedStatement = node.EmbeddedStatement.Clone()
					};
					s.Replace(node, newFor);
				},
				node.ForToken
			);
		}
	}
}

