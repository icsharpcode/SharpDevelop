// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
