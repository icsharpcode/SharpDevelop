// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Describes changes which occured between 2 <see cref="PositionedGraph"/>s.
	/// </summary>
	public class GraphDiff
	{
		private List<PositionedNode> addedNodes = new List<PositionedNode>();
		private List<PositionedNode> deletedNodes = new List<PositionedNode>();
		private List<PositionedNode> changedNodes = new List<PositionedNode>();
		private Dictionary<PositionedNode, PositionedNode> matching = new Dictionary<PositionedNode, PositionedNode>();
		
		/// <summary>
		/// Nodes in the new graph that were added.
		/// </summary>
		public IList<PositionedNode> AddedNodes
		{
			get { return addedNodes.AsReadOnly(); }
		}
		
		/// <summary>
		/// Nodes in the old graph that were removed.
		/// </summary>
		public IList<PositionedNode> RemovedNodes
		{
			get { return deletedNodes.AsReadOnly(); }
		}
		
		/// <summary>
		/// Nodes in the old graph that are present also in the new graph (they represent the same debuggee instance).
		/// The matching new nodes can be obtained by <see cref="GetMatchingNewNode"/>.
		/// </summary>
		public IList<PositionedNode> ChangedNodes
		{
			get { return changedNodes.AsReadOnly(); }
		}
		
		public PositionedNode GetMatchingNewNode(PositionedNode oldNode)
		{
			return matching.GetValue(oldNode);
		}
		
		internal void SetAdded(PositionedNode addedNode)
		{
			addedNodes.Add(addedNode);
		}
		
		internal void SetRemoved(PositionedNode removeddNode)
		{
			deletedNodes.Add(removeddNode);
		}
		
		internal void SetMatching(PositionedNode matchFrom, PositionedNode matchTo)
		{
			matching[matchFrom] = matchTo;
			changedNodes.Add(matchFrom);
		}
	}
}
