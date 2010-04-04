// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Calculates diff of 2 <see cref="PositionedGraph"/>s.
	/// </summary>
	public class GraphMatcher
	{
		public GraphMatcher()
		{
		}
		
		public GraphDiff MatchGraphs(PositionedGraph oldGraph, PositionedGraph newGraph)
		{
			if (oldGraph == null)
			{
				if (newGraph == null)
				{
					return new GraphDiff();
				}
				else
				{
					GraphDiff addAllDiff = new GraphDiff();
					foreach	(PositionedGraphNode newNode in newGraph.Nodes)
						addAllDiff.SetAdded(newNode);
					return addAllDiff;
				}
			}
			else if (newGraph == null)
			{
				GraphDiff removeAllDiff = new GraphDiff();
				foreach	(PositionedGraphNode oldNode in oldGraph.Nodes)
					removeAllDiff.SetRemoved(oldNode);
				return removeAllDiff;
			}
			
			// none of the graphs is null
			GraphDiff diff = new GraphDiff();
			
			Dictionary<int, PositionedGraphNode> newNodeForHashCode = buildHashToNodeMap(newGraph);
			Dictionary<PositionedGraphNode, bool> newNodeMatched = new Dictionary<PositionedGraphNode, bool>();
			
			foreach	(PositionedGraphNode oldNode in oldGraph.Nodes)
			{
				PositionedGraphNode matchingNode = matchNode(oldNode, newNodeForHashCode);
				if (matchingNode != null)
				{
					diff.SetMatching(oldNode, matchingNode);
					newNodeMatched[matchingNode] = true;
				}
				else
				{
					diff.SetRemoved(oldNode);
				}
			}
			foreach	(PositionedGraphNode newNode in newGraph.Nodes)
			{
				if (!newNodeMatched.ContainsKey(newNode))
				{
					diff.SetAdded(newNode);
				}
			}
			
			return diff;
		}
		
		private Dictionary<int, PositionedGraphNode> buildHashToNodeMap(PositionedGraph graph)
		{
			var hashToNodeMap = new Dictionary<int, PositionedGraphNode>();
			foreach (PositionedGraphNode node in graph.Nodes)
			{
				hashToNodeMap[node.ObjectNode.HashCode] = node;
			}
			return hashToNodeMap;
		}
		
		private PositionedGraphNode matchNode(PositionedGraphNode oldNode, Dictionary<int, PositionedGraphNode> newNodeMap)
		{
			PositionedGraphNode newNodeFound = newNodeMap.GetValue(oldNode.ObjectNode.HashCode);
			if ((newNodeFound != null) && isSameAddress(oldNode, newNodeFound))
			{
				return newNodeFound;
			}
			else
			{
				return null;
			}
		}
		
		private bool isSameAddress(PositionedGraphNode node1, PositionedGraphNode node2)
		{
			return node1.ObjectNode.PermanentReference.GetObjectAddress() == node2.ObjectNode.PermanentReference.GetObjectAddress();
		}
	}
}
