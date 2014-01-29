//
// ConvertIfToAndExpressionIssue.cs
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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("'if' statement can be re-written as '&&' expression",
	                  Description = "Convert 'if' to '&&' expression",
	                  Category = IssueCategories.Opportunities,
	                  Severity = Severity.Hint)]
	public class ConvertIfToAndExpressionIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ConvertIfToAndExpressionIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			static readonly AstNode ifPattern = 
				new IfElseStatement(
					new AnyNode ("condition"),
					PatternHelper.EmbeddedStatement (
						new AssignmentExpression(
							new AnyNode("target"),
							new PrimitiveExpression (false)
						)
					)
				);

			static readonly AstNode varDelarationPattern = 
				new VariableDeclarationStatement(new AnyNode("type"), Pattern.AnyString, new AnyNode("initializer"));

			void AddTo(IfElseStatement ifElseStatement, VariableDeclarationStatement varDeclaration, Expression expr)
			{
			}

			public override void VisitIfElseStatement(IfElseStatement ifElseStatement)
			{
				base.VisitIfElseStatement(ifElseStatement);

				var match = ifPattern.Match(ifElseStatement);
				if (match.Success) {
					var varDeclaration = ifElseStatement.GetPrevSibling(s => s.Role == BlockStatement.StatementRole) as VariableDeclarationStatement;
					var target = match.Get<Expression>("target").Single();
					var match2 = varDelarationPattern.Match(varDeclaration);
					if (match2.Success) {
						var initializer = varDeclaration.Variables.FirstOrDefault();
						if (initializer != null && target is IdentifierExpression && ((IdentifierExpression)target).Identifier != initializer.Name)
							return;
						var expr = match.Get<Expression>("condition").Single();
						if (!ConvertIfToOrExpressionIssue.CheckTarget(target, expr))
							return;
						AddIssue(new CodeIssue(
							ifElseStatement.IfToken,
							ctx.TranslateString("Convert to '&&' expresssion"),
							ctx.TranslateString("Replace with '&&'"),
							script => {
								var variable = initializer;
								var initalizerExpression = variable.Initializer.Clone();
								var bOp = initalizerExpression as BinaryOperatorExpression;
								if (bOp != null && bOp.Operator == BinaryOperatorType.ConditionalOr)
									initalizerExpression = new ParenthesizedExpression (initalizerExpression);
								script.Replace(
									varDeclaration, 
									new VariableDeclarationStatement(
										varDeclaration.Type.Clone(),
										variable.Name,
										new BinaryOperatorExpression(initalizerExpression, BinaryOperatorType.ConditionalAnd, CSharpUtil.InvertCondition(expr)) 
									)
								);
							script.Remove(ifElseStatement); 
						}
						) {
							IssueMarker = IssueMarker.DottedLine
						});
						return;
					} else {
						var expr = match.Get<Expression>("condition").Single();
						if (!ConvertIfToOrExpressionIssue.CheckTarget(target, expr))
							return;
						AddIssue(new CodeIssue(
							ifElseStatement.IfToken,
							ctx.TranslateString("Convert to '&=' expresssion"),
							ctx.TranslateString("Replace with '&='"),
							script =>
								script.Replace(
									ifElseStatement, 
									new ExpressionStatement(
										new AssignmentExpression(
											target.Clone(),
											AssignmentOperatorType.BitwiseAnd,
											CSharpUtil.InvertCondition(expr)
										) 
									)
								)
						) { IssueMarker = IssueMarker.DottedLine });
					}
				}
			}
		}
	}
}

