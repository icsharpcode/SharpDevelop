//
// UseAsAndNullCheckAction.cs
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
using ICSharpCode.NRefactory.PatternMatching;
using System.Linq;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction(
		"Use 'as' and null check", 
		Description = "Converts a 'is' into an 'as' and null check")]
	public class UseAsAndNullCheckAction : SpecializedCodeAction<IfElseStatement>
	{
		static readonly AstNode pattern = 
			new IfElseStatement(
				new NamedNode ("isExpression", PatternHelper.OptionalParentheses(new IsExpression(PatternHelper.OptionalParentheses(new AnyNode()), PatternHelper.AnyType()))),
				new AnyNode("embedded"),
				new AnyNodeOrNull()
			);

		static readonly AstNode negatedPattern = 
			new IfElseStatement(
				new UnaryOperatorExpression (UnaryOperatorType.Not, new NamedNode ("isExpression", PatternHelper.OptionalParentheses(new IsExpression(PatternHelper.OptionalParentheses(new AnyNode()), PatternHelper.AnyType())))),
				PatternHelper.EmbeddedStatement(new Choice { new ReturnStatement (new AnyNodeOrNull()), new BreakStatement (), new ContinueStatement () } ),
				new AnyNodeOrNull()
			);


		internal static bool IsEmbeddedStatement(AstNode stmt)
		{
			return stmt.Role == Roles.EmbeddedStatement || 
				stmt.Role == IfElseStatement.TrueRole || 
				stmt.Role == IfElseStatement.FalseRole;
		}

		static CodeAction HandleNegatedCase(BaseRefactoringContext ctx, IfElseStatement ifElseStatement, Match match, out IsExpression isExpression, out int foundCastCount)
		{
			foundCastCount = 0;
			var outerIs          = match.Get<Expression>("isExpression").Single();
			isExpression         = CSharpUtil.GetInnerMostExpression(outerIs) as IsExpression;
			var obj              = CSharpUtil.GetInnerMostExpression(isExpression.Expression);
			var castToType       = isExpression.Type;

			var cast = new Choice {
				PatternHelper.OptionalParentheses(PatternHelper.OptionalParentheses(obj.Clone()).CastTo(castToType.Clone())),
				PatternHelper.OptionalParentheses(PatternHelper.OptionalParentheses(obj.Clone()).CastAs(castToType.Clone()))
			};

			var rr = ctx.Resolve(castToType);
			if (rr == null || rr.IsError || rr.Type.IsReferenceType == false)
				return null;
			var foundCasts = ifElseStatement.GetParent<BlockStatement>().DescendantNodes(n => n.StartLocation >= ifElseStatement.StartLocation && !cast.IsMatch(n)).Where(n => cast.IsMatch(n)).ToList();
			foundCastCount = foundCasts.Count;

			return new CodeAction(ctx.TranslateString("Use 'as' and check for null"), script =>  {
				var varName = ctx.GetNameProposal(CreateMethodDeclarationAction.GuessNameFromType(rr.Type), ifElseStatement.StartLocation);
				var varDec = new VariableDeclarationStatement(new PrimitiveType("var"), varName, new AsExpression(obj.Clone(), castToType.Clone()));
				var binaryOperatorIdentifier = new IdentifierExpression(varName);
				var binaryOperatorExpression = new BinaryOperatorExpression(binaryOperatorIdentifier, BinaryOperatorType.Equality, new NullReferenceExpression());
				var linkedNodes = new List<AstNode>();
				linkedNodes.Add(varDec.Variables.First().NameToken);
				linkedNodes.Add(binaryOperatorIdentifier);
				if (IsEmbeddedStatement(ifElseStatement)) {
					var block = new BlockStatement();
					block.Add(varDec);
					var newIf = (IfElseStatement)ifElseStatement.Clone();
					newIf.Condition = binaryOperatorExpression;
					foreach (var node in newIf.DescendantNodesAndSelf(n => !cast.IsMatch(n)).Where(n => cast.IsMatch(n))) {
						var id = new IdentifierExpression(varName);
						linkedNodes.Add(id);
						node.ReplaceWith(id);
					}
					block.Add(newIf);
					script.Replace(ifElseStatement, block);
				}
				else {
					script.InsertBefore(ifElseStatement, varDec);
					script.Replace(ifElseStatement.Condition, binaryOperatorExpression);
					foreach (var c in foundCasts) {
						var id = new IdentifierExpression(varName);
						linkedNodes.Add(id);
						script.Replace(c, id);
					}
				}
				script.Link(linkedNodes);
			}, isExpression.IsToken);

		}

		internal static CodeAction ScanIfElse (BaseRefactoringContext ctx, IfElseStatement ifElseStatement, out IsExpression isExpression, out int foundCastCount)
		{
			foundCastCount = 0;
			var match = pattern.Match(ifElseStatement);
			if (!match.Success) {
				match = negatedPattern.Match(ifElseStatement);
				if (match.Success)
					return HandleNegatedCase(ctx, ifElseStatement, match, out isExpression, out foundCastCount);
				isExpression = null;
				return null;
			}

			var outerIs          = match.Get<Expression>("isExpression").Single();
			isExpression         = CSharpUtil.GetInnerMostExpression(outerIs) as IsExpression;
			var obj              = CSharpUtil.GetInnerMostExpression(isExpression.Expression);
			var castToType       = isExpression.Type;
			var embeddedStatment = match.Get<Statement>("embedded").Single();

			var cast = new Choice {
				PatternHelper.OptionalParentheses(PatternHelper.OptionalParentheses(obj.Clone()).CastTo(castToType.Clone())),
				PatternHelper.OptionalParentheses(PatternHelper.OptionalParentheses(obj.Clone()).CastAs(castToType.Clone()))
			};

			var rr = ctx.Resolve(castToType);
			if (rr == null || rr.IsError || rr.Type.IsReferenceType == false)
				return null;
			var foundCasts = embeddedStatment.DescendantNodesAndSelf(n => !cast.IsMatch(n)).Where(n => cast.IsMatch(n)).ToList();
			foundCastCount = foundCasts.Count;

			return new CodeAction(ctx.TranslateString("Use 'as' and check for null"), script =>  {
				var varName = ctx.GetNameProposal(CreateMethodDeclarationAction.GuessNameFromType(rr.Type), ifElseStatement.StartLocation);
				var varDec = new VariableDeclarationStatement(new PrimitiveType("var"), varName, new AsExpression(obj.Clone(), castToType.Clone()));
				var binaryOperatorIdentifier = new IdentifierExpression(varName);
				var binaryOperatorExpression = new BinaryOperatorExpression(binaryOperatorIdentifier, BinaryOperatorType.InEquality, new NullReferenceExpression());
				var linkedNodes = new List<AstNode>();
				linkedNodes.Add(varDec.Variables.First().NameToken);
				linkedNodes.Add(binaryOperatorIdentifier);
				if (IsEmbeddedStatement(ifElseStatement)) {
					var block = new BlockStatement();
					block.Add(varDec);
					var newIf = (IfElseStatement)ifElseStatement.Clone();
					newIf.Condition = binaryOperatorExpression;
					foreach (var node in newIf.DescendantNodesAndSelf(n => !cast.IsMatch(n)).Where(n => cast.IsMatch(n))) {
						var id = new IdentifierExpression(varName);
						linkedNodes.Add(id);
						node.ReplaceWith(id);
					}
					block.Add(newIf);
					script.Replace(ifElseStatement, block);
				}
				else {
					script.InsertBefore(ifElseStatement, varDec);
					script.Replace(outerIs, binaryOperatorExpression);
					foreach (var c in foundCasts) {
						var id = new IdentifierExpression(varName);
						linkedNodes.Add(id);
						script.Replace(c, id);
					}
				}
				script.Link(linkedNodes);
			}, isExpression.IsToken);

		}

		static List<AstNode> SearchCasts(RefactoringContext ctx, Statement embeddedStatement, Expression obj, AstType type, out ResolveResult rr)
		{
			var cast = new Choice {
				PatternHelper.OptionalParentheses(PatternHelper.OptionalParentheses(obj.Clone()).CastTo(type.Clone())),
				PatternHelper.OptionalParentheses(PatternHelper.OptionalParentheses(obj.Clone()).CastAs(type.Clone()))
			};

			rr = ctx.Resolve(type);
			if (rr == null || rr.IsError)
				return null;
			return embeddedStatement.DescendantNodesAndSelf(n => !cast.IsMatch(n)).Where(n => cast.IsMatch(n)).ToList();
		}

		protected override CodeAction GetAction(RefactoringContext ctx, IfElseStatement ifElseStatement)
		{
			var isExpr = ctx.GetNode<IsExpression>();
			if (isExpr == null || !isExpr.IsToken.Contains(ctx.Location))
				return null;

			int foundCasts;
			var action = ScanIfElse(ctx, ifElseStatement, out isExpr, out foundCasts);
			if (action != null)
				return action;

			isExpr = ctx.GetNode<IsExpression>();
			var node = isExpr.Parent;
			while (node is ParenthesizedExpression)
				node = node.Parent;
			var uOp = node as UnaryOperatorExpression;
			if (uOp != null && uOp.Operator == UnaryOperatorType.Not) {
				var rr = ctx.Resolve(isExpr.Type);
				if (rr == null || rr.IsError || rr.Type.IsReferenceType == false)
					return null;

				return new CodeAction(ctx.TranslateString("Use 'as' and check for null"), script =>  {
					var varName = ctx.GetNameProposal(CreateMethodDeclarationAction.GuessNameFromType(rr.Type), ifElseStatement.StartLocation);
					var varDec = new VariableDeclarationStatement(new PrimitiveType("var"), varName, new AsExpression(isExpr.Expression.Clone(), isExpr.Type.Clone()));
					var binaryOperatorIdentifier = new IdentifierExpression(varName);
					var binaryOperatorExpression = new BinaryOperatorExpression(binaryOperatorIdentifier, BinaryOperatorType.Equality, new NullReferenceExpression());
					if (IsEmbeddedStatement(ifElseStatement)) {
						var block = new BlockStatement();
						block.Add(varDec);
						var newIf = (IfElseStatement)ifElseStatement.Clone();
						newIf.Condition = binaryOperatorExpression;
						block.Add(newIf);
						script.Replace(ifElseStatement, block);
					}
					else {
						script.InsertBefore(ifElseStatement, varDec);
						script.Replace(uOp, binaryOperatorExpression);
					}
				}, isExpr.IsToken);
			}

			var obj = isExpr.Expression;
			var castToType = isExpr.Type;

			var cast = new Choice {
				PatternHelper.OptionalParentheses(PatternHelper.OptionalParentheses(obj.Clone()).CastTo(castToType.Clone())),
				PatternHelper.OptionalParentheses(PatternHelper.OptionalParentheses(obj.Clone()).CastAs(castToType.Clone()))
			};

			var rr2 = ctx.Resolve(castToType);
			if (rr2 == null || rr2.IsError || rr2.Type.IsReferenceType == false)
				return null;
			var foundCasts2 = isExpr.GetParent<Statement>().DescendantNodesAndSelf(n => !cast.IsMatch(n)).Where(n => isExpr.StartLocation < n.StartLocation && cast.IsMatch(n)).ToList();

			return new CodeAction(ctx.TranslateString("Use 'as' and check for null"), script =>  {
				var varName = ctx.GetNameProposal(CreateMethodDeclarationAction.GuessNameFromType(rr2.Type), ifElseStatement.StartLocation);
				var varDec = new VariableDeclarationStatement(new PrimitiveType("var"), varName, new AsExpression(obj.Clone(), castToType.Clone()));
				var binaryOperatorIdentifier = new IdentifierExpression(varName);
				var binaryOperatorExpression = new BinaryOperatorExpression(binaryOperatorIdentifier, BinaryOperatorType.InEquality, new NullReferenceExpression());
				var linkedNodes = new List<AstNode>();
				linkedNodes.Add(varDec.Variables.First().NameToken);
				linkedNodes.Add(binaryOperatorIdentifier);
				if (IsEmbeddedStatement(ifElseStatement)) {
					var block = new BlockStatement();
					block.Add(varDec);
					var newIf = (IfElseStatement)ifElseStatement.Clone();
					newIf.Condition = binaryOperatorExpression;
					foreach (var node2 in newIf.DescendantNodesAndSelf(n => !cast.IsMatch(n)).Where(n => cast.IsMatch(n))) {
						var id = new IdentifierExpression(varName);
						linkedNodes.Add(id);
						node2.ReplaceWith(id);
					}
					block.Add(newIf);
					script.Replace(ifElseStatement, block);
				}
				else {
					script.InsertBefore(ifElseStatement, varDec);
					script.Replace(isExpr, binaryOperatorExpression);
					foreach (var c in foundCasts2) {
						var id = new IdentifierExpression(varName);
						linkedNodes.Add(id);
						script.Replace(c, id);
					}
				}
				script.Link(linkedNodes);
			}, isExpr.IsToken);
		}
	}
}
