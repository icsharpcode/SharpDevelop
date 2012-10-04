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
					var sortedNodes = SortUsingBlock(originalNodes, context).ToArray();

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

		private static IEnumerable<AstNode> SortUsingBlock(IEnumerable<AstNode> nodes, RefactoringContext context)
		{
			var infos = nodes.Select(_ => new UsingInfo(_, context));
			var orderedInfos = infos.OrderBy(_ => _, new UsingInfoComparer());
			var orderedNodes = orderedInfos.Select(_ => _.Node);

			return orderedNodes;
		}


		private sealed class UsingInfo
		{
			public AstNode Node { get; private set; }

			public string Alias { get; private set; }
			public string Name { get; private set; }

			public bool IsAlias { get; private set; }
			public bool IsAssembly { get; private set; }
			public bool IsSystem { get; private set; }

			public UsingInfo(AstNode node, RefactoringContext context)
			{
				var importAndAlias = GetImportAndAlias(node);

				Node = node;

				Alias = importAndAlias.Item2;
				Name = importAndAlias.Item1.ToString();

				IsAlias = Alias != null;

				var result = context.Resolve(importAndAlias.Item1) as NamespaceResolveResult;
				var mainSourceAssembly = result != null ? result.Namespace.ContributingAssemblies.First() : null;
				var unresolvedAssembly = mainSourceAssembly != null ? mainSourceAssembly.UnresolvedAssembly : null;
				IsAssembly = unresolvedAssembly is DefaultUnresolvedAssembly;

				IsSystem = IsAssembly && Name.StartsWith("System");
			}

			private static Tuple<AstType, string> GetImportAndAlias(AstNode node)
			{
				var plainUsing = node as UsingDeclaration;
				if (plainUsing != null)
					return Tuple.Create(plainUsing.Import, (string)null);
				
				var aliasUsing = node as UsingAliasDeclaration;
				if (aliasUsing != null)
					return Tuple.Create(aliasUsing.Import, aliasUsing.Alias);

				throw new InvalidOperationException(string.Format("Invalid using node: {0}", node));
			}
		}

		private sealed class UsingInfoComparer: IComparer<UsingInfo>
		{
			public int Compare(UsingInfo x, UsingInfo y)
			{
				if (x.IsAlias != y.IsAlias)
					return x.IsAlias && !y.IsAlias ? 1 : -1;
				else if (x.IsAssembly != y.IsAssembly)
					return x.IsAssembly && !y.IsAssembly ? -1 : 1;
				else if (x.IsSystem != y.IsSystem)
					return x.IsSystem && !y.IsSystem ? -1 : 1;
				else if (x.Alias != y.Alias)
					return Comparer<string>.Default.Compare(x.Alias, y.Alias);
				else if (x.Name != y.Name)
					return Comparer<string>.Default.Compare(x.Name, y.Name);
				else
					return 0;
			}
		}
	}
}
