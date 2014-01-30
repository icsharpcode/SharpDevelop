// 
// LinqQueryToFluentAction.cs
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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Convert LINQ query to fluent syntax",
	               Description = "Converts a LINQ query to the equivalent fluent syntax.")]
	public class LinqQueryToFluentAction : SpecializedCodeAction<QueryExpression>
	{
		protected override CodeAction GetAction(RefactoringContext context, QueryExpression node)
		{
			AstNode currentNode = node;
			for (;;) {
				QueryContinuationClause continuationParent = currentNode.Parent as QueryContinuationClause;
				if (continuationParent != null) {
					currentNode = continuationParent;
					continue;
				}
				QueryExpression exprParent = currentNode.Parent as QueryExpression;
				if (exprParent != null) {
					currentNode = exprParent;
					continue;
				}

				break;
			}

			node = (QueryExpression)currentNode;

			return new CodeAction(context.TranslateString("Convert LINQ query to fluent syntax"),
			                      script => ConvertQueryToFluent(context, script, node),
			                      node);
		}

		static void ConvertQueryToFluent(RefactoringContext context, Script script, QueryExpression query) {
			IEnumerable<string> underscoreIdentifiers = GetNameProposals (context, query, "_");
			Expression newExpression = GetFluentFromQuery(query, underscoreIdentifiers);
			script.Replace (query, newExpression);
		}

		static IEnumerable<string> GetNameProposals(RefactoringContext context, QueryExpression query, string baseName)
		{
			var resolver = context.GetResolverStateBefore(query);
			int current = -1;
			string nameProposal;
			for (;;) {
				do {
					++current;
					nameProposal = baseName + (current == 0 ? string.Empty : current.ToString());
				} while (IsNameUsed (resolver, query, nameProposal));

				yield return nameProposal;
			}
		}

		static bool IsNameUsed(CSharpResolver resolver, QueryExpression query, string name)
		{
			if (resolver.ResolveSimpleName(name, new List<IType>()) is LocalResolveResult) {
				return true;
			}

			if (query.Ancestors.OfType <VariableInitializer>().Any(variable => variable.Name == name)) {
				return true;
			}

			if (query.Ancestors.OfType <BlockStatement>()
			    .Any(blockStatement => DeclaresLocalVariable(blockStatement, name))) {

				return true;
			}

			return query.Descendants.OfType<Identifier> ().Any (identifier => identifier.Name == name);
		}

		static bool DeclaresLocalVariable(BlockStatement blockStatement, string name) {
			return blockStatement.Descendants.OfType <VariableInitializer>()
				.Any(variable => variable.Name == name &&
				     variable.Ancestors.OfType<BlockStatement>().First() == blockStatement);
		}

		static Expression GetFluentFromQuery (QueryExpression query, IEnumerable<string> underscoreIdentifiers)
		{
			var queryExpander = new QueryExpressionExpander();
			var expandResult = queryExpander.ExpandQueryExpressions(query, underscoreIdentifiers);

			return (Expression) expandResult.AstNode;
		}
	}
}

