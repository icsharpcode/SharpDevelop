//
// ForCanBeConvertedToForeachIssue.cs
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
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("'for' can be converted into 'foreach'",
	                  Description = "Foreach loops are more efficient",
	                  Category = IssueCategories.Opportunities,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "ForCanBeConvertedToForeach")]
	public class ForCanBeConvertedToForeachIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ForCanBeConvertedToForeachIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base (ctx)
			{
			}

			static readonly AstNode forPattern =
				new Choice {
					new ForStatement {
						Initializers = {
							new VariableDeclarationStatement {
								Type = new AnyNode("int"),
								Variables = {
									new NamedNode("iteratorInitialzer", new VariableInitializer(Pattern.AnyString, new PrimitiveExpression(0)))
								}
							}
						},
						Condition = PatternHelper.OptionalParentheses(
							new BinaryOperatorExpression(
								PatternHelper.OptionalParentheses(new AnyNode("iterator")), 
								BinaryOperatorType.LessThan,
								PatternHelper.OptionalParentheses(
									new NamedNode("upperBound", new MemberReferenceExpression(new AnyNode (), Pattern.AnyString))
								)
							)
						),
						Iterators = {
							new ExpressionStatement(
								new Choice {
									new UnaryOperatorExpression(UnaryOperatorType.Increment, new Backreference("iterator")), 
									new UnaryOperatorExpression(UnaryOperatorType.PostIncrement, new Backreference("iterator")) 
								}
							)
						},
						EmbeddedStatement = new AnyNode("body")
					},
					new ForStatement {
						Initializers = {
							new VariableDeclarationStatement {
								Type = new AnyNode("int"),
								Variables = {
									new NamedNode("iteratorInitialzer", new VariableInitializer(Pattern.AnyString, new PrimitiveExpression(0))),
									new NamedNode("upperBoundInitializer", new VariableInitializer(Pattern.AnyString, new NamedNode("upperBound", new MemberReferenceExpression(new AnyNode (), Pattern.AnyString)))),
								}
							}
						},
						Condition = PatternHelper.OptionalParentheses(
							new BinaryOperatorExpression(
								PatternHelper.OptionalParentheses(new AnyNode("iterator")), 
								BinaryOperatorType.LessThan,
								PatternHelper.OptionalParentheses(
									new AnyNode("upperBoundInitializerName")
								)
							)
						),
						Iterators = {
							new ExpressionStatement(
								new Choice {
									new UnaryOperatorExpression(UnaryOperatorType.Increment, new Backreference("iterator")), 
									new UnaryOperatorExpression(UnaryOperatorType.PostIncrement, new Backreference("iterator")) 
								}
							)
						},
						EmbeddedStatement = new AnyNode("body")
					},
				};
			static readonly AstNode varDeclPattern =
				new VariableDeclarationStatement {
					Type = new AnyNode(),
					Variables = {
						new VariableInitializer(Pattern.AnyString, new NamedNode("indexer", new IndexerExpression(new AnyNode(), new IdentifierExpression(Pattern.AnyString))))
					}
				};
			static readonly AstNode varTypePattern =
				new SimpleType("var");

			static bool IsEnumerable(IType type)
			{
				return type.Name == "IEnumerable" && (type.Namespace == "System.Collections.Generic" || type.Namespace == "System.Collections");
			}

			public override void VisitForStatement(ForStatement forStatement)
			{
				base.VisitForStatement(forStatement);
				var forMatch = forPattern.Match(forStatement);
				if (!forMatch.Success)
					return;
				var body = forStatement.EmbeddedStatement as BlockStatement;
				if (body == null || !body.Statements.Any())
					return;
				var varDeclStmt = body.Statements.First() as VariableDeclarationStatement;
				if (varDeclStmt == null)
					return;
				var varMatch = varDeclPattern.Match(varDeclStmt);
				if (!varMatch.Success)
					return;
				var typeNode = forMatch.Get<AstNode>("int").FirstOrDefault();
				var varDecl = forMatch.Get<VariableInitializer>("iteratorInitialzer").FirstOrDefault();
				var iterator = forMatch.Get<IdentifierExpression>("iterator").FirstOrDefault();
				var upperBound = forMatch.Get<MemberReferenceExpression>("upperBound").FirstOrDefault();
				if (typeNode == null || varDecl == null || iterator == null || upperBound == null)
					return;

				// Check iterator type
				if (!varTypePattern.IsMatch(typeNode)) {
					var typeRR = ctx.Resolve(typeNode);
					if (!typeRR.Type.IsKnownType(KnownTypeCode.Int32))
						return;
				}

				if (varDecl.Name != iterator.Identifier)
					return;

				var upperBoundInitializer = forMatch.Get<VariableInitializer>("upperBoundInitializer").FirstOrDefault();
				var upperBoundInitializerName = forMatch.Get<IdentifierExpression>("upperBoundInitializerName").FirstOrDefault();
				if (upperBoundInitializer != null) {
					if (upperBoundInitializerName == null || upperBoundInitializer.Name != upperBoundInitializerName.Identifier)
						return;
				}

				var indexer = varMatch.Get<IndexerExpression>("indexer").Single();
				if (((IdentifierExpression)indexer.Arguments.First()).Identifier != iterator.Identifier)
					return;
				if (!indexer.Target.IsMatch(upperBound.Target))
					return;

				var rr = ctx.Resolve(upperBound) as MemberResolveResult;
				if (rr == null || rr.IsError)
					return;

				if (!(rr.Member.Name == "Length" && rr.Member.DeclaringType.Name == "Array" && rr.Member.DeclaringType.Namespace == "System") &&
				!(rr.Member.Name == "Count" && (IsEnumerable(rr.TargetResult.Type) || rr.TargetResult.Type.GetAllBaseTypes().Any(IsEnumerable))))
					return;

				var variableInitializer = varDeclStmt.Variables.First();
				var lr = ctx.Resolve(variableInitializer) as LocalResolveResult;
				if (lr == null)
					return;

				var ir = ctx.Resolve(varDecl) as LocalResolveResult;
				if (ir == null)
					return;

				var analyze = new ConvertToConstantIssue.VariableUsageAnalyzation(ctx);
				analyze.SetAnalyzedRange(
					varDeclStmt,
					forStatement.EmbeddedStatement,
					false
				);
				forStatement.EmbeddedStatement.AcceptVisitor(analyze);
				if (analyze.GetStatus(lr.Variable) == ICSharpCode.NRefactory.CSharp.Refactoring.ExtractMethod.VariableState.Changed ||
				    analyze.GetStatus(ir.Variable) == ICSharpCode.NRefactory.CSharp.Refactoring.ExtractMethod.VariableState.Changed ||
				    analyze.GetStatus(ir.Variable) == ICSharpCode.NRefactory.CSharp.Refactoring.ExtractMethod.VariableState.Used)
					return;

				AddIssue(new CodeIssue(
					forStatement.ForToken,
					ctx.TranslateString("'for' loop can be converted to 'foreach'"),
					ctx.TranslateString("Convert to 'foreach'"),
					script => {
						var foreachBody = (BlockStatement)forStatement.EmbeddedStatement.Clone();
						foreachBody.Statements.First().Remove();

						var fe = new ForeachStatement {
							VariableType = new PrimitiveType("var"),
							VariableName = variableInitializer.Name,
							InExpression = upperBound.Target.Clone(),
							EmbeddedStatement = foreachBody
						};
						script.Replace(forStatement, fe); 
					}
				) { IssueMarker = IssueMarker.DottedLine });

			}
		}
	}
}

