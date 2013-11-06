// 
// RemoveBraces.cs
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
using System.Threading;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Remove braces", Description = "Removes redundant braces around a statement.")]
	public class RemoveBracesAction : CodeActionProvider
	{
		internal static bool IsSpecialNode (AstNode node, out string keyword, out Statement embeddedStatement)
		{
			if (node != null) {
				if (node.Role == IfElseStatement.IfKeywordRole) {
					keyword = "if";
					embeddedStatement = ((IfElseStatement)node.Parent).TrueStatement;
					return true;
				}

				if (node.Role == IfElseStatement.ElseKeywordRole) {
					keyword = "else";
					embeddedStatement = ((IfElseStatement)node.Parent).FalseStatement;
					return true;
				}

				if (node.Role == DoWhileStatement.DoKeywordRole || node.Role == DoWhileStatement.WhileKeywordRole) {
					keyword = "do";
					embeddedStatement = ((DoWhileStatement)node.Parent).EmbeddedStatement;
					return true;
				}

				if (node.Role == ForeachStatement.ForeachKeywordRole) {
					keyword = "foreach";
					embeddedStatement = ((ForeachStatement)node.Parent).EmbeddedStatement;
					return true;
				}

				if (node.Role == ForStatement.ForKeywordRole) {
					keyword = "for";
					embeddedStatement = ((ForStatement)node.Parent).EmbeddedStatement;
					return true;
				}

				if (node.Role == LockStatement.LockKeywordRole) {
					keyword = "lock";
					embeddedStatement = ((LockStatement)node.Parent).EmbeddedStatement;
					return true;
				}

				if (node.Role == UsingStatement.UsingKeywordRole) {
					keyword = "using";
					embeddedStatement = ((UsingStatement)node.Parent).EmbeddedStatement;
					return true;
				}

				if (node.Role == WhileStatement.WhileKeywordRole) {
					keyword = "while";
					embeddedStatement = ((WhileStatement)node.Parent).EmbeddedStatement;
					return true;
				}
			}
			keyword = null;
			embeddedStatement = null;
			return false;
		}

		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			string keyword;
			Statement embeddedStatement;
			BlockStatement block;

			var currentNode = context.GetNode();
			if (IsSpecialNode(currentNode, out keyword, out embeddedStatement)) {
				block = embeddedStatement as BlockStatement;
				if (block == null || block.Statements.Count != 1 || block.Statements.First() is VariableDeclarationStatement)
					yield break;
			} else {
				block = GetBlockStatement(context);
			}

			if (block == null)
				yield break;

			yield return new CodeAction (
				keyword != null ? string.Format(context.TranslateString("Remove braces from '{0}'"), keyword) : context.TranslateString("Remove braces"), 
				script => {
					var start = script.GetCurrentOffset (block.LBraceToken.GetPrevNode ().EndLocation);
					var end = script.GetCurrentOffset (block.LBraceToken.EndLocation);
					if (end <= start)
						return;
					script.RemoveText (start, end - start);
					script.Remove(block.LBraceToken);
					script.Remove(block.RBraceToken);
					script.FormatText(block.Parent);
				}, 
				currentNode
			);
		}
		
		static BlockStatement GetBlockStatement(RefactoringContext context)
		{
			var block = context.GetNode<BlockStatement>();
			if (block == null || block.LBraceToken.IsNull || block.RBraceToken.IsNull)
				return null;
			if (!(block.LBraceToken.IsInside(context.Location) || block.RBraceToken.IsInside(context.Location)))
				return null;
			if (!(block.Parent is Statement) || block.Parent is TryCatchStatement) 
				return null;
			if (block.Statements.Count != 1 || block.Statements.First () is VariableDeclarationStatement) 
				return null;
			return block;
		}
	}
}
