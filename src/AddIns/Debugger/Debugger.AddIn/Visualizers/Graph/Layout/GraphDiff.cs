// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Describes changes between 2 <see cref="PositionedGraph"/>s.
	/// </summary>
	public class GraphDiff
	{
		private List<PositionedGraphNode> addedNodes = new List<PositionedGraphNode>();
		private List<PositionedGraphNode> deletedNodes = new List<PositionedGraphNode>();
		private List<PositionedGraphNode> changedNodes = new List<PositionedGraphNode>();
		private Dictionary<PositionedGraphNode, PositionedGraphNode> matching = new Dictionary<PositionedGraphNode, PositionedGraphNode>();
		
		/// <summary>
		/// Nodes in the new graph that were added.
		/// </summary>
		public IList<PositionedGraphNode> AddedNodes
		{
			get { return addedNodes.AsReadOnly(); }
		}
		
		/// <summary>
		/// Nodes in the old graph that were removed.
		/// </summary>
		public IList<PositionedGraphNode> RemovedNodes
		{
			get { return deletedNodes.AsReadOnly(); }
		}
		
		/// <summary>
		/// Nodes in the old graph that were chaged.
		/// These have matching new nodes, which can be obtained by <see cref="GetMatchingNewNode"/>.
		/// </summary>
		public IList<PositionedGraphNode> ChangedNodes
		{
			get { return changedNodes.AsReadOnly(); }
		}
		
		public PositionedGraphNode GetMatchingNewNode(PositionedGraphNode oldNode)
		{
			return matching.GetValue(oldNode);
		}
		
		internal void SetAdded(PositionedGraphNode addedNode)
		{
			addedNodes.Add(addedNode);
		}
		
		internal void SetRemoved(PositionedGraphNode removeddNode)
		{
			deletedNodes.Add(removeddNode);
		}
		
		internal void SetMatching(PositionedGraphNode matchFrom, PositionedGraphNode matchTo)
		{
			matching[matchFrom] = matchTo;
			changedNodes.Add(matchFrom);
		}
		
		public GraphDiff()
		{
			
		}
		
		/*public void MakeReadOnly()
		{
			addedNodes = ((List<PositionedNode>)addedNodes).AsReadOnly();
			deletedNodes = ((List<PositionedNode>)deletedNodes).AsReadOnly();
			changedNodes = ((List<PositionedNode>)changedNodes).AsReadOnly();
		}*/
	}
}
