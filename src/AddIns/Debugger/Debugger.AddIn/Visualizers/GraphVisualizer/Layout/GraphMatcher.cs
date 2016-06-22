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
using ICSharpCode.SharpDevelop;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Calculates diff between 2 <see cref="PositionedGraph"/>s.
	/// </summary>
	public class GraphMatcher
	{
		/// <summary>
		/// Calculates diff between 2 <see cref="PositionedGraph"/>s. 
		/// The <see cref="GraphDiff"/> describes a matching between nodes in the graphs, added and removed nodes.
		/// </summary>
		public GraphDiff MatchGraphs(PositionedGraph oldGraph, PositionedGraph newGraph)
		{
			if (oldGraph == null) {
				if (newGraph == null) {
					return new GraphDiff();
				} else {
					GraphDiff addAllDiff = new GraphDiff();
					foreach	(PositionedNode newNode in newGraph.Nodes)
						addAllDiff.SetAdded(newNode);
					return addAllDiff;
				}
			} else if (newGraph == null) {
				GraphDiff removeAllDiff = new GraphDiff();
				foreach	(PositionedNode oldNode in oldGraph.Nodes)
					removeAllDiff.SetRemoved(oldNode);
				return removeAllDiff;
			}
			// none of the graphs is null
			
			GraphDiff diff = new GraphDiff();
			
			Dictionary<int, PositionedNode> newNodeForHashCode = BuildHashToNodeMap(newGraph);
			Dictionary<PositionedNode, bool> newNodeMatched = new Dictionary<PositionedNode, bool>();
			
			foreach	(PositionedNode oldNode in oldGraph.Nodes) {
				PositionedNode matchingNode = MatchNode(oldNode, newNodeForHashCode);
				
				if (matchingNode != null) {
					diff.SetMatching(oldNode, matchingNode);
					newNodeMatched[matchingNode] = true;
				} else {
					diff.SetRemoved(oldNode);
				}
			}
			foreach	(PositionedNode newNode in newGraph.Nodes) {
				if (!newNodeMatched.ContainsKey(newNode)) {
					diff.SetAdded(newNode);
				}
			}
			return diff;
		}
		
		Dictionary<int, PositionedNode> BuildHashToNodeMap(PositionedGraph graph)
		{
			var hashToNodeMap = new Dictionary<int, PositionedNode>();
			foreach (PositionedNode node in graph.Nodes) {
				hashToNodeMap[node.ObjectNode.HashCode] = node;
			}
			return hashToNodeMap;
		}
		
		PositionedNode MatchNode(PositionedNode oldNode, Dictionary<int, PositionedNode> newNodeMap)
		{
			PositionedNode newNodeFound = newNodeMap.GetOrDefault(oldNode.ObjectNode.HashCode);
			if ((newNodeFound != null) && IsSameAddress(oldNode, newNodeFound))	{
				return newNodeFound;
			} else {
				return null;
			}
		}
		
		bool IsSameAddress(PositionedNode node1, PositionedNode node2)
		{
			return node1.ObjectNode.PermanentReference.GetObjectAddress() == node2.ObjectNode.PermanentReference.GetObjectAddress();
		}
	}
}
