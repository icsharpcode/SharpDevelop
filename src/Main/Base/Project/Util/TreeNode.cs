// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop
{
	// TODO: is this interface really useful?
	// the use with TypeGraphNode seems redundant, we can just directly use the TypeGraphNodes...
	
	/// <summary>
	/// Generic TreeNode with content and children.
	/// </summary>
	public interface ITreeNode<out TContent>
	{
		TContent Content { get; }
		
		IEnumerable<ITreeNode<TContent>> Children { get; }
	}
	
	public sealed class TreeNode<TContent> : ITreeNode<TContent>
	{
		public TreeNode(TContent content)
		{
			if (content == null)
				throw new ArgumentNullException("content");
			this.Content = content;
		}
		
		public TContent Content { get; private set; }
		
		public IEnumerable<ITreeNode<TContent>> Children { get; set; }
		
		public override string ToString()
		{
			return string.Format("[TreeNode {0}]", this.Content.ToString());
		}
		
		public static TreeNode<TContent> FromGraph<GraphNode>(GraphNode rootNode, Func<GraphNode, IEnumerable<GraphNode>> children, Func<GraphNode, TContent> content)
		{
			HashSet<GraphNode> visited = new HashSet<GraphNode>();
			return FromGraph(visited, rootNode, children, content);
		}
		
		static TreeNode<TContent> FromGraph<GraphNode>(HashSet<GraphNode> visited, GraphNode graphNode, Func<GraphNode, IEnumerable<GraphNode>> children, Func<GraphNode, TContent> content)
		{
			TreeNode<TContent> treeNode = new TreeNode<TContent>(content(graphNode));
			List<TreeNode<TContent>> childList = new List<TreeNode<TContent>>();
			foreach (GraphNode graphChild in children(graphNode)) {
				if (visited.Add(graphChild)) { // every graph node may appear only once in the tree
					childList.Add(FromGraph(visited, graphChild, children, content));
				}
			}
			treeNode.Children = childList;
			return treeNode;
		}
	}
}
