// 
// ConvertForeachToFor.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2011 Mike Krüger <mkrueger@novell.com>
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
using ICSharpCode.NRefactory.TypeSystem;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Converts a foreach loop to for.
	/// </summary>
	[ContextAction("Convert 'foreach' loop to 'for'", Description = "Works on 'foreach' loops that allow direct access to its elements.")]
	public class ConvertForeachToForAction : CodeActionProvider
	{
		static readonly string[] VariableNames = { "i", "j", "k", "l", "n", "m", "x", "y", "z"};
		static readonly string[] CollectionNames = { "list" };

		static string GetName(ICSharpCode.NRefactory.CSharp.Resolver.CSharpResolver state, string[] variableNames)
		{
			for (int i = 0; i < 1000; i++) {
				foreach (var vn in variableNames) {
					string id = i > 0 ? vn + i : vn;
					var rr = state.LookupSimpleNameOrTypeName(id, new IType[0], NameLookupMode.Expression);
					if (rr.IsError) 
						return id;
				}
			}
			return null;
		}

		string GetBoundName(Expression inExpression)
		{
			var ie = inExpression as IdentifierExpression;
			if (ie != null)
				return ie.Identifier;
			var mre = inExpression as MemberReferenceExpression;
			if (mre != null)
				return GetBoundName(mre.Target) + mre.MemberName;
			return "max";
		}

		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			bool hasIndexAccess;
			var foreachStatement = GetForeachStatement(context, out hasIndexAccess);
			if (foreachStatement == null || foreachStatement.EmbeddedStatement == null)
				yield break;
			var state = context.GetResolverStateBefore (foreachStatement.EmbeddedStatement);
			string name = GetName(state, VariableNames);
			if (name == null) // very unlikely, but just in case ...
				yield break;

			yield return new CodeAction(context.TranslateString("Convert 'foreach' loop to 'for'"), script => {
				var result = context.Resolve(foreachStatement.InExpression);
				var countProperty = GetCountProperty(result.Type);
				var inExpression = foreachStatement.InExpression;

				var initializer = hasIndexAccess ? new VariableDeclarationStatement(new PrimitiveType("int"), name, new PrimitiveExpression(0)) :
				                  new VariableDeclarationStatement(new SimpleType("var"), name, new InvocationExpression(new MemberReferenceExpression (inExpression.Clone (), "GetEnumerator")));
				var id1 = new IdentifierExpression(name);
				var id2 = id1.Clone();
				var id3 = id1.Clone();
				Statement declarationStatement = null;
				if (inExpression is ObjectCreateExpression || inExpression is ArrayCreateExpression) {
					string listName = GetName(state, CollectionNames) ?? "col";
					declarationStatement = new VariableDeclarationStatement (
						new PrimitiveType ("var"),
						listName,
						inExpression.Clone ()
					);
					inExpression = new IdentifierExpression (listName);
				}

				var variableDeclarationStatement = new VariableDeclarationStatement(
					foreachStatement.VariableType.Clone(),
					foreachStatement.VariableName,
					hasIndexAccess ? (Expression)new IndexerExpression(inExpression.Clone(), id3) : new MemberReferenceExpression(id1, "Current")
				);

				var forStatement = new ForStatement {
					Initializers = { initializer },
					Condition = hasIndexAccess ? (Expression)new BinaryOperatorExpression (id1, BinaryOperatorType.LessThan, new MemberReferenceExpression (inExpression.Clone (), countProperty)) :
					            new InvocationExpression(new MemberReferenceExpression (id2, "MoveNext")),
					EmbeddedStatement = new BlockStatement {
						variableDeclarationStatement
					}
				};

				if (hasIndexAccess)
					forStatement.Iterators.Add(new UnaryOperatorExpression (UnaryOperatorType.PostIncrement, id2));

				if (foreachStatement.EmbeddedStatement is BlockStatement) {
					variableDeclarationStatement.Remove();
					var oldBlock = (BlockStatement)foreachStatement.EmbeddedStatement.Clone();
					if (oldBlock.Statements.Any()) {
						oldBlock.Statements.InsertBefore(oldBlock.Statements.First(), variableDeclarationStatement);
					} else {
						oldBlock.Statements.Add(variableDeclarationStatement);
					}
					forStatement.EmbeddedStatement = oldBlock;
				} else {
					forStatement.EmbeddedStatement.AddChild (foreachStatement.EmbeddedStatement.Clone (), BlockStatement.StatementRole);
				}
				if (declarationStatement != null)
					script.InsertBefore (foreachStatement, declarationStatement);
				script.Replace (foreachStatement, forStatement);
				if (hasIndexAccess) {
					script.Link (initializer.Variables.First ().NameToken, id1, id2, id3);
				} else {
					script.Link (initializer.Variables.First ().NameToken, id1, id2);
				}
			}, foreachStatement);

			if (!hasIndexAccess)
				yield break;
			yield return new CodeAction(context.TranslateString("Convert 'foreach' loop to optimized 'for'"), script => {
				var result = context.Resolve(foreachStatement.InExpression);
				var countProperty = GetCountProperty(result.Type);

				var initializer = new VariableDeclarationStatement(new PrimitiveType("int"), name, new PrimitiveExpression(0));
				var id1 = new IdentifierExpression(name);
				var id2 = id1.Clone();
				var id3 = id1.Clone();
				var inExpression = foreachStatement.InExpression;
				Statement declarationStatement = null;
				if (inExpression is ObjectCreateExpression || inExpression is ArrayCreateExpression) {
					string listName = GetName(state, CollectionNames) ?? "col";
					declarationStatement = new VariableDeclarationStatement (
						new PrimitiveType ("var"),
						listName,
						inExpression.Clone ()
						);
					inExpression = new IdentifierExpression (listName);
				}

				var variableDeclarationStatement = new VariableDeclarationStatement(
					foreachStatement.VariableType.Clone(),
					foreachStatement.VariableName,
					new IndexerExpression(inExpression.Clone(), id3)
					);

				string optimizedUpperBound = GetBoundName(inExpression) + countProperty;
				initializer.Variables.Add(new VariableInitializer(optimizedUpperBound, new MemberReferenceExpression (inExpression.Clone (), countProperty)));
				var forStatement = new ForStatement {
					Initializers = { initializer },
					Condition = new BinaryOperatorExpression (id1, BinaryOperatorType.LessThan, new IdentifierExpression(optimizedUpperBound)),
					Iterators = { new UnaryOperatorExpression (UnaryOperatorType.PostIncrement, id2) },
					EmbeddedStatement = new BlockStatement {
						variableDeclarationStatement
					}
				};

				if (foreachStatement.EmbeddedStatement is BlockStatement) {
					variableDeclarationStatement.Remove();
					var oldBlock = (BlockStatement)foreachStatement.EmbeddedStatement.Clone();
					if (oldBlock.Statements.Any()) {
						oldBlock.Statements.InsertBefore(oldBlock.Statements.First(), variableDeclarationStatement);
					} else {
						oldBlock.Statements.Add(variableDeclarationStatement);
					}
					forStatement.EmbeddedStatement = oldBlock;
				} else {
					forStatement.EmbeddedStatement.AddChild (foreachStatement.EmbeddedStatement.Clone (), BlockStatement.StatementRole);
				}
				if (declarationStatement != null)
					script.InsertBefore (foreachStatement, declarationStatement);
				script.Replace (foreachStatement, forStatement);
				script.Link (initializer.Variables.First ().NameToken, id1, id2, id3);
			}, foreachStatement);
		}
		
		static string GetCountProperty(IType type)
		{
			return type.Kind == TypeKind.Array ? "Length" : "Count";
		}

		static ForeachStatement GetForeachStatement (RefactoringContext context, out bool hasIndexAccess)
		{
			var astNode = context.GetNode ();
			if (astNode == null) {
				hasIndexAccess = false;
				return null;
			}
			var result = (astNode as ForeachStatement) ?? astNode.Parent as ForeachStatement;
			if (result == null) {
				hasIndexAccess = false;
				return null;
			}
			var collection = context.Resolve (result.InExpression);
			hasIndexAccess = collection.Type.Kind == TypeKind.Array || collection.Type.GetProperties(p => p.IsIndexer).Any();
			return result;
		}
	}
}
