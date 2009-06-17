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
		/// Nodes in the old graph that were chaged.
		/// These have matching new nodes, which can be obtained by <see cref="GetMatchingNewNode"/>.
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
