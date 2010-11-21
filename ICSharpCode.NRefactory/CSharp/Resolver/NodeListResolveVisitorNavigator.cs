// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// <see cref="IResolveVisitorNavigator"/> implementation that resolves a list of nodes.
	/// We will skip all nodes which are not the target nodes or ancestors of the target nodes.
	/// </summary>
	public sealed class NodeListResolveVisitorNavigator : IResolveVisitorNavigator
	{
		readonly Dictionary<INode, ResolveVisitorNavigationMode> dict = new Dictionary<INode, ResolveVisitorNavigationMode>();
		
		/// <summary>
		/// Creates a new NodeListResolveVisitorNavigator that resolves the specified nodes.
		/// </summary>
		public NodeListResolveVisitorNavigator(IEnumerable<INode> nodes)
		{
			if (nodes == null)
				throw new ArgumentNullException("nodes");
			foreach (INode node in nodes) {
				dict[node] = ResolveVisitorNavigationMode.Resolve;
				for (INode ancestor = node.Parent; ancestor != null && !dict.ContainsKey(ancestor); ancestor = ancestor.Parent) {
					dict.Add(ancestor, ResolveVisitorNavigationMode.Scan);
				}
			}
		}
		
		/// <inheritdoc/>
		public ResolveVisitorNavigationMode Scan(INode node)
		{
			ResolveVisitorNavigationMode mode;
			if (dict.TryGetValue(node, out mode))
				return mode;
			else
				return ResolveVisitorNavigationMode.Skip;
		}
	}
}
