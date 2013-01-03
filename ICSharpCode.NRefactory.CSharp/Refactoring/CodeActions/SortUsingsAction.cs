using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Sort usings", Description = "Sorts usings by their origin and then alphabetically.")]
	public class SortUsingsAction: ICodeActionProvider
	{
		public IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var usingNode = FindUsingNodeAtCursor(context);
			if (usingNode == null)
				yield break;

			yield return new CodeAction(context.TranslateString("Sort usings"), script =>
			{
				var blocks = EnumerateUsingBlocks(context.RootNode);

				foreach (var block in blocks)
				{
					var originalNodes = block.ToArray();
					var sortedNodes = UsingHelper.SortUsingBlock(originalNodes, context).ToArray();

					for (var i = 0; i < originalNodes.Length; ++i)
						script.Replace(originalNodes[i], sortedNodes[i].Clone());
				}
			});
		}

		private static AstNode FindUsingNodeAtCursor(RefactoringContext context)
		{
			// If cursor is inside using declaration
			var locationAsIs = context.Location;
			// If cursor is at end of line with using declaration
			var locationLeft = new TextLocation(locationAsIs.Line, locationAsIs.Column - 1);

			var possibleNodes = new[] { locationAsIs, locationLeft }
				.Select(_ => context.RootNode.GetNodeAt(_, IsUsingDeclaration));
			var usingNode = possibleNodes.Where(_ => _ != null).Distinct().SingleOrDefault();

			return usingNode;
		}

		private static bool IsUsingDeclaration(AstNode node)
		{
			return node is UsingDeclaration || node is UsingAliasDeclaration;
		}

		private static IEnumerable<IEnumerable<AstNode>> EnumerateUsingBlocks(AstNode root)
		{
			var alreadyAddedNodes = new HashSet<AstNode>();

			foreach (var child in root.Descendants)
				if (IsUsingDeclaration(child) && !alreadyAddedNodes.Contains(child)) {
					var blockNodes = EnumerateUsingBlockNodes(child);

					alreadyAddedNodes.UnionWith(blockNodes);
					yield return blockNodes;
				}
		}

		private static IEnumerable<AstNode> EnumerateUsingBlockNodes(AstNode firstNode)
		{
			for (var node = firstNode; IsUsingDeclaration(node); node = node.NextSibling)
				yield return node;
		}
	}
}
